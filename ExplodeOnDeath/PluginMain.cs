using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplodeOnDeath
{
    public class PluginMain : Plugin<Config>
    {
        public override string Name => "ExplodeOnDeath";
        public override string Author => "VirtualBrightPlayz";
        public override Version Version => new Version(1, 0, 0);

        public PluginEvents PLEV;

        public override void OnEnabled()
        {
            base.OnEnabled();
            PLEV = new PluginEvents(this);
            Exiled.Events.Handlers.Player.Died += PLEV.PlayerDied;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Player.Died -= PLEV.PlayerDied;
            PLEV = null;
        }
    }
}
