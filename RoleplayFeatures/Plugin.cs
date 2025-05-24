// Copyright 2025, github.com/BIBlical33
//
// Main class of plugin
//
// License: Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
// See: https://creativecommons.org/licenses/by-sa/3.0/

using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using MEC;
using RoleplayFeatures.Events;

namespace RoleplayFeatures
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Author => "BIBlical";

        public override string Name => "RoleplayFeatures";

        public override string Prefix => Name;

        public override Version RequiredExiledVersion { get; } = new Version(9, 0, 0);

        public override Version Version { get; } = new Version(1, 3, 3);

        public static Plugin Instance { get; private set; } = null!;

        private EventHandlers? handlers;

        internal static Dictionary<int, List<(EffectType Type, byte Intensity, float RemainingTime)>> escapingPlayerEffects = [];

        internal static Dictionary<int, DateTime> escapeTimes = [];

        internal static HashSet<Room> scp079Rooms = [];

        internal static Dictionary<int, CoroutineHandle> active079Downloads = [];

        internal static HashSet<int> has079FlashDrive = [];

        public override void OnEnabled()
        {
            Instance = this;
            handlers = new EventHandlers();

            Exiled.Events.Handlers.Player.Transmitting += handlers.OnTransmitting;
            Exiled.Events.Handlers.Player.ChangingRole += handlers.OnChangingRole;
            Exiled.Events.Handlers.Player.StoppingGenerator += handlers.OnDeactivatingGenerator;
            Exiled.Events.Handlers.Player.InteractingElevator += handlers.OnInteractingElevator;
            Exiled.Events.Handlers.Player.Escaping += handlers.OnEscaping;

            Exiled.Events.Handlers.Server.RoundStarted += handlers.OnRoundStarted;

            Exiled.Events.Handlers.Warhead.ChangingLeverStatus += handlers.OnChangingLeverStatus;
            Exiled.Events.Handlers.Warhead.Starting += handlers.OnStartingDetonation;
            Exiled.Events.Handlers.Warhead.Stopping += handlers.OnStoppingDetonation;

            Exiled.Events.Handlers.Scp914.Activating += handlers.OnScp914Activating;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting += handlers.OnChangingKnobSetting;

            Exiled.Events.Handlers.Scp096.AddingTarget += handlers.OnAddingTarget;
            Exiled.Events.Handlers.Scp096.CalmingDown += handlers.OnCalmingDown;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            if (handlers is null)
                return;

            Exiled.Events.Handlers.Player.Transmitting -= handlers.OnTransmitting;
            Exiled.Events.Handlers.Player.ChangingRole -= handlers.OnChangingRole;
            Exiled.Events.Handlers.Player.InteractingElevator -= handlers.OnInteractingElevator;
            Exiled.Events.Handlers.Player.StoppingGenerator -= handlers.OnDeactivatingGenerator;
            Exiled.Events.Handlers.Player.Escaping -= handlers.OnEscaping;

            Exiled.Events.Handlers.Server.RoundStarted -= handlers.OnRoundStarted;

            Exiled.Events.Handlers.Warhead.ChangingLeverStatus -= handlers.OnChangingLeverStatus;
            Exiled.Events.Handlers.Warhead.Starting -= handlers.OnStartingDetonation;
            Exiled.Events.Handlers.Warhead.Stopping -= handlers.OnStoppingDetonation;

            Exiled.Events.Handlers.Scp914.Activating -= handlers.OnScp914Activating;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting -= handlers.OnChangingKnobSetting;

            Exiled.Events.Handlers.Scp096.AddingTarget -= handlers.OnAddingTarget;
            Exiled.Events.Handlers.Scp096.CalmingDown -= handlers.OnCalmingDown;

            handlers = null;
            Instance = null!;

            base.OnDisabled();
        }
    }
}