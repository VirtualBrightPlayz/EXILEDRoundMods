using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Grenades;
using Mirror;
using System;
using System.Linq;
using UnityEngine;

namespace ExplodeOnDeath
{
    public class PluginEvents
    {
        private PluginMain plugin;

        public PluginEvents(PluginMain pluginMain)
        {
            this.plugin = pluginMain;
        }

        public void PlayerDied(DiedEventArgs ev)
        {
            Grenade grenade = GameObject.Instantiate(ev.Target.ReferenceHub.GetComponent<GrenadeManager>().availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFrag).grenadeInstance).GetComponent<Grenade>();
            grenade.fuseDuration = plugin.Config.Delay + 0.1f;
            grenade.throwerTeam = Team.TUT;
            grenade.NetworkthrowerTeam = Team.TUT;
            grenade.InitData(ev.Target.ReferenceHub.GetComponent<GrenadeManager>(), Vector3.zero, Vector3.zero);
            grenade.throwerTeam = Team.TUT;
            grenade.NetworkthrowerTeam = Team.TUT;
            grenade.transform.position = ev.Target.GameObject.transform.position;
            NetworkServer.Spawn(grenade.gameObject);
        }
    }
}