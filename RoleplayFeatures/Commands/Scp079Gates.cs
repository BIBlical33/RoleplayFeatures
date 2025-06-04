// Copyright 2025, github.com/BIBlical33
//
// .079gates command
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

namespace RoleplayFeatures.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
public class Scp079Gates : ICommand
{
    private Config Config => Plugin.Instance.Config;

    public string Command { get; } = "079gates";

    public string[] Aliases { get; } = ["79gates", "scp079gates", "scp79gates"];

    public string Description { get; } = "SCP-079 gates control";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!Config.Scp079Escape.IsDownloadable || Generator.List?.Count(generator => generator?.IsEngaged == true) == 3)
        {
            response = "Command is unavailable";
            return false;
        }

        if (!(Player.Get((CommandSender)sender).Role == RoleTypeId.Scp079))
        {
            response = "Command is available for SCP-079 only";
            return false;
        }

        if (arguments.Count != 1)
        {
            response = "Please enter a valid subcommand:\nopen\nclose";
            return false;
        }

        if (arguments.At(0) == "open")
        {
            Door.Get(DoorType.Scp079First).IsOpen = true;
            Door.Get(DoorType.Scp079Second).IsOpen = true;
            response = "Granted!";
            return true;
        }

        if (arguments.At(0) == "close")
        {
            var players = Player.List;

            foreach (Player player in players)
            {
                if (((player.IsHuman && player.Role != RoleTypeId.Tutorial) || player.Role == RoleTypeId.Scp049) && Plugin.scp079Rooms.Contains(player.CurrentRoom))
                {
                    response = "In order to close the gates, humans and SCP-049 must leave the containment cell";
                    return false;
                }
            }

            Door.Get(DoorType.Scp079First).IsOpen = false; Door.Get(DoorType.Scp079Second).IsOpen = false;
            response = "Granted!";
            return true;
        }

        response = "Please enter a valid subcommand:\nopen\nclose";
        return false;
    }
}
