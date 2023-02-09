using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Respawning;
using Utils.NonAllocLINQ;
using Player = Exiled.Events.Handlers.Player;

namespace SCPPlugins.CISpies
{
    public class CISpies : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);

        public override void OnEnabled()
        {
            Player.Joined += PlayerOnJoined;
            Player.Hurting += PlayerOnHurting;
            Player.Dying += PlayerOnDying;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam += ServerOnRespawningTeam;
            Exiled.Events.Handlers.Server.EndingRound += ServerOnEndingRound;
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Player.Joined -= PlayerOnJoined;
            Player.Hurting -= PlayerOnHurting;
            Player.Dying -= PlayerOnDying;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam -= ServerOnRespawningTeam;
            Exiled.Events.Handlers.Server.EndingRound -= ServerOnEndingRound;
            base.OnDisabled();
        }
        
        private void ServerOnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency) return;
            Random rng = new Random();
            if (rng.Next(0, 100) < Config.SpyChance)
            {
                Exiled.API.Features.Player target = ev.Players[rng.Next(ev.Players.Count)];
                target.SessionVariables["IsSpy"] = true;
                target.ShowHint("You are a Chaos Insurgency spy!\n" +
                                "You will be revealed once you use the .reveal command\n" +
                                "Friendly fire against CI's is reduced",10f);
            }
        }

        private static void PlayerOnDying(DyingEventArgs ev)
        {
            ev.Player.SessionVariables["IsSpy"] = false;
        }

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
                ev.DamageHandler.IsFriendlyFire = true;
            }
            if (ev.Player.IsCHI && attackerIsSpy && ev.Attacker.IsNTF)
            {
                ev.Attacker.ShowHint("You are a CI spy, remember?",2f);
                ev.DamageHandler.IsFriendlyFire = true;
            }
            if (ev.Player.IsNTF && playerIsSpy && ev.Attacker.IsNTF && attackerIsSpy)
            {
                ev.Attacker.ShowHint($"{ev.Player} is also a CI spy!",2f);
                ev.DamageHandler.IsFriendlyFire = true;
            }
            else if (ev.Player.IsNTF && attackerIsSpy && ev.Attacker.IsNTF)
            {
                ev.DamageHandler.IsFriendlyFire = false;
            }
            else if (ev.Attacker.IsNTF && playerIsSpy && ev.Player.IsNTF)
            {
                ev.DamageHandler.IsFriendlyFire = false;
            }
        }

        private static void PlayerOnJoined(JoinedEventArgs ev)
        {
            ev.Player.SessionVariables["IsSpy"] = false;
        }
        
        private static void ServerOnEndingRound(EndingRoundEventArgs ev)
        {
            if (ev.LeadingTeam != LeadingTeam.FacilityForces) return;
            //if any spy is alive stop round from ending
            if (Exiled.API.Features.Player.Dictionary.Any(pair => pair.Value.SessionVariables["IsSpy"].Equals(true)))
            {
                ev.IsAllowed = false;
            }
            //if all alive players are spies end round with CI winning
            var alivePlayers = Exiled.API.Features.Player.Dictionary.Where(pair => pair.Value.IsAlive);
            if (alivePlayers.All(pair => pair.Value.SessionVariables["IsSpy"].Equals(true)))
            {
                ev.LeadingTeam = LeadingTeam.ChaosInsurgency;
                ev.IsAllowed = true;
            }
        }
        
        private static void ServerOnRoundStarted()
        {
            Exiled.API.Features.Player.Dictionary.ForEachValue(player =>
            {
                player.SessionVariables["IsSpy"] = false;
            });
        }
    }
}