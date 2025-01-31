using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp914;
using Exiled.Events.EventArgs.Warhead;
using PlayerRoles;
using System.Collections.Generic;

namespace RoleplayImmersion
{
    public class EventHandler
    {
        private readonly Config _config;

        private readonly Dictionary<int, string> originalNames = new();

        private readonly Dictionary<int, int> scp096TargetsAggroCount = new();

        private readonly Dictionary<int, bool> scp096TargetsThirdAggroStatus = new();

        public EventHandler(Config config) => _config = config;

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
                    scp096TargetsThirdAggroStatus[ev.Target.Id] = true;
                }
            }

        }

        public void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            if (scp096TargetsThirdAggroStatus.TryGetValue(ev.Player.Id, out bool isNotPosibleToInteracteElevator) &&
                isNotPosibleToInteracteElevator)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint(_config.HintMessage);
            }
        }

        public void OnCalmingDown(CalmingDownEventArgs ev)
        {
            foreach (var key in new List<int>(scp096TargetsThirdAggroStatus.Keys))
            {
                scp096TargetsThirdAggroStatus[key] = false;
            }
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            originalNames.Remove(ev.Player.Id);
            scp096TargetsThirdAggroStatus.Remove(ev.Player.Id);
            scp096TargetsAggroCount.Remove(ev.Player.Id);

            // I'm C++ developer moment
            int ntf_tokens, ci_tokens;
            bool ntf_result = Respawn.TryGetTokens(SpawnableFaction.NtfWave, out ntf_tokens), ci_result = Respawn.TryGetTokens(SpawnableFaction.ChaosWave, out ci_tokens);

            if (ntf_result && ntf_tokens == 0) Respawn.ModifyTokens(PlayerRoles.Faction.FoundationStaff, 1);
            if (ci_result && ci_tokens == 0) Respawn.ModifyTokens(PlayerRoles.Faction.FoundationEnemy, 1);
        }

        public void OnRoundStarted()
        {
            originalNames.Clear();
            scp096TargetsAggroCount.Clear();
            scp096TargetsThirdAggroStatus.Clear();
        }
    }
}
