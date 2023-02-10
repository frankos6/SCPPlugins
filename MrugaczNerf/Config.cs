using Exiled.API.Interfaces;

namespace SCPPlugins.MrugaczNerf
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
        public float BlinkCooldown { get; set; } = 5;
    }
}