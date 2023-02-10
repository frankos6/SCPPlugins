﻿using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp173;
using Exiled.Events.Handlers;
using PlayerRoles;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.API.Features.Server;
using PlayerRoles.PlayableScps.Scp173;

namespace SCPPlugins.MrugaczNerf
{
    public class MrugaczNerf : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);
        
        /// <inheritdoc />
        public override void OnEnabled()
        {
            Player.InteractingDoor += PlayerOnInteractingDoor;
            Scp173.Blinking += Scp173OnBlinking;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            base.OnEnabled();
        }
        
        /// <inheritdoc />
        public override void OnDisabled()
        {
            Player.InteractingDoor -= PlayerOnInteractingDoor;
            Scp173.Blinking -= Scp173OnBlinking;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            base.OnDisabled();
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingDoor"/>
        private static void PlayerOnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Door.Room.Type != RoomType.HczEzCheckpointA && ev.Door.Room.Type != RoomType.HczEzCheckpointA) return;
            if (ev.Player.Role == RoleTypeId.Scientist || ev.Player.Role == RoleTypeId.ClassD)
            {
                Server.SessionVariables["LateGame"] = true;
            }
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Scp173.OnBlinking"/>
        private void Scp173OnBlinking(BlinkingEventArgs ev)
        {
            ev.BlinkCooldown = Server.SessionVariables["LateGame"].Equals(false) ? Config.BlinkCooldown : Scp173BlinkTimer.CooldownBaseline;
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        private static void ServerOnRoundStarted()
        {
            Server.SessionVariables["LateGame"] = false;
        }
    }
}