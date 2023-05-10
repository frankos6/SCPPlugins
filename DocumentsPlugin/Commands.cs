using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;

namespace SCPPlugins.DocumentsPlugin
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DocumentsCommand : ICommand
    {
        public string Command => "documents";
        public string[] Aliases { get; } = { "docs" };
        public string Description => "Gets amount of collected documents.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Round.InProgress)
            {
                response = "The round must be in progress.";
                return false;
            }
            Player player = Player.Get(sender);
            if (!player.TryGetSessionVariable("Documents", out int count))
            {
                throw new Exception($"Could not get Documents variable from {player.Nickname}");
            }
            response = $"You have collected {count}/4 documents.";
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DropDocumentsCommand : ICommand
    {
        public string Command => "dropdocuments";
        public string[] Aliases { get; } = { "dropdocs" };
        public string Description => "Drops specified amount of documents.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Round.InProgress)
            {
                response = "The round must be in progress.";
                return false;
            }
            Player player = Player.Get(sender);
            if (!player.TryGetSessionVariable("Documents", out int count))
            {
                throw new Exception($"Could not get Documents variable from {player.Nickname}");
            }
            int arg = 0;
            bool result;
            try
            {
                result = int.TryParse(arguments.FirstElement(), out arg);
            }
            catch (IndexOutOfRangeException)
            {
                result = false;
            }
            if (!result)
            {
                player.SessionVariables["Documents"] = 0;
                for (int i = count; i > 0; i--)
                {
                    CustomItem.TrySpawn((uint)1, player.Position, out Pickup pickup);
                    Log.Debug($"Spawned Documents at {pickup.Position} (dropped by {player.Nickname})");
                }
                response = $"Dropped {count} documents.";
                return true;
            }
            if (arg <= 0)
            {
                response = "Argument must be bigger that 0.";
                return false;
            }
            else
            {
                if (arg >= count) arg = count;
                player.SessionVariables["Documents"] = count - arg;
                for (int i = arg; i > 0; i--) { 
                    CustomItem.TrySpawn((uint)1, player.Position, out Pickup pickup); 
                    Log.Debug($"Spawned Documents at {pickup.Position} (dropped by {player.Nickname})");
                }
                response = $"Dropped {arg} documents.";
                return true;
            }
        }
    }
}