using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInfection
{
    public class PluginMain : Plugin<Config>
    {
        public override string Name => "ClassInfection";
        public override string Author => "VirtualBrightPlayz";
        public override Version Version => new Version(1, 0, 0);

        public PluginEvents PLEV;

        public override void OnEnabled()
        {
            base.OnEnabled();
            PLEV = new PluginEvents(this);
            Exiled.Events.Handlers.Player.Dying += PLEV.Dying;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Player.Dying -= PLEV.Dying;
            PLEV = null;
        }
    }
}
