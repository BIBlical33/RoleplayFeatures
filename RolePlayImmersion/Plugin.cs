using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;

namespace RoleplayImmersion
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "BIBlical";

        public override string Name => "RolePlayImmersion";
        
        public override string Prefix => Name;

        public override Version RequiredExiledVersion { get; } = new Version(9, 0, 0);

        public override Version Version { get; } = new Version(1, 1, 0);

        public static Plugin Instance;

        public static Dictionary<int, List<(EffectType Type, byte Intensity, float RemainingTime)>> escapingPlayerEffects = new();

        public static Dictionary<int, DateTime> escapeTimes { get; } = new();

        private EventHandlers _handlers { get; set; }

        public override void OnEnabled()
        {
            Instance = this;
            _handlers = new EventHandlers(Config);

            Exiled.Events.Handlers.Player.Transmitting += _handlers.OnTransmitting;
            Exiled.Events.Handlers.Player.ChangingRole += _handlers.OnChangingRole;
            Exiled.Events.Handlers.Player.StoppingGenerator += _handlers.OnDeactivatingGenerator;
            Exiled.Events.Handlers.Player.InteractingElevator += _handlers.OnInteractingElevator;
            Exiled.Events.Handlers.Player.Escaping += _handlers.OnEscaping;

            Exiled.Events.Handlers.Server.RoundStarted += _handlers.OnRoundStarted;

            Exiled.Events.Handlers.Warhead.ChangingLeverStatus += _handlers.OnChangingLeverStatus;
            Exiled.Events.Handlers.Warhead.Starting += _handlers.OnStartingDetonation;
            Exiled.Events.Handlers.Warhead.Stopping += _handlers.OnStoppingDetonation;

            Exiled.Events.Handlers.Scp914.Activating += _handlers.OnScp914Activating;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting += _handlers.OnChangingKnobSetting;

            Exiled.Events.Handlers.Scp096.AddingTarget += _handlers.OnAddingTarget;
            Exiled.Events.Handlers.Scp096.CalmingDown += _handlers.OnCalmingDown;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Transmitting -= _handlers.OnTransmitting;
            Exiled.Events.Handlers.Player.ChangingRole -= _handlers.OnChangingRole;
            Exiled.Events.Handlers.Player.InteractingElevator -= _handlers.OnInteractingElevator;
            Exiled.Events.Handlers.Player.StoppingGenerator -= _handlers.OnDeactivatingGenerator;
            Exiled.Events.Handlers.Player.Escaping -= _handlers.OnEscaping;

            Exiled.Events.Handlers.Server.RoundStarted -= _handlers.OnRoundStarted;
            
            Exiled.Events.Handlers.Warhead.ChangingLeverStatus -= _handlers.OnChangingLeverStatus;
            Exiled.Events.Handlers.Warhead.Starting -= _handlers.OnStartingDetonation;
            Exiled.Events.Handlers.Warhead.Stopping -= _handlers.OnStoppingDetonation;

            Exiled.Events.Handlers.Scp914.Activating -= _handlers.OnScp914Activating;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting -= _handlers.OnChangingKnobSetting;

            Exiled.Events.Handlers.Scp096.AddingTarget -= _handlers.OnAddingTarget;
            Exiled.Events.Handlers.Scp096.CalmingDown -= _handlers.OnCalmingDown;

            _handlers = null;

            base.OnDisabled();
        }
    }
}