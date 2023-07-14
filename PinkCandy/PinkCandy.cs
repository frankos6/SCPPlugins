using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp330;
using Exiled.Events.Handlers;
using InventorySystem.Items.Usables.Scp330;
using Random = UnityEngine.Random;

namespace SCPPlugins.PinkCandy
{
    public class PinkCandy : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version RequiredExiledVersion => new Version(7, 2, 0, 0);

        public override void OnEnabled()
        {
            Scp330.InteractingScp330 += Scp330OnInteractingScp330;
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
            if (Random.Range(0f,100f) <= Config.PinkCandyChance) ev.Candy = CandyKindID.Pink;
        }
    }
}