using Exiled.API.Features;
using System;
using Exiled.Events.EventArgs.Scp096;

namespace SCPPlugins.NotifySCP096Targets
{
    // ReSharper disable once InconsistentNaming
    public class NotifySCP096Targets : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0, 0);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Scp096.AddingTarget += Scp096OnAddingTarget;
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Scp096.AddingTarget -= Scp096OnAddingTarget;
            base.OnDisabled();
        }
        
        /// <inheritdoc cref="Exiled.Events.Handlers.Scp096.OnAddingTarget"/>
        private static void Scp096OnAddingTarget(AddingTargetEventArgs ev)
        {
            ev.Target.ShowHint("You are targeted by SCP-096",5f);
        }
    }
}