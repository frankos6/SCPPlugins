using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Utils.NonAllocLINQ;
using Player = Exiled.Events.Handlers.Player;

namespace SCPPlugins.JoinMidRound
{
    public class JoinMidRound : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);

        public override void OnEnabled()
        {
            Player.Joined += PlayerOnJoined;
            Exiled.Events.Handlers.Server.RoundStarted += ServerOnRoundStarted;
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Player.Joined -= PlayerOnJoined;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerOnRoundStarted;
            base.OnDisabled();
        }
        
        private static void PlayerOnJoined(JoinedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinedMidRound"] = Round.InProgress;
        }
        
        private static void ServerOnRoundStarted()
        {
            Exiled.API.Features.Player.Dictionary.ForEachValue(player =>
            {
                player.SessionVariables["JoinedMidRound"] = false;
            });
        }
        
    }
}