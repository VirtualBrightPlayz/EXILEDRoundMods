using Exiled.API.Extensions;
using Exiled.Events.EventArgs;
using System;

namespace ClassInfection
{
    public class PluginEvents
    {
        private PluginMain plugin;

        public PluginEvents(PluginMain main)
        {
            plugin = main;
        }

        internal void Dying(DyingEventArgs ev)
        {
            if (ev.Killer == null || ev.Killer == ev.Target || ev.Killer.Role.GetSide() == ev.Target.Role.GetSide())
                return;
            ev.IsAllowed = false;
            bool isSCP = ev.Killer.Role.GetSide() == Exiled.API.Enums.Side.Scp;
            ev.Target.SetRole(ev.Killer.Role, true, false);
            ev.Target.Position = ev.Killer.Position;
            if (isSCP)
                ev.Target.Health *= plugin.Config.SCPHealthMultiply;
        }
    }
}