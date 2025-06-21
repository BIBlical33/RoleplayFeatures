// Copyright 2025, github.com/BIBlical33
//
// .079download command
//
// License: Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
// See: https://creativecommons.org/licenses/by-sa/3.0/

using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.API.Features.Doors;
using System.Linq;
using MEC;
using System.Collections.Generic;

namespace RoleplayFeatures.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
public class Scp079DownloadCommand : ICommand
{
    private Config Config => Plugin.Instance.Config;

    public string Command { get; } = "079download";

    public string[] Aliases { get; } = ["79download", "scp079download", "download079"];

    public string Description { get; } = "Allows to download SCP-079 for its escape";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get((CommandSender)sender);

        if (!Config.Scp079Escape.IsDownloadable)
        {
            response = "Command is unavailable";
            return false;
        }

        if (!(player.Role == RoleTypeId.Scp049 || player.IsHuman))
        {
            response = "Command is available for humans and SCP-049 only";
            return false;
        }

        bool isScp079exists = Player.Get(RoleTypeId.Scp079).Any();
        if (!isScp079exists)
        {
            response = "There is no SCP-079 player on the server";
            return false;
        }

        if (!Door.Get(DoorType.Scp079Armory).Rooms.ToList().Contains(player.CurrentRoom))
        {
            response = "You must be in SCP-079's containment cell to use the command";
            return false;
        }

        if (Plugin.active079Downloads.ContainsKey(player.Id))
        {
            response = "You're already downloading SCP-079";
            return false;
        }

        if (Plugin.has079FlashDrive.Contains(player.Id))
        {
            response = "You're already downloaded SCP-079";
            return false;
        }

        response = "The download has started";

        foreach (var room in Plugin.scp079Rooms)
            room.Color = new UnityEngine.Color(1, 0, 0);

        if (Config.Scp079Escape.IsDownloadCassieEnabled)
            Cassie.Message(Config.Scp079Escape.CassieDownloadMessage);

        if (Plugin.active079Downloads.ContainsKey(player.Id))
        {
            Timing.KillCoroutines(Plugin.active079Downloads[player.Id]);
            Plugin.active079Downloads.Remove(player.Id);
        }

        CoroutineHandle handle = Timing.RunCoroutine(Download079Coroutine(player));
        Plugin.active079Downloads[player.Id] = handle;

        return true;
    }

    private IEnumerator<float> Download079Coroutine(Player player)
    {
        float timer = 0f;
        while (timer < Config.Scp079Escape.DownloadDuration)
        {
            if (!Plugin.scp079Rooms.Contains(player.CurrentRoom))
            {
                player.ShowHint(Plugin.Instance.Translation.Scp079DownloadStoppingHint);
                Plugin.active079Downloads.Remove(player.Id);
                yield break;
            }

            yield return Timing.WaitForSeconds(1f);
            timer += 1f;
        }

        Plugin.has079FlashDrive.Add(player.Id);
        Plugin.active079Downloads.Remove(player.Id);
        player.ShowHint(Plugin.Instance.Translation.Scp079DownloadCompletedHint);
    }
}
