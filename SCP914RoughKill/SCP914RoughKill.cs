﻿using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp914;
using Scp914;

namespace SCPPlugins.SCP914RoughKill
{
    // ReSharper disable once InconsistentNaming
    public class SCP914RoughKill : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version RequiredExiledVersion => new Version(7, 2, 0, 0);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp914.UpgradingPlayer += Scp914OnUpgradingPlayer;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp914.UpgradingPlayer -= Scp914OnUpgradingPlayer;
            base.OnDisabled();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Scp914.OnUpgradingPlayer" />
        private static void Scp914OnUpgradingPlayer(UpgradingPlayerEventArgs ev)
        {
            if (ev.KnobSetting != Scp914KnobSetting.Rough) return;
            ev.Player.Kill("Rozpierdolenie się totalne");
        }
    }
}