using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SCPPlugins.JoinMidRound
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        [Description("Sets the auto-respawn timer (in seconds)")] 
        public int RespawnTimer { get; set; } = 7;
    }
}