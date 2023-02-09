using System;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;

namespace SCPPlugins.CISpies
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class RevealCommand : ICommand
    {
        public string Command => "reveal";
        public string[] Aliases { get; } = { };
        public string Description => "Reveal that you are a spy";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Round.InProgress)
            {
                response = "The round must be in progress.";
                return false;
            }
            Player player = Player.Get(sender);
            if (player.SessionVariables["IsSpy"].Equals(false))
            {
                response = "You are not a spy.";
                return false;
            }
            if (player.IsCHI && player.SessionVariables["IsSpy"].Equals(true))
            {
                response = "You have already been revealed.";
                return false;
            }
            if (player.IsNTF && player.SessionVariables["IsSpy"].Equals(true))
            {
                player.Role.Set(RoleTypeId.ChaosRifleman,SpawnReason.ForceClass,RoleSpawnFlags.None);
                response = "You have been revealed.";
                player.ShowHint("You have been revealed!");
                return true;
            }
            response = "xd";
            return false;
        }
    }
}