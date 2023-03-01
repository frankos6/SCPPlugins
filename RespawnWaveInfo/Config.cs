using Exiled.API.Interfaces;

namespace SCPPlugins.RespawnWaveInfo
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
    }
}