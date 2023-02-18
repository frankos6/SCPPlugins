using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using Respawning;
using Player = Exiled.API.Features.Player;

namespace SCPPlugins.CISpies
{
    // ReSharper disable once InconsistentNaming
    public class CISpies : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Joined += PlayerOnJoined;
            Exiled.Events.Handlers.Player.Hurting += PlayerOnHurting;
            Exiled.Events.Handlers.Player.Died += PlayerOnDied;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += PlayerOnChangingSpectatedPlayer;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam += ServerOnRespawningTeam;
            Exiled.Events.Handlers.Server.EndingRound += ServerOnEndingRound;
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Joined -= PlayerOnJoined;
            Exiled.Events.Handlers.Player.Hurting -= PlayerOnHurting;
            Exiled.Events.Handlers.Player.Died -= PlayerOnDied;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer -= PlayerOnChangingSpectatedPlayer;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam -= ServerOnRespawningTeam;
            Exiled.Events.Handlers.Server.EndingRound -= ServerOnEndingRound;
            base.OnDisabled();
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRespawningTeam"/>
        private void ServerOnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency) return;
            if (Server.SessionVariables["SpyRespawned"].Equals(true)) return;
            Random rng = new Random();
            if (rng.Next(0, 100) < Config.SpyChance)
            {
                if (Config.SpawnSpyOnce) Server.SessionVariables["SpyRespawned"] = true;
                Player target = ev.Players[rng.Next(ev.Players.Count)];
                target.SessionVariables["IsSpy"] = true;
                Log.Debug($"{target.Nickname} respawned as a spy");
                target.ShowHint("You are a Chaos Insurgency spy!\n" +
                                "You will be revealed once you use the .reveal command\n" +
                                "Friendly fire against CI's is reduced",10f);
            }
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDied"/>
        private static void PlayerOnDied(DiedEventArgs ev)
        {
            ev.Player.SessionVariables["IsSpy"] = false;
            var spies = Player.List.Where(p => p.SessionVariables["IsSpy"].Equals(true) && p.IsAlive).ToArray();
            var scps = Player.List.Count(p => p.IsScp && p.IsAlive);
            var cis = Player.List.Count(p => p.LeadingTeam == LeadingTeam.ChaosInsurgency && p.IsAlive);
            var foundation = Player.List.Count(p => p.LeadingTeam == LeadingTeam.FacilityForces && p.IsAlive);
            Player spy = spies.Count() == 1 ? spies[0] : null;
            if (spy != null && scps == 0 && cis == 0 && foundation > 1)
            {
                RevealPlayer(spy);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHurting"/>
        private static void PlayerOnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null) return;
            if (!ev.Player.TryGetSessionVariable("IsSpy", out bool playerIsSpy))
            {
                //Log.Error($"Could not get IsSpy variable from {ev.Player.Nickname}");
                throw new Exception($"Could not get IsSpy variable from {ev.Attacker.Nickname}");
            }
            if (!ev.Attacker.TryGetSessionVariable("IsSpy", out bool attackerIsSpy))
            {
                //Log.Error($"Could not get IsSpy variable from {ev.Attacker.Nickname}");
                throw new Exception($"Could not get IsSpy variable from {ev.Attacker.Nickname}");
            }
            if (ev.Attacker.IsCHI && playerIsSpy && ev.Player.IsNTF)
            {
                ev.Attacker.ShowHint($"{ev.Player.Nickname} is a CI spy!",2f);
                ev.Amount *= 0.4f;
            }
            if (ev.Player.IsCHI && attackerIsSpy && ev.Attacker.IsNTF)
            {
                ev.Attacker.ShowHint("You are a CI spy, remember?",2f);
                ev.Amount *= 0.4f;
            }
            if (ev.Player.IsNTF && playerIsSpy && ev.Attacker.IsNTF && attackerIsSpy)
            {
                ev.Attacker.ShowHint($"{ev.Player.Nickname} is also a CI spy!",2f);
                ev.Amount *= 0.4f;
            }
            else if (ev.Player.IsNTF && attackerIsSpy && ev.Attacker.IsNTF)
            {
                ev.Amount *= 2.5f;
            }
            else if (ev.Attacker.IsNTF && playerIsSpy && ev.Player.IsNTF)
            {
                ev.Amount *= 2.5f;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnJoined"/>
        private static void PlayerOnJoined(JoinedEventArgs ev)
        {
            ev.Player.SessionVariables["IsSpy"] = false;
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnEndingRound"/>
        private static void ServerOnEndingRound(EndingRoundEventArgs ev)
        {
            var spies = Player.List.Where(p => p.SessionVariables["IsSpy"].Equals(true) && p.IsAlive).ToArray();
            var foundation = Player.List.Count(p => p.LeadingTeam == LeadingTeam.FacilityForces && p.IsAlive);
            Player spy = spies.Count() == 1 ? spies[0] : null;
            if (spy == null) return; //if no spy is alive dont handle the event
            if (foundation == 1) //if only spy is alive
            {
                RevealPlayer(spy);
            }
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        private static void ServerOnRoundStarted()
        {
            Server.SessionVariables["SpyRespawned"] = false;
            Player.List.ToList().ForEach(player =>
            {
                player.SessionVariables["IsSpy"] = false;
            });
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingSpectatedPlayer"/>
        private static void PlayerOnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.NewTarget == null) return;
            if (ev.NewTarget.SessionVariables["IsSpy"].Equals(true))
            {
                ev.Player.ShowHint($"{ev.NewTarget.Nickname} is a CI spy.",10f);
            }
        }

        /// <summary>
        /// Reveals a <see cref="Exiled.API.Features.Player"/> if they are a spy
        /// </summary>
        /// <param name="player">The <see cref="Exiled.API.Features.Player"/> to try to reveal</param>
        public static void RevealPlayer(Player player)
        {
            if (!Round.InProgress) return;
            if (player.SessionVariables["IsSpy"].Equals(false)) return;
            player.SessionVariables["IsSpy"] = false;
            player.Role.Set(RoleTypeId.ChaosRifleman,SpawnReason.ForceClass,RoleSpawnFlags.None);
            player.ShowHint("You have been revealed!");
        }
    }
}