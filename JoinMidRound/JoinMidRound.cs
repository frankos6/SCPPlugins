using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.API.Features.Player;
using MEC;
using PlayerRoles;
using Server = Exiled.API.Features.Server;

namespace SCPPlugins.JoinMidRound
{
    public class JoinMidRound : Plugin<Config>
    {
        private readonly Dictionary<Player, CoroutineHandle> _respawnCoroutines =
            new Dictionary<Player, CoroutineHandle>();

        public override string Author => "frankos6";
        public override Version RequiredExiledVersion => new Version(7, 0, 0, 0);
        public static JoinMidRound Singleton;

        public override void OnEnabled()
        {
            Singleton = this;
            Exiled.Events.Handlers.Player.Left += PlayerOnLeft;
            Exiled.Events.Handlers.Player.VoiceChatting += PlayerOnVoiceChatting;
            Exiled.Events.Handlers.Player.Verified += PlayerOnVerified;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Singleton = null;
            Exiled.Events.Handlers.Player.Left -= PlayerOnLeft;
            Exiled.Events.Handlers.Player.VoiceChatting -= PlayerOnVoiceChatting;
            Exiled.Events.Handlers.Player.Verified -= PlayerOnVerified;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            base.OnDisabled();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted" />
        private static void ServerOnRoundStarted()
        {
            Server.SessionVariables["LeftPlayers"] = new string[] { };
            Player.List.ToList().ForEach(player => { player.SessionVariables["JoinedMidRound"] = false; });
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnLeft" />
        private static void PlayerOnLeft(LeftEventArgs ev)
        {
            if (!Round.InProgress) return;
            if (!Server.TryGetSessionVariable("LeftPlayers", out string[] list))
                throw new Exception("Could not get LeftPlayers from session variables");
            Server.SessionVariables["LeftPlayers"] =
                list.Append(ev.Player.UserId).ToArray(); //anti-abuse system
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnVoiceChatting" />
        private void PlayerOnVoiceChatting(VoiceChattingEventArgs ev)
        {
            if (ev.Player == null) return;
            if (!_respawnCoroutines.TryGetValue(ev.Player, out var handle)) return;
            Timing.KillCoroutines(handle);
            ev.Player.ShowHint("Respawning was cancelled.\n" +
                               "You can still use the .respawn command to respawn", 5f);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnVerified" />
        private void PlayerOnVerified(VerifiedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinedMidRound"] = Round.InProgress;
            //this event is used to ensure that the player will see the hints
            if (!Round.InProgress) return;
            if (!Server.TryGetSessionVariable("LeftPlayers", out string[] list))
                throw new Exception("Could not get LeftPlayers from session variables");
            if (list.Contains(ev.Player.UserId)) return;
            if (Round.ElapsedTime.TotalSeconds > Config.LateJoinTime) return;
            _respawnCoroutines.Add(ev.Player, Timing.RunCoroutine(RespawnCoroutine(ev.Player)));
        }

        /// <summary>
        ///     Respawns a player after a delay, cancellable with voice chat key
        /// </summary>
        /// <param name="player">The player to respawn</param>
        private IEnumerator<float> RespawnCoroutine(Player player)
        {
            for (var i = Config.RespawnTimer; i > 0; i--)
            {
                player.ShowHint($"You will be respawned as Class D in {i} seconds!\n" +
                                "Use the voice chat key to cancel", 1f);
                yield return Timing.WaitForSeconds(1f);
            }

            player.Role.Set(RoleTypeId.ClassD, SpawnReason.LateJoin, RoleSpawnFlags.All);
            _respawnCoroutines.Remove(player);
        }
    }
}