using System;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Utils.NonAllocLINQ;
using Player = Exiled.Events.Handlers.Player;

namespace SCPPlugins.DocumentsPlugin
{
    public class DocumentsPlugin : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);

        public override void OnEnabled()
        {
            Player.Joined += PlayerOnJoined;
            Player.Dying += PlayerOnDying;
            Player.Escaping += PlayerOnEscaping;
            Player.Handcuffing += PlayerOnHandcuffing;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            CustomItem.RegisterItems();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Player.Joined -= PlayerOnJoined;
            Player.Dying -= PlayerOnDying;
            Player.Escaping -= PlayerOnEscaping;
            Player.Handcuffing -= PlayerOnHandcuffing;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            CustomItem.UnregisterItems();
            base.OnDisabled();
        }
        
        private void ServerOnRoundStarted()
        {
            Exiled.API.Features.Player.Dictionary.ForEachValue(player =>
            {
                player.SessionVariables["Documents"] = 0;
            });
        }

        private void PlayerOnHandcuffing(HandcuffingEventArgs ev)
        {
            if (ev.Target.Role != RoleTypeId.Scientist && ev.Target.Role != RoleTypeId.FacilityGuard) return;
            if (!ev.Target.TryGetSessionVariable("Documents", out int count))
            {
                throw new Exception($"Could not get Documents variable from {ev.Target.Nickname}");
            }
            ev.Target.SessionVariables["Documents"] = 0;
            for (int i = count; i > 0; i--)
            {
                CustomItem.TrySpawn(1, ev.Target.Position, out Pickup pickup);
                Log.Debug($"Spawned Documents at {pickup.Position} (dropped by {ev.Target.Nickname} on being cuffed)");
            }
        }

        private void PlayerOnEscaping(EscapingEventArgs ev)
        {
            if (ev.Player.Role != RoleTypeId.Scientist && ev.Player.Role != RoleTypeId.FacilityGuard) return;
            if (!ev.Player.TryGetSessionVariable("Documents", out int count))
            {
                throw new Exception($"Could not get Documents variable from {ev.Player.Nickname}");
            }
            if (count == 4)
            {
                ev.Player.SessionVariables["Documents"] = 0;
                string message = "Attention all personnel. The Foundation has secured important containment information";
                Cassie.Message(message,isSubtitles:true);
                Round.EndRound();
            }
            else
            {
                ev.Player.SessionVariables["Documents"] = 0;
                for (int i = count; i > 0; i--)
                {
                    CustomItem.TrySpawn(1, ev.Player.Position, out Pickup pickup);
                    Log.Debug($"Spawned Documents at {pickup.Position} (dropped by {ev.Player.Nickname} on escape)");
                }
            }
        }

        private void PlayerOnDying(DyingEventArgs ev)
        {
            if (ev.Player.Role != RoleTypeId.Scientist && ev.Player.Role != RoleTypeId.FacilityGuard) return;
            if (!ev.Player.TryGetSessionVariable("Documents", out int count))
            {
                throw new Exception($"Could not get Documents variable from {ev.Player.Nickname}");
            }
            ev.Player.SessionVariables["Documents"] = 0;
            for (int i = count; i > 0; i--)
            {
                CustomItem.TrySpawn(1, ev.Player.Position, out Pickup pickup);
                Log.Debug($"Spawned Documents at {pickup.Position} (dropped by {ev.Player.Nickname} on death)");
            }
        }

        private void PlayerOnJoined(JoinedEventArgs ev)
        {
            ev.Player.SessionVariables["Documents"] = 0;
        }
    }
}