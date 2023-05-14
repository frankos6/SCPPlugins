using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SCPPlugins.JoinMidRound
{
    public class Config : IConfig
    {
        [Description("Sets the auto-respawn timer (in seconds)")]
        public int RespawnTimer { get; set; } = 7;
        [Description("Sets the maximum round time for late join (in seconds)")] 
        public int LateJoinTime { get; set; } = 120;

        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
    }
}