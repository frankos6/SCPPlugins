using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Respawning;
using UnityEngine;
using Player = Exiled.API.Features.Player;

namespace SCPPlugins.DocumentsPlugin
{
    public class DocumentsPlugin : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 2, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Joined += PlayerOnJoined;
            Exiled.Events.Handlers.Player.Dying += PlayerOnDying;
            Exiled.Events.Handlers.Player.Escaping += PlayerOnEscaping;
            Exiled.Events.Handlers.Player.Handcuffing += PlayerOnHandcuffing;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            CustomItem.RegisterItems();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Joined -= PlayerOnJoined;
            Exiled.Events.Handlers.Player.Dying -= PlayerOnDying;
            Exiled.Events.Handlers.Player.Escaping -= PlayerOnEscaping;
            Exiled.Events.Handlers.Player.Handcuffing -= PlayerOnHandcuffing;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            CustomItem.UnregisterItems();
            base.OnDisabled();
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        private static void ServerOnRoundStarted()
        {
            Player.List.ToList().ForEach(player =>
            {
                player.SessionVariables["Documents"] = 0;
            });
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHandcuffing"/>
        private static void PlayerOnHandcuffing(HandcuffingEventArgs ev)
        {
            if (ev.Target.Role != RoleTypeId.Scientist && ev.Target.Role != RoleTypeId.FacilityGuard) return;
            if (!ev.Target.TryGetSessionVariable("Documents", out int count))
            {
                throw new Exception($"Could not get Documents variable from {ev.Target.Nickname}");
            }
            ev.Target.SessionVariables["Documents"] = 0;
            for (var i = count; i > 0; i--) //drop all documents
            {
                if (CustomItem.TrySpawn(1u, ev.Target.Position, out var pickup))
                    Log.Debug($"Spawned Documents at {pickup?.Position ?? new Vector3()} (dropped by {ev.Target.Nickname} on being cuffed)");
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEscaping"/>
        private static void PlayerOnEscaping(EscapingEventArgs ev)
        {
            if (ev.Player.Role != RoleTypeId.Scientist && ev.Player.Role != RoleTypeId.FacilityGuard) return;
            if (!ev.Player.TryGetSessionVariable("Documents", out int count))
            {
                throw new Exception($"Could not get Documents variable from {ev.Player.Nickname}");
            }
            if (count == 4)
            {
                if (ev.Player.Role == RoleTypeId.Scientist)
                {
                    ev.Player.SessionVariables["Documents"] = 0;
                    Cassie.Message("Attention all personnel. The Foundation has secured important containment information", isSubtitles:true);
                    Player.List.ToList().ForEach(player => //change each player's class (used to end the round)
                    {
                        player.Role.Set(RoleTypeId.NtfPrivate);
                    });
                } 
                else 
                {
                    ev.Player.SessionVariables["Documents"] = 0;
                    ev.NewRole = RoleTypeId.NtfSpecialist;
                    ev.EscapeScenario = EscapeScenario.Scientist;
                    ev.IsAllowed = true;
                    Respawn.GrantTickets(SpawnableTeamType.NineTailedFox,10); //add tickets for MTF wave
                }
            }
            else
            {
                if (ev.Player.Role == RoleTypeId.Scientist)
                {
                    ev.Player.SessionVariables["Documents"] = 0;
                    for (var i = count; i > 0; i--) //drop all documents
                    {
                        if (CustomItem.TrySpawn(1u, ev.Player.Position, out var pickup))
                            Log.Debug($"Spawned Documents at {pickup?.Position ?? new Vector3()} (dropped by {ev.Player.Nickname} on escape)");
                    }
                }
                else
                {
                    ev.Player.ShowHint("You must have all 4 documents to escape.");
                }
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDying"/>
        private static void PlayerOnDying(DyingEventArgs ev)
        {
            if (ev.Player.Role != RoleTypeId.Scientist && ev.Player.Role != RoleTypeId.FacilityGuard) return;
            if (!ev.Player.TryGetSessionVariable("Documents", out int count))
            {
                throw new Exception($"Could not get Documents variable from {ev.Player.Nickname}");
            }
            ev.Player.SessionVariables["Documents"] = 0;
            for (var i = count; i > 0; i--) //drop all documents
            {
                if (CustomItem.TrySpawn(1u, ev.Player.Position, out var pickup))
                    Log.Debug($"Spawned Documents at {pickup?.Position ?? new Vector3()} (dropped by {ev.Player.Nickname} on death)");
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnJoined"/>
        private static void PlayerOnJoined(JoinedEventArgs ev)
        {
            ev.Player.SessionVariables["Documents"] = 0;
        }
        
    }
}