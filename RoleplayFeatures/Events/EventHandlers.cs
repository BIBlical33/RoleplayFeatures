// Copyright 2025, github.com/BIBlical33
//
// Handling the events specified in Plugin.cs
//
// License: Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
// See: https://creativecommons.org/licenses/by-sa/3.0/

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp914;
using Exiled.Events.EventArgs.Warhead;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features.Doors;
using MEC;
using System.Collections.ObjectModel;

namespace RoleplayFeatures.Events;

public class EventHandlers
{
    private Config Config => Plugin.Instance.Config;

    private Translation Translation => Plugin.Instance.Translation;

    private readonly Dictionary<int, string> originalNames = [];

    private readonly Dictionary<int, int> scp096TargetsAggroCount = [];

    private readonly Dictionary<int, bool> scp096TargetsFinalAggroStatus = [];

    private static readonly HashSet<RoleTypeId> mainScps = [
            RoleTypeId.Scp049,
            RoleTypeId.Scp079,
            RoleTypeId.Scp096,
            RoleTypeId.Scp106,
            RoleTypeId.Scp173,
            RoleTypeId.Scp939,
            RoleTypeId.Scp3114
    ];

    private readonly Dictionary<int, DateTime> scpIsEscaped = [];

    public void OnTransmitting(TransmittingEventArgs ev)
    {
        if (Config.IsUnknownTransmittingEnabled)
        {
            if (ev.Player.IsTransmitting)
            {
                if (!originalNames.ContainsKey(ev.Player.Id))
                {
                    originalNames[ev.Player.Id] = ev.Player.CustomName;
                }

                ev.Player.CustomName = Config.RadioCustomName;
            }
            else
            {
                if (originalNames.TryGetValue(ev.Player.Id, out string originalName))
                {
                    ev.Player.CustomName = originalName;
                    originalNames.Remove(ev.Player.Id);
                }
            }
        }
    }

    public void OnDeactivatingGenerator(StoppingGeneratorEventArgs ev)
    {
        if (IsNonSentientScp(ev.Player))
            ev.IsAllowed = false;
    }

    public void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev)
    {
        if (IsNonSentientScp(ev.Player))
            ev.IsAllowed = false;
    }

    public void OnStartingDetonation(StartingEventArgs ev)
    {
        if (ev.Player.Role.Side == Side.Scp)
            ev.IsAllowed = false;
    }

    public void OnStoppingDetonation(StoppingEventArgs ev)
    {
        if (IsNonSentientScp(ev.Player))
            ev.IsAllowed = false;
    }

    public void OnScp914Activating(ActivatingEventArgs ev)
    {
        if (IsNonSentientScp(ev.Player))
            ev.IsAllowed = false;
    }

    public void OnChangingKnobSetting(ChangingKnobSettingEventArgs ev)
    {
        if (IsNonSentientScp(ev.Player))
            ev.IsAllowed = false;
    }

    public void OnAddingTarget(AddingTargetEventArgs ev)
    {
        if (!scp096TargetsAggroCount.ContainsKey(ev.Target.Id))
        {
            scp096TargetsAggroCount[ev.Target.Id] = 1;
        }
        else
        {
            scp096TargetsAggroCount[ev.Target.Id]++;
            if (scp096TargetsAggroCount[ev.Target.Id] > Config.EscapingByElevatorMaxTimes)
            {
                scp096TargetsFinalAggroStatus[ev.Target.Id] = true;
            }
        }
    }

    public void OnInteractingElevator(InteractingElevatorEventArgs ev)
    {
        if (scp096TargetsFinalAggroStatus.TryGetValue(ev.Player.Id, out bool isNotPosibleToInteracteElevator) &&
            isNotPosibleToInteracteElevator)
        {
            ev.IsAllowed = false;
            ev.Player.ShowHint(Translation.Scp096ElevatorHint);
        }
    }

    public void OnCalmingDown(CalmingDownEventArgs ev)
    {
        foreach (var key in scp096TargetsFinalAggroStatus.Keys)
        {
            scp096TargetsFinalAggroStatus[key] = false;
        }
    }

    public void OnChangingRole(ChangingRoleEventArgs ev)
    {
        originalNames.Remove(ev.Player.Id);
        scp096TargetsFinalAggroStatus.Remove(ev.Player.Id);
        scp096TargetsAggroCount.Remove(ev.Player.Id);

        if (Config.IsInfinityWavesTokensEnabled)
        {
            bool ntf_result = Respawn.TryGetTokens(SpawnableFaction.NtfWave, out int ntfTokens), ci_result = Respawn.TryGetTokens(SpawnableFaction.ChaosWave, out int ciTokens);

            if (ntf_result && ntfTokens == 0) Respawn.ModifyTokens(PlayerRoles.Faction.FoundationStaff, 1);

            if (ci_result && ciTokens == 0) Respawn.ModifyTokens(PlayerRoles.Faction.FoundationEnemy, 1);
        }

        if (Config.IsScpEscapeCassiesEnabled && scpIsEscaped.ContainsKey(ev.Player.Id))
        {
            if (ev.NewRole == RoleTypeId.Spectator && mainScps.Contains(ev.Player.Role) && (DateTime.UtcNow - scpIsEscaped[ev.Player.Id]).TotalSeconds < 20)
            {
                string escapingScpName = ev.Player.Role.Name, scpCassieName = "SCP ";

                for (int i = 0; i < escapingScpName.Length; ++i)
                    if (char.IsDigit(escapingScpName[i]))
                        scpCassieName += escapingScpName[i] + " ";

                Cassie.Message(string.Format(Config.ScpEscapeCassieContent, scpCassieName));
            }
            else
            {
                scpIsEscaped.Remove(ev.Player.Id);
            }
        }

        if (Config.Scp079Escape.IsDownloadable)
        {
            if (Plugin.active079Downloads.TryGetValue(ev.Player.Id, out var handle))
            {
                Timing.KillCoroutines(handle);
                Plugin.active079Downloads.Remove(ev.Player.Id);
            }

            Plugin.has079FlashDrive.Remove(ev.Player.Id);
        }
    }

    public void OnRoundStarted()
    {
        DataStructuresClear();

        if (Config.Scp079Escape.IsDownloadable)
        {
            var rooms = Door.Get(DoorType.Scp079First).Rooms
                .Concat(Door.Get(DoorType.Scp079Second).Rooms)
                .Concat(Door.Get(DoorType.Scp079Armory).Rooms);

            foreach (var room in rooms)
                Plugin.scp079Rooms.Add(room);
        }
    }

    public void OnEscaping(EscapingEventArgs ev)
    {
        if (Config.IsChaosEscapeAllowed)
        {
            if (ev.Player.Role.Side == Side.ChaosInsurgency && (ev.Player.CountItem(ItemCategory.SCPItem) + ev.Player.CountItem(ItemCategory.SpecialWeapon) >= Config.ScpsAndSpecialWeaponsCountToChaosEscape))
            {
                ev.Player.ClearInventory();
                ev.Player.Role.Set(RoleTypeId.Spectator);
            }
        }

        if (Config.KeepEffectsAfterEscaping)
        {
            if (ev.Player.Role.Side != Side.Scp && ev.IsAllowed && ev.NewRole != RoleTypeId.Spectator)
            {
                Plugin.escapingPlayerEffects[ev.Player.Id] = [.. ev.Player.ActiveEffects.Select(e => (e.GetEffectType(), e.Intensity, e.TimeLeft))];

                Plugin.escapeTimes[ev.Player.Id] = DateTime.UtcNow;
            }
        }

        if (Config.IsScpEscapeCassiesEnabled)
        {
            if (mainScps.Contains(ev.Player.Role))
                scpIsEscaped[ev.Player.Id] = DateTime.UtcNow;
        }

        if (Config.Scp079Escape.IsDownloadable && !ev.Player.IsCuffed)
        {
            if (Plugin.has079FlashDrive.Contains(ev.Player.Id))
            {
                Plugin.has079FlashDrive.Remove(ev.Player.Id);

                foreach (Player player in Player.List)
                {
                    if (player.Role == RoleTypeId.Scp079)
                        player.Role.Set(RoleTypeId.Spectator);
                }

                if (Config.IsScpEscapeCassiesEnabled)
                    Cassie.Message(string.Format(Config.ScpEscapeCassieContent, "SCP 0 7 9"));
            }
        }
    }

    private bool IsNonSentientScp(Player player)
    {
        return player.Role.Side == Side.Scp && player.Role != RoleTypeId.Scp049;
    }

    private void DataStructuresClear()
    {
        originalNames.Clear();
        scp096TargetsAggroCount.Clear();
        scp096TargetsFinalAggroStatus.Clear();
        scpIsEscaped.Clear();
        Plugin.escapeTimes.Clear();
        Plugin.escapingPlayerEffects.Clear();
        Plugin.scp079Rooms.Clear();
        Plugin.active079Downloads.Clear();
        Plugin.has079FlashDrive.Clear();
    }
}
