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

namespace RoleplayImmersion
{
    public class EventHandlers
    {
        private readonly Config _config;

        private readonly Dictionary<int, string> originalNames = new();

        private readonly Dictionary<int, int> scp096TargetsAggroCount = new();

        private readonly Dictionary<int, bool> scp096TargetsFinalAggroStatus = new();

        private static readonly HashSet<RoleTypeId> mainScps = new() {
            RoleTypeId.Scp049,
            RoleTypeId.Scp079,
            RoleTypeId.Scp096,
            RoleTypeId.Scp106,
            RoleTypeId.Scp173,
            RoleTypeId.Scp939,
            RoleTypeId.Scp3114
        };

        private readonly Dictionary<int, DateTime> scpIsEscaped = new();

        public EventHandlers(Config config) => _config = config;

        public void OnTransmitting(TransmittingEventArgs ev)
        {
            if (_config.IsUnknownTransmittingEnabled)
            {
                if (ev.Player.IsTransmitting)
                {
                    if (!originalNames.ContainsKey(ev.Player.Id))
                    {
                        originalNames[ev.Player.Id] = ev.Player.CustomName;
                    }

                    ev.Player.CustomName = _config.RadioCustomName;
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

        private bool IsNonSentientScp(Player player)
        {
            return player.Role.Side == Side.Scp && player.Role != RoleTypeId.Scp049;
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
                if (scp096TargetsAggroCount[ev.Target.Id] > _config.EscapingByElevatorMaxTimes)
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
                ev.Player.ShowHint(_config.Scp096ElevatorHint);
            }
        }

        public void OnCalmingDown(CalmingDownEventArgs ev)
        {
            foreach (var key in new List<int>(scp096TargetsFinalAggroStatus.Keys))
            {
                scp096TargetsFinalAggroStatus[key] = false;
            }
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            originalNames.Remove(ev.Player.Id);
            scp096TargetsFinalAggroStatus.Remove(ev.Player.Id);
            scp096TargetsAggroCount.Remove(ev.Player.Id);

            if (_config.IsInfinityMtfAndCiTokensEnabled)
            {
                // I'm C++ developer moment
                int ntf_tokens, ci_tokens;
                bool ntf_result = Respawn.TryGetTokens(SpawnableFaction.NtfWave, out ntf_tokens), ci_result = Respawn.TryGetTokens(SpawnableFaction.ChaosWave, out ci_tokens);

                if (ntf_result && ntf_tokens == 0) Respawn.ModifyTokens(PlayerRoles.Faction.FoundationStaff, 1);

                if (ci_result && ci_tokens == 0) Respawn.ModifyTokens(PlayerRoles.Faction.FoundationEnemy, 1);
            }

            if (_config.IsScpEscapeCassiesEnabled)
            {
                if (scpIsEscaped.ContainsKey(ev.Player.Id))
                    if (ev.NewRole == RoleTypeId.Spectator && mainScps.Contains(ev.Player.Role) && (DateTime.UtcNow - scpIsEscaped[ev.Player.Id]).TotalSeconds < 20)
                    {
                        string escapingScpName = ev.Player.Role.Name, scpCassieName = "SCP ";

                        for (int i = 0; i < escapingScpName.Length; ++i)
                        {
                            if (char.IsDigit(escapingScpName[i]))
                            {
                                scpCassieName += escapingScpName[i] + " ";
                            }
                        }

                        Cassie.Message(string.Format(_config.ScpEscapeCassieContent, scpCassieName));
                    }
                    else { scpIsEscaped.Remove(ev.Player.Id); }

            }
        }

        public void OnRoundStarted()
        {
            originalNames.Clear();
            scp096TargetsAggroCount.Clear();
            scp096TargetsFinalAggroStatus.Clear();
            scpIsEscaped.Clear();
            Plugin.escapeTimes.Clear();
        }

        public void OnEscaping(EscapingEventArgs ev)
        {
            if (_config.IsChaosEscapeWithThreeScpsOrSpecialWeaponAllowed)
            {
                if (ev.Player.Role.Side == Side.ChaosInsurgency && (ev.Player.CountItem(ItemCategory.SCPItem) + ev.Player.CountItem(ItemCategory.SpecialWeapon) >= _config.ScpsAndSpecialWeaponsCountToChaosEscape))
                {
                    ev.NewRole = RoleTypeId.Spectator;
                    ev.IsAllowed = true;
                }
            }

            if (_config.KeepEffectsAfterEscaping)
            {
                if (ev.Player.Role.Side != Side.Scp && ev.IsAllowed && ev.NewRole != RoleTypeId.Spectator)
                {
                    Plugin.escapingPlayerEffects[ev.Player.Id] = ev.Player.ActiveEffects.Select(e => (e.GetEffectType(), e.Intensity, e.TimeLeft)).ToList();

                    Plugin.escapeTimes[ev.Player.Id] = DateTime.UtcNow;
                }
            }

            if (_config.IsScpEscapeCassiesEnabled)
            {
                if (mainScps.Contains(ev.Player.Role))
                {
                    scpIsEscaped[ev.Player.Id] = DateTime.UtcNow;
                }
            }
        }
    }
}
