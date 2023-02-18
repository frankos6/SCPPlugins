﻿using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SCPPlugins.MrugaczNerf
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
        [Description("The increased Blink duration (in seconds)")]
        public float BlinkCooldown { get; set; } = 5;
    }
}