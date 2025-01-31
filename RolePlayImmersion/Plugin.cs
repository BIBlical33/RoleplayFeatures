using Exiled.API.Features;

namespace RoleplayImmersion
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "BIBlical";

        public override string Name => "RolePlayImmersion";

        public override string Prefix => Name;

        public static Plugin Instance;

        private EventHandler _handlers { get; set; }

        public override void OnEnabled()
        {
            Instance = this;
            _handlers = new EventHandler(Config);

            Exiled.Events.Handlers.Player.Transmitting += _handlers.OnTransmitting;
            Exiled.Events.Handlers.Player.ChangingRole += _handlers.OnChangingRole;
            Exiled.Events.Handlers.Player.StoppingGenerator += _handlers.OnDeactivatingGenerator;
            Exiled.Events.Handlers.Player.InteractingElevator += _handlers.OnInteractingElevator;

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