using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Respawning;
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
            Player.Jumping += PlayerOnJumping;
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
            Player.Jumping -= PlayerOnJumping;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            CustomItem.UnregisterItems();
            base.OnDisabled();
        }
        
        private static void ServerOnRoundStarted()
        {
            Exiled.API.Features.Player.List.ToList().ForEach(player =>
            {
                player.SessionVariables["Documents"] = 0;
            });
        }

        private static void PlayerOnHandcuffing(HandcuffingEventArgs ev)
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

        private static void PlayerOnEscaping(EscapingEventArgs ev)
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
                Exiled.API.Features.Player.List.ToList().ForEach(player =>
                {
                    player.Role.Set(RoleTypeId.NtfPrivate);
                });
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

        private static void PlayerOnDying(DyingEventArgs ev)
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

        private static void PlayerOnJoined(JoinedEventArgs ev)
        {
            ev.Player.SessionVariables["Documents"] = 0;
        }
        
        //used to handle guards escaping
        private static void PlayerOnJumping(JumpingEventArgs ev)
        {
            if (ev.Player.Role != RoleTypeId.FacilityGuard) return; //use only for guards
            if (ev.Player.Position.x >= 121.4 && ev.Player.Position.x <= 133.3) //escape area x coordinates (should stay the same between rounds)
            {
                if (ev.Player.Position.z >= 18.6 && ev.Player.Position.z <= 28.9) //escape area z coordinates
                {
                    if (!ev.Player.TryGetSessionVariable("Documents", out int count)) //safe way to get document count
                    {
                        throw new Exception($"Could not get Documents variable from {ev.Player.Nickname}");
                    }
                    if (count == 4)
                    {
                        ev.Player.SessionVariables["Documents"] = 0;
                        ev.Player.Role.Set(RoleTypeId.NtfSpecialist,SpawnReason.Escaped,RoleSpawnFlags.All);
                        ev.Player.ShowHint("You have escaped with the documents.");
                        Respawn.GrantTickets(SpawnableTeamType.NineTailedFox,10);
                    }
                    else
                    {
                        ev.Player.ShowHint("You must have all 4 documents to escape.");
                    }
                }
            }
        }
        
    }
}