using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using Respawning;
using Server = Exiled.Events.Handlers.Server;

namespace SCPPlugins.RespawnWaveInfo
{
    public class RespawnWaveInfo : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 1, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);

        public override void OnEnabled()
        {
            Server.RoundStarted += ServerOnRoundStarted;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Server.RoundStarted -= ServerOnRoundStarted;
            base.OnDisabled();
        }

        private static void ServerOnRoundStarted()
        {
            Timing.RunCoroutine(NotifyCoroutine());
        }
        
        private static IEnumerator<float> NotifyCoroutine()
        {
            while (true)
            {
                if (Round.IsEnded) yield break;
                var spectators = Player.List.Where(p => p.Role == RoleTypeId.Spectator).ToList();
                var wavetime = Respawn.TimeUntilSpawnWave;
                var nextteam = Respawn.NextKnownTeam == SpawnableTeamType.ChaosInsurgency;
                string team = nextteam ? "Chaos Insurgency" : "Nine-Tailed Fox";
                foreach (var spec in spectators)
                {
                    spec.Broadcast(1,$"Time until next wave: {wavetime.Minutes}:{wavetime.Seconds:D2}\n" +
                                     $"Next team: {team}");
                }
                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}