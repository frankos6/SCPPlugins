using System;
using System.Reflection;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp330;
using Exiled.Events.Handlers;
using InventorySystem.Items.Usables.Scp330;

namespace SCPPlugins.PinkCandy
{
    public class PinkCandy : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "1.3.0");
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);
        private Random _rng;

        public override void OnEnabled()
        {
            Scp330.InteractingScp330 += Scp330OnInteractingScp330;
            _rng = new Random();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Scp330.InteractingScp330 += Scp330OnInteractingScp330;
            base.OnDisabled();
        }
        
        private void Scp330OnInteractingScp330(InteractingScp330EventArgs ev)
        {
            if (ev.ShouldSever) return; //dont give pink candy as 3rd candy
            if (_rng.Next(0, 100) < Config.PinkCandyChance)
            {
                ev.Candy = CandyKindID.Pink;
            }
        }
    }
}