using System.ComponentModel;
using Exiled.API.Interfaces;
using SCPPlugins.CISpies.Enums;

namespace SCPPlugins.CISpies
{
    public class Config : IConfig
    {
        [Description("Chance to respawn a spy in a MTF wave (in percent)")]
        public float SpyChance { get; set; } = 25;

        [Description("Specifies if a spy should spawn only once per round")]
        public bool SpawnSpyOnce { get; set; } = true;

        [Description("Value added to spy chance depending on modifier type")]
        public float ChanceModifier { get; set; } = 1.5f;

        [Description("Modifier type - PerPlayerOnline, PerPlayerRespawning, Disabled")]
        public ModifierType ModifierType { get; set; } = ModifierType.Disabled;

        [Description("Make cuffed Class D escape as spies")]
        public bool ClassDSpies { get; set; } = false;

        [Description("Chance for cuffed Class D to escape as a spy")]
        public float ClassDSpyChance { get; set; } = 5f;

        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
    }
}