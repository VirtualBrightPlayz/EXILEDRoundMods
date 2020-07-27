using System;

namespace ItemRandomizer
{
    public class PluginEvents
    {
        public PluginMain plugin;
        public PluginEvents(PluginMain main)
        {
            plugin = main;
        }

        public void RoundStart()
        {
            plugin.Randomize();
        }
    }
}