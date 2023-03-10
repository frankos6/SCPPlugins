using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SCPPlugins.PinkCandy
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
        [Description("Sets the chance to get pink candy from SCP-330 (in percent)")] 
        public int PinkCandyChance { get; set; } = 15;
    }
}