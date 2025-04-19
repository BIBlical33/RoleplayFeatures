using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.API.Features.Doors;
using System.Linq;
using MEC;

namespace RoleplayImmersion
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Scp079DownloadCommand : ICommand
    {

        private Config Config => Plugin.Instance?.Config;

        public string Command { get; } = "079download";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Allows to download SCP-079 for its escape";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((CommandSender)sender);

            if (!Config.IsScp079Downloadable)
            {
                response = "The command is unavailable";
                return false;
            }

            if (!(player.Role == RoleTypeId.Scp049 || player.IsHuman))
            {
                response = "The command is available for humans and SCP-049 only";
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

            if (Config.IsScp079DownloadCassieEnabled)
                Cassie.Message(Config.Scp079CassieDownloadMessage);

            if (Plugin.active079Downloads.ContainsKey(player.Id))
            {
                Timing.KillCoroutines(Plugin.active079Downloads[player.Id]);
                Plugin.active079Downloads.Remove(player.Id);
            }

            CoroutineHandle handle = Timing.RunCoroutine(Plugin.Instance._handlers.Download079Coroutine(player));
            Plugin.active079Downloads[player.Id] = handle;

            return true;
        }
    }
}