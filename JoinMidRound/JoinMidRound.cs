using System;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.API.Features.Player;

namespace SCPPlugins.JoinMidRound
{
    public class JoinMidRound : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Joined += PlayerOnJoined;
            Exiled.Events.Handlers.Player.Left += PlayerOnLeft;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Joined -= PlayerOnJoined;
            Exiled.Events.Handlers.Player.Left -= PlayerOnLeft;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            base.OnDisabled();
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnJoined"/>
        private static void PlayerOnJoined(JoinedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinedMidRound"] = Round.InProgress;
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
            Server.SessionVariables["LeftPlayers"] = list.Append(ev.Player.UserId).ToArray();
        }
    }
}