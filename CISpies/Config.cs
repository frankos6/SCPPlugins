using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SCPPlugins.CISpies
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        [Description("Chance to respawn a spy in a MTF wave (in percent)")]
        public int SpyChance { get; set; } = 25;
        [Description("Specifies if a spy should spawn only once per round")]
        public bool SpawnSpyOnce { get; set; } = true;
    }
}