using System.ComponentModel;
using Exiled.API.Interfaces;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace SCPPlugins.ModifyScpPreferences
{
    public class Config : IConfig
    {
        [Description("SCP-049 preference setting\n" +
                     "  # Should be between -5 and 5, if not will default to players' custom preferences")]
        public int Scp049 { get; set; }

        [Description("SCP-079 preference setting")]
        public int Scp079 { get; set; }

        [Description("SCP-096 preference setting")]
        public int Scp096 { get; set; }

        [Description("SCP-106 preference setting")]
        public int Scp106 { get; set; }

        [Description("SCP-173 preference setting")]
        public int Scp173 { get; set; }

        [Description("SCP-939 preference setting")]
        public int Scp939 { get; set; }

        [Description("List of players who ignore this plugin\n" +
                     "  # UserID format is SteamId64Here@steam, DiscordUserIDHere@discord, etc...")]
        public string[] IgnoredPlayers { get; set; } = { "SomeSteamId64@steam" };

        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
    }
}