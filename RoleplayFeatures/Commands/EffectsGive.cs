namespace RoleplayFeatures.Commands;

using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.API.Enums;

[CommandHandler(typeof(ClientCommandHandler))]
public class EffectsGive : ICommand
{

    private Config config => Plugin.Instance.Config;

    public string Command { get; } = "effectsgive";

    public string[] Aliases { get; } = Array.Empty<string>();

    public string Description { get; } = "Returns effects after escaping";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get((CommandSender)sender);

        if (!config.KeepEffectsAfterEscaping)
        {
            response = "Command is unavailable";
            return false;
        }

        if (!player.IsHuman)
        {
            response = "Command is available for humans only";
            return false;
        }

        Plugin.escapingPlayerEffects.TryGetValue(player.Id, out var effects);
        Plugin.escapeTimes.TryGetValue(player.Id, out var escapeTime);

        if (effects != null && (DateTime.UtcNow - escapeTime).TotalSeconds <= config.TimeToApplyEffects)
        {
            foreach (var (type, intensity, remaining) in Plugin.escapingPlayerEffects[player.Id])
                if (type != EffectType.Scp1344)
                    player.EnableEffect(type, intensity, duration: remaining);

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
