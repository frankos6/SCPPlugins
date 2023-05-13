using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using UnityEngine;

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

            var player = Player.Get(sender);
            if (!player.TryGetSessionVariable("Documents", out int count))
                throw new Exception($"Could not get Documents variable from {player.Nickname}");
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

            var player = Player.Get(sender);
            if (!player.TryGetSessionVariable("Documents", out int count))
                throw new Exception($"Could not get Documents variable from {player.Nickname}");
            var arg = 0;
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
                for (var i = count; i > 0; i--)
                    if (CustomItem.TrySpawn(1u, player.Position, out var pickup))
                        Log.Debug(
                            $"Spawned Documents at {pickup?.Position ?? new Vector3()} (dropped by {player.Nickname})");
                response = $"Dropped {count} documents.";
                return true;
            }

            if (arg <= 0)
            {
                response = "Argument must be bigger that 0.";
                return false;
            }

            if (arg >= count) arg = count;
            player.SessionVariables["Documents"] = count - arg;
            for (var i = arg; i > 0; i--)
                if (CustomItem.TrySpawn(1u, player.Position, out var pickup))
                    Log.Debug(
                        $"Spawned Documents at {pickup?.Position ?? new Vector3()} (dropped by {player.Nickname})");
            response = $"Dropped {arg} documents.";
            return true;
        }
    }
}