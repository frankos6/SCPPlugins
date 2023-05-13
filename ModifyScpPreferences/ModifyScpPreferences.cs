using Exiled.API.Features;
using System;
using System.Reflection;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace SCPPlugins.ModifyScpPreferences
{
    public class ModifyScpPreferences : Plugin<Config>
    {
        public override string Author => "frankos6";
        public override Version RequiredExiledVersion => new Version(7, 0, 0, 0);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Verified += PlayerOnVerified;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= PlayerOnVerified;
            base.OnDisabled();
        }
        
        private void PlayerOnVerified(VerifiedEventArgs ev)
        {
            if (Config.IgnoredPlayers.Contains(ev.Player.UserId)) //dont handle for ignored players
            {
                Log.Debug($"Ignoring {ev.Player.Nickname}");
                return;
            }
            Log.Debug($"{ev.Player.Nickname}'s preferences before:");
            foreach (var role in ev.Player.ScpPreferences.Preferences.Keys)
            {
                Log.Debug($"{role.GetFullName()}: {ev.Player.ScpPreferences.Preferences[role]}");
            }
            if (Config.Scp049 >= -5 && Config.Scp049 <= 5)
            {
                ev.Player.ScpPreferences.Preferences[RoleTypeId.Scp049] = Config.Scp049;
            }
            if (Config.Scp079 >= -5 && Config.Scp079 <= 5)
            {
                ev.Player.ScpPreferences.Preferences[RoleTypeId.Scp079] = Config.Scp079;
            }
            if (Config.Scp096 >= -5 && Config.Scp096 <= 5)
            {
                ev.Player.ScpPreferences.Preferences[RoleTypeId.Scp096] = Config.Scp096;
            }
            if (Config.Scp106 >= -5 && Config.Scp106 <= 5)
            {
                ev.Player.ScpPreferences.Preferences[RoleTypeId.Scp106] = Config.Scp106;
            }
            if (Config.Scp173 >= -5 && Config.Scp173 <= 5)
            {
                ev.Player.ScpPreferences.Preferences[RoleTypeId.Scp173] = Config.Scp173;
            }
            if (Config.Scp939 >= -5 && Config.Scp939 <= 5)
            {
                ev.Player.ScpPreferences.Preferences[RoleTypeId.Scp939] = Config.Scp939;
            }
            Log.Debug($"{ev.Player.Nickname}'s preferences after:");
            foreach (var role in ev.Player.ScpPreferences.Preferences.Keys)
            {
                Log.Debug($"{role.GetFullName()}: {ev.Player.ScpPreferences.Preferences[role]}");
            }
        }
    }
}