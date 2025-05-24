// Copyright 2025, github.com/BIBlical33
//
// .effectsgive command
//
// License: Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
// See: https://creativecommons.org/licenses/by-sa/3.0/

namespace RoleplayFeatures.Commands;

using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.API.Enums;

[CommandHandler(typeof(ClientCommandHandler))]
public class EffectsGive : ICommand
{
    private Config Config => Plugin.Instance.Config;

    public string Command { get; } = "effectsgive";

    public string[] Aliases { get; } = [];

    public string Description { get; } = "Returns effects after escaping";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get((CommandSender)sender);

        if (!Config.KeepEffectsAfterEscaping)
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

        if (effects != null && (DateTime.UtcNow - escapeTime).TotalSeconds <= Config.TimeToApplyEffects)
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
