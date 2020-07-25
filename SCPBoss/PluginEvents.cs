using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using System;
using System.Collections.Generic;

namespace SCPBoss
{
    public class PluginEvents
    {
        private PluginMain plugin;
        private bool hasStarted = false;
        private bool bossSpawned = false;

        public PluginEvents(PluginMain main)
        {
            plugin = main;
        }

        internal void PlayerSpawn(SpawningEventArgs ev)
        {
            if (ev.RoleType.GetSide() == Exiled.API.Enums.Side.Scp && bossSpawned && !hasStarted)
            {
                ev.Player.SetRole(plugin.Config.scpReplaceType);
                return;
            }
            if (ev.RoleType.GetSide() == Exiled.API.Enums.Side.Scp)
            {
                bossSpawned = true;
                Timing.RunCoroutine(SpawnLate(ev.Player));
            }
        }

        private IEnumerator<float> SpawnLate(Player player)
        {
            yield return Timing.WaitForSeconds(5f);
            float hp = player.Health;
            player.Health = hp * plugin.Config.BaseHPMultiply;
            player.MaxHealth = (int)(hp * plugin.Config.BaseHPMultiply);
        }

        internal void RoundStarted()
        {
            hasStarted = true;
        }

        internal void WaitingForPlayers()
        {
            hasStarted = false;
            bossSpawned = false;
        }
    }
}