using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using Respawning;
using Player = Exiled.API.Features.Player;
using MEC;
using System.Collections.Generic;
using SCPPlugins.CISpies.Enums;
using Random = System.Random;

namespace SCPPlugins.CISpies
{
    // ReSharper disable once InconsistentNaming
    public class CISpies : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 2, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);
        private Random _rng;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Joined += PlayerOnJoined;
            Exiled.Events.Handlers.Player.Hurting += PlayerOnHurting;
            Exiled.Events.Handlers.Player.Died += PlayerOnDied;
            Exiled.Events.Handlers.Player.Escaping += PlayerOnEscaping;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += PlayerOnChangingSpectatedPlayer;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam += ServerOnRespawningTeam;
            Exiled.Events.Handlers.Server.EndingRound += ServerOnEndingRound;
            _rng = new Random();
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Joined -= PlayerOnJoined;
            Exiled.Events.Handlers.Player.Hurting -= PlayerOnHurting;
            Exiled.Events.Handlers.Player.Died -= PlayerOnDied;
            Exiled.Events.Handlers.Player.Escaping -= PlayerOnEscaping;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer -= PlayerOnChangingSpectatedPlayer;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam -= ServerOnRespawningTeam;
            Exiled.Events.Handlers.Server.EndingRound -= ServerOnEndingRound;
            base.OnDisabled();
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRespawningTeam"/>
        private void ServerOnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency) return; //only handle MTF spawns
            if (Server.SessionVariables["SpyRespawned"].Equals(true)) return; //check if a spy spawned this round
            var chance = Config.SpyChance;
            Log.Debug($"Base chance: {chance}");
            Log.Debug($"Modifier type: {Config.ModifierType}");
            switch (Config.ModifierType)
            {
                case ModifierType.PerPlayerOnline:
                    chance += Server.PlayerCount * Config.ChanceModifier;
                    Log.Debug($"Modified chance: {chance}");
                    break;
                case ModifierType.PerPlayerRespawning:
                    chance += ev.Players.Count * Config.ChanceModifier;
                    Log.Debug($"Modified chance: {chance}");
                    break;
            }
            if (_rng.Next(0, 100) < chance)
            {
                if (Config.SpawnSpyOnce) Server.SessionVariables["SpyRespawned"] = true; //prevent another spy from spawning
                var target = ev.Players[_rng.Next(ev.Players.Count)]; //choose a random spawned MTF
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
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHurting"/>
        private static void PlayerOnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null) return; //prevents errors in console
            if (ev.Attacker == ev.Player) return; //dont handle self-harm
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
            if (ev.Attacker.IsCHI && playerIsSpy && ev.Player.IsNTF) //CI shooting a spy
            {
                ev.Attacker.ShowHint($"{ev.Player.Nickname} is a CI spy!",2f);
                ev.Amount *= 0.4f;
            }
            if (ev.Player.IsCHI && attackerIsSpy && ev.Attacker.IsNTF) //spy shooting a CI
            {
                ev.Attacker.ShowHint("You are a CI spy, remember?",2f);
                ev.Amount *= 0.4f;
            }
            if (ev.Player.IsNTF && playerIsSpy && ev.Attacker.IsNTF && attackerIsSpy) //spy shooting a spy
            {
                ev.Attacker.ShowHint($"{ev.Player.Nickname} is also a CI spy!",2f);
            }
            else if (ev.Player.IsNTF && attackerIsSpy && ev.Attacker.IsNTF) //spy shooting a MTF
            {
                ev.Amount *= 2.5f;
            }
            else if (ev.Attacker.IsNTF && playerIsSpy && ev.Player.IsNTF) //MTF shooting a spy
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
            //don't end round if any spies are alive with only MTF's
            if (Player.List.Any(x => x.SessionVariables["IsSpy"].Equals(true)) && ev.LeadingTeam == LeadingTeam.FacilityForces)
            {
                ev.IsRoundEnded = false;
            }
            //assign the right team the win if only spies are alive
            if (!Player.List.Any(x => x.IsNTF && x.SessionVariables["IsSpy"].Equals(false)))
            {
                ev.LeadingTeam = Round.EscapedDClasses == 0 && Player.List.Any(x=>x.IsScp) ? LeadingTeam.Anomalies : LeadingTeam.ChaosInsurgency;
                ev.IsRoundEnded = Player.List.All(x=>x.Role != RoleTypeId.ClassD && x.Role != RoleTypeId.Scientist); //dont end round if d/scientists are alive
            }
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        private static void ServerOnRoundStarted()
        {
            Timing.RunCoroutine(NotifySpiesCoroutine());
            Server.SessionVariables["SpyRespawned"] = false;
            Player.List.ToList().ForEach(player =>
            {
                player.SessionVariables["IsSpy"] = false;
            });
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingSpectatedPlayer"/>
        private static void PlayerOnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.NewTarget == null) return; //prevents errors
            if (ev.NewTarget.SessionVariables["IsSpy"].Equals(true))
            {
                ev.Player.ShowHint($"{ev.NewTarget.Nickname} is a CI spy.",10f); //text shown to spectator
            }
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEscaping"/>
        private void PlayerOnEscaping(EscapingEventArgs ev)
        {
            if (ev.EscapeScenario != EscapeScenario.CuffedClassD && !ev.IsAllowed && !Config.ClassDSpies) return;
            if (_rng.Next(0, 100) < Config.ClassDSpyChance)
            {
                ev.Player.SessionVariables["IsSpy"] = true;
                Log.Debug($"{ev.Player.Nickname} escaped as a spy");
                ev.Player.ShowHint("You are a Chaos Insurgency spy!\n" +
                                "Yes, this is intended, so act normal\n" +
                                "Friendly fire against CI's is reduced",10f);
            }
        }

        /// <summary>
        /// Reveals a <see cref="Exiled.API.Features.Player"/> if they are a spy
        /// </summary>
        /// <param name="player">The <see cref="Exiled.API.Features.Player"/> to try to reveal</param>
        public static void RevealPlayer(Player player)
        {
            if (!Round.InProgress) return; //prevents errors
            if (player.SessionVariables["IsSpy"].Equals(false)) return; //only handle spies
            player.SessionVariables["IsSpy"] = false;
            player.Role.Set(RoleTypeId.ChaosRifleman,SpawnReason.ForceClass,RoleSpawnFlags.None); //Change class to CI Rifleman, keep inventory, don't use default spawnpoint
            player.ShowHint("You have been revealed!");
        }

        /// <summary>
        /// Periodically notifies any spy <see cref="Exiled.API.Features.Player"/> that they are a spy
        /// </summary>
        private static IEnumerator<float> NotifySpiesCoroutine()
        {
            while (true)
            {
                if (Round.IsEnded) yield break;
                var spies = Player.List.Where(p => p.SessionVariables["IsSpy"].Equals(true) && p.IsAlive);
                foreach (var spy in spies)
                {
                    spy.ShowHint("Remember that you are a CI spy!",5f);
                }
                yield return Timing.WaitForSeconds(30f);
            }
        }
    }
}