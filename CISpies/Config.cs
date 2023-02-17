using Exiled.API.Interfaces;

namespace SCPPlugins.CISpies
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        public int SpyChance { get; set; } = 25;
        public bool SpawnSpyOnce { get; set; } = true;
    }
}