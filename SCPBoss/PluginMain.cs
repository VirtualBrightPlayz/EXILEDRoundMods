using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPBoss
{
    public class PluginMain : Plugin<Config>
    {
        public override string Name => "SCPBoss";
        public override string Author => "VirtualBrightPlayz";
        public override Version Version => new Version(1, 0, 0);

        public PluginEvents PLEV;

        public override void OnEnabled()
        {
            base.OnEnabled();
            PLEV = new PluginEvents(this);
            Exiled.Events.Handlers.Player.Spawning += PLEV.PlayerSpawn;
            Exiled.Events.Handlers.Server.RoundStarted += PLEV.RoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers += PLEV.WaitingForPlayers;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Player.Spawning -= PLEV.PlayerSpawn;
            Exiled.Events.Handlers.Server.RoundStarted -= PLEV.RoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= PLEV.WaitingForPlayers;
            PLEV = null;
        }
    }
}
