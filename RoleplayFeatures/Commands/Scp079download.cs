namespace RoleplayFeatures.Commands;

using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.API.Features.Doors;
using System.Linq;
using MEC;

[CommandHandler(typeof(ClientCommandHandler))]
public class Scp079DownloadCommand : ICommand
{
    private Config config => Plugin.Instance.Config;

    public string Command { get; } = "079download";

    public string[] Aliases { get; } = Array.Empty<string>();

    public string Description { get; } = "Allows to download SCP-079 for its escape";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get((CommandSender)sender);

        if (!config.IsScp079Downloadable)
        {
            response = "Command is unavailable";
            return false;
        }

        if (!(player.Role == RoleTypeId.Scp049 || player.IsHuman))
        {
            response = "Command is available for humans and SCP-049 only";
            return false;
        }

        bool isScp079exists = false;

        foreach (Player p in Player.List)
        {
            if (p.Role == RoleTypeId.Scp079)
            {
                isScp079exists = true;
                break;
            }
        }

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

        if (config.IsScp079DownloadCassieEnabled)
            Cassie.Message(config.Scp079CassieDownloadMessage);

        if (Plugin.active079Downloads.ContainsKey(player.Id))
        {
            Timing.KillCoroutines(Plugin.active079Downloads[player.Id]);
            Plugin.active079Downloads.Remove(player.Id);
        }

        CoroutineHandle handle = Timing.RunCoroutine(Plugin.Instance.handlers.Download079Coroutine(player));
        Plugin.active079Downloads[player.Id] = handle;

        return true;
    }
}
