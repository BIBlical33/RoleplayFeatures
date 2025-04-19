using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.API.Features.Doors;

namespace RoleplayImmersion
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Scp079GatesCommand : ICommand
    {

        private Config Config => Plugin.Instance?.Config;

        public string Command { get; } = "079gates";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "SCP-079 gates control";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Config.IsScp079Downloadable)
            {
                response = "The command is unavailable";
                return false;
            }

            if (!(Player.Get((CommandSender)sender).Role == RoleTypeId.Scp079))
            {
                response = "The command is available for SCP-079 only";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "\nPlease enter a valid subcommand:\nopen\nclose";
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

            response = "\nPlease enter a valid subcommand:\nopen\nclose";
            return false;
        }
    }
}
