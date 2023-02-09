using Exiled.API.Interfaces;

namespace SCPPlugins.JoinMidRound
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
    }
}