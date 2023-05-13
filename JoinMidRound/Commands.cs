using System;
using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;

namespace SCPPlugins.JoinMidRound
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class RespawnCommand : ICommand
    {
        public string Command => "respawn";
        public string[] Aliases { get; } = { };

        public string Description => "Allows the player to respawn as Class D if he joined after the round started.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Round.InProgress)
            {
                response = "The round must be in progress.";
                return false;
            }

            var player = Player.Get(sender);
            if (!Server.TryGetSessionVariable("LeftPlayers", out string[] list))
                throw new Exception("Could not get LeftPlayers from session variables");
            if (list.Contains(player.UserId)) //anti-abuse system
            {
                response = "You already died this round, didn't you?";
                return false;
            }

            if (Round.ElapsedTime.TotalSeconds <= 120 &&
                player.SessionVariables["JoinedMidRound"].Equals(true) && player.Role == RoleTypeId.Spectator)
            {
                player.SessionVariables["JoinedMidRound"] = false;
                player.Role.Set(RoleTypeId.ClassD);
                response = "Respawning as Class D...";
                return true;
            }

            if (Round.ElapsedTime.TotalSeconds > 120)
            {
                response = "120 second grace period has expired.";
                return false;
            }

            if (player.SessionVariables["JoinedMidRound"].Equals(false))
            {
                response = "You did not join mid-round.";
                return false;
            }

            if (player.IsAlive)
            {
                response = "You must be dead to respawn.";
                return false;
            }

            response = "xd";
            return false;
        }
    }
}