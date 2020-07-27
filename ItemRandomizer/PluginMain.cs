using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ItemRandomizer
{
    public class PluginMain : Plugin<Config>
    {
        public override string Name => "ItemRandomizer";
        public override string Author => "VirtualBrightPlayz";
        public override Version Version => new Version(1, 0, 0);

        public PluginEvents PLEV;
        public static PluginMain main;

        public override void OnEnabled()
        {
            base.OnEnabled();
            PLEV = new PluginEvents(this);
            Exiled.Events.Handlers.Server.RoundStarted += PLEV.RoundStart;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Server.RoundStarted -= PLEV.RoundStart;
            PLEV = null;
        }

        public void Randomize()
        {
            Pickup[] pcks = new Pickup[0];
            Pickup.Instances.CopyTo(pcks);
            foreach (var pck in pcks)
            {
                if (!Config.ExcludeItems.Contains(pck.info.itemId))
                {
                    pck.Delete();
                }
            }
            foreach (var room in Map.Rooms)
            {
            }
        }
    }
}
