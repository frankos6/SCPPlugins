using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MEC;
using PlayerRoles;
using Respawning;
using Server = Exiled.Events.Handlers.Server;

namespace SCPPlugins.RespawnWaveInfo
{
    public class RespawnWaveInfo : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version RequiredExiledVersion => new Version(7, 0, 0, 0);

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
                string message;
                if (Round.IsEnded) yield break;
                var spectators = Player.List.Where(p => p.Role == RoleTypeId.Spectator).ToArray();
                var wavetime = Respawn.TimeUntilSpawnWave;
                var team = Respawn.NextKnownTeam == SpawnableTeamType.ChaosInsurgency
                    ? "Chaos Insurgency" : "Nine-Tailed Fox";
                if (Respawn.IsSpawning)
                {
                    message = "Time until next wave: Spawning!\n" +
                              $"Next team: {team}";
                }
                else
                {
                    message = $"Time until next wave: {wavetime.Minutes}:{wavetime.Seconds:D2}\n" +
                              "Next team: ███████";
                }
                foreach (var spec in spectators)
                {
                    spec.Broadcast(1,message);
                }
                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}