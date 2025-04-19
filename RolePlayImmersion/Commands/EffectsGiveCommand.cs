using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.API.Enums;

namespace RoleplayImmersion
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class EffectsGiveCommand : ICommand
    {

        private Config Config => Plugin.Instance?.Config;

        public string Command { get; } = "effectsgive";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Returns effects after escaping";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((CommandSender)sender);

            if (!Config.KeepEffectsAfterEscaping)
            {
                response = "The command is unavailable";
                return false;
            }

            if (!player.IsHuman)
            {
                response = "The command is available for humans only";
                return false;
            }

            Plugin.escapingPlayerEffects.TryGetValue(player.Id, out var effects);
            Plugin.escapeTimes.TryGetValue(player.Id, out var escapeTime);

            if (effects != null && escapeTime != null && (DateTime.UtcNow - escapeTime).TotalSeconds <= Config.TimeToApplyEffects)
            {
                foreach (var (type, intensity, remaining) in Plugin.escapingPlayerEffects[player.Id])
                {
                    if (type != EffectType.Scp1344)
                        player.EnableEffect(type, intensity, duration: remaining);
                }

                Plugin.escapingPlayerEffects.Remove(player.Id);
                Plugin.escapeTimes.Remove(player.Id);
                response = "Granted!";
                return true;
            }
            else
            {
                response = "Time limit exceeded";
                return false;
            }
        }
    }
}