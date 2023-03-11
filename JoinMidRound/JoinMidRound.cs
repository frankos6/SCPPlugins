using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.API.Features.Player;
using MEC;
using PlayerRoles;

namespace SCPPlugins.JoinMidRound
{
    public class JoinMidRound : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 2, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);

        private Dictionary<Player, CoroutineHandle> _respawnCoroutines = new Dictionary<Player, CoroutineHandle>();

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Left += PlayerOnLeft;
            Exiled.Events.Handlers.Player.VoiceChatting += PlayerOnVoiceChatting;
            Exiled.Events.Handlers.Player.Verified += PlayerOnVerified;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Left -= PlayerOnLeft;
            Exiled.Events.Handlers.Player.VoiceChatting -= PlayerOnVoiceChatting;
            Exiled.Events.Handlers.Player.Verified -= PlayerOnVerified;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            base.OnDisabled();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        private static void ServerOnRoundStarted()
        {
            Server.SessionVariables["LeftPlayers"] = new string[] { };
            Player.List.ToList().ForEach(player =>
            {
                player.SessionVariables["JoinedMidRound"] = false;
            });
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnLeft"/>
        private static void PlayerOnLeft(LeftEventArgs ev)
        {
            if (!Round.InProgress) return;
            if (!Server.TryGetSessionVariable("LeftPlayers", out string[] list))
            {
                throw new Exception("Could not get LeftPlayers from session variables");
            }
            Server.SessionVariables["LeftPlayers"] = list.Append(ev.Player.UserId).ToArray(); //anti-abuse system
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnVoiceChatting"/>
        private void PlayerOnVoiceChatting(VoiceChattingEventArgs ev)
        {
            if (_respawnCoroutines.ContainsKey(ev.Player))
            {
                Timing.KillCoroutines(_respawnCoroutines[ev.Player]);
                ev.Player.ShowHint("Respawning was cancelled.\n" +
                                   "You can still use the .respawn command to respawn",5f);
            }
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnVerified"/>
        private void PlayerOnVerified(VerifiedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinedMidRound"] = Round.InProgress;
            //this event is used to ensure that the player will see the hints
            if (!Round.InProgress) return;
            if (!Server.TryGetSessionVariable("LeftPlayers", out string[] list))
            {
                throw new Exception("Could not get LeftPlayers from session variables");
            }
            if (list.Contains(ev.Player.UserId)) return;
            if (Round.ElapsedTime.TotalSeconds > 120) return;
            _respawnCoroutines.Add(ev.Player,Timing.RunCoroutine(RespawnCoroutine(ev.Player)));
        }
        
        /// <summary>
        /// Respawns a player after a delay, cancellable with voice chat key
        /// </summary>
        /// <param name="player">The player to respawn</param>
        private IEnumerator<float> RespawnCoroutine(Player player)
        {
            for (int i = Config.RespawnTimer; i > 0; i--)
            {
                player.ShowHint($"You will be respawned as Class D in {i} seconds!\n" +
                                "Use the voice chat key to cancel",1f);
                yield return Timing.WaitForSeconds(1f);
            }
            player.Role.Set(RoleTypeId.ClassD,SpawnReason.LateJoin,RoleSpawnFlags.All);
            _respawnCoroutines.Remove(player);
        }
    }
}