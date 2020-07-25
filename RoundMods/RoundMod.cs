using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grenades;
using MEC;
using Mirror;
using UnityEngine;
using HarmonyLib;
using System.IO;
using YamlDotNet.Serialization;
using Exiled.API.Features;

namespace RoundMods
{
    public class RoundMod : Plugin<RMConfig>
    {
        public override string Name => "RoundMods";
        public override string Author => "VirtualBrightPlayz";
        public override System.Version Version => new System.Version(1, 4, 0);

        internal PluginEvents PLEV;
        public static RoundMod instance = null;

        public ModType curMod = ModType.NONE;

        public List<ModType> enabledTypes = new List<ModType>();

        /*public List<ModType> allowedTypes = new List<ModType>();
        public List<ModType> enabledTypes = new List<ModType>(); // enabled but not used


        public List<RoleType> allowedrngRoles = new List<RoleType>();
        public List<RoleType> allowedrngRolesBoss = new List<RoleType>();
        public List<ItemType> allowedRngWeapons = new List<ItemType>();
        public List<ItemType> allowedRngMeds = new List<ItemType>();
        public List<RoleType> noInfectRoles = new List<RoleType>();
        public List<ItemType> notRandomizeItems = new List<ItemType>();

        public float bossHpMulti = 2f;

        public Dictionary<ModType, string> translations = new Dictionary<ModType, string>();
        public Dictionary<string, string> translations2 = new Dictionary<string, string>();


        public int maxMods;
        public static string pluginDir;*/

        public List<ModType> GetAllowedMods()
        {
            List<ModType> list = new List<ModType>();
            foreach (var item in Config.mods)
            {
                if (item.allow && !list.Contains(item.mod))
                    list.Add(item.mod);
            }
            return list;
        }

        public List<RoleType> AllowedRngRolesSameSCP()
        {
            List<RoleType> list = new List<RoleType>();
            foreach (var item in Config.samescps)
            {
                list.Add((RoleType)item);
            }
            return list;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Server.RoundStarted -= PLEV.RoundStart;
            Exiled.Events.Handlers.Server.RestartingRound -= PLEV.RoundRestart;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= PLEV.WaitForPlayers;
            Exiled.Events.Handlers.Player.Spawning -= PLEV.PlayerSpawn;
            Exiled.Events.Handlers.Server.RespawningTeam -= PLEV.TeamSpawn;
            Exiled.Events.Handlers.Player.Joined -= PLEV.PlayerJoin;
            Exiled.Events.Handlers.Player.Escaping -= PLEV.PlayerEscape;
            Exiled.Events.Handlers.Player.Died -= PLEV.PlayerDeath;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= PLEV.PDDie;
            //Events.RemoteAdminCommandEvent -= PLEV.RACmd;
            Exiled.Events.Handlers.Player.Hurting -= PLEV.PlayerHurt;
            PLEV = null;
            instance = null;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            /*pluginDir = Path.Combine(Paths.Configs, "RoundMod");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!File.Exists(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml")))
                File.WriteAllText(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"), "");
            ConfigLoad();*/
            PLEV = new PluginEvents(this);
            Exiled.Events.Handlers.Server.RoundStarted += PLEV.RoundStart;
            Exiled.Events.Handlers.Server.RestartingRound += PLEV.RoundRestart;
            Exiled.Events.Handlers.Server.WaitingForPlayers += PLEV.WaitForPlayers;
            Exiled.Events.Handlers.Player.Spawning += PLEV.PlayerSpawn;
            Exiled.Events.Handlers.Server.RespawningTeam += PLEV.TeamSpawn;
            Exiled.Events.Handlers.Player.Joined += PLEV.PlayerJoin;
            Exiled.Events.Handlers.Player.Escaping += PLEV.PlayerEscape;
            Exiled.Events.Handlers.Player.Died += PLEV.PlayerDeath;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += PLEV.PDDie;
            //Events.RemoteAdminCommandEvent += PLEV.RACmd;
            Exiled.Events.Handlers.Player.Hurting += PLEV.PlayerHurt;
            instance = this;
            //Harmony instance2 = new Harmony("net.virtualbrightplayz.roundmod");
            //instance2.PatchAll();
        }

        /*public void ConfigLoad()
        {
            string data = File.ReadAllText(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"));
            var des = new DeserializerBuilder().Build();
            var data2 = (IDictionary<object, object>)des.Deserialize<object>(data);

            allowedTypes.Clear();
            translations.Clear();
            translations2.Clear();

            allowedRngMeds.Clear();
            allowedrngRoles.Clear();
            allowedrngRolesBoss.Clear();
            allowedRngWeapons.Clear();
            noInfectRoles.Clear();

            allowedRngMeds = data2.GetList("rm_rng.meds", new List<object>() { ItemType.Adrenaline, ItemType.Medkit, ItemType.Painkillers }).ConvertAll((c) => c.GetType() == typeof(ItemType) ? (ItemType)c : (ItemType)int.Parse((string)c));
            allowedRngWeapons = data2.GetList("rm_rng.weapons", new List<object>() { ItemType.GunCOM15, ItemType.GunE11SR, ItemType.GunLogicer, ItemType.GunMP7, ItemType.GunProject90, ItemType.GunUSP, ItemType.MicroHID, ItemType.GrenadeFlash, ItemType.GrenadeFrag }).ConvertAll((c) => c.GetType() == typeof(ItemType) ? (ItemType)c : (ItemType)int.Parse((string)c));
            allowedrngRoles = data2.GetList("rm_rng.samescps", new List<object>() { RoleType.Scp049, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 }).ConvertAll((c) => c.GetType() == typeof(RoleType) ? (RoleType)c : (RoleType)int.Parse((string)c));
            allowedrngRolesBoss = data2.GetList("rm_rng.bossscps", new List<object>() { RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 }).ConvertAll((c) => c.GetType() == typeof(RoleType) ? (RoleType)c : (RoleType)int.Parse((string)c));
            noInfectRoles = data2.GetList("rm_rng.noinfect", new List<object>() { RoleType.Scp079 }).ConvertAll((c) => c.GetType() == typeof(RoleType) ? (RoleType)c : (RoleType)int.Parse((string)c));
            notRandomizeItems = data2.GetList("rm_rng.notrandomized", new List<object>() { ItemType.KeycardScientist, ItemType.KeycardZoneManager, ItemType.KeycardJanitor }).ConvertAll((c) => c.GetType() == typeof(ItemType) ? (ItemType)c : (ItemType)int.Parse((string)c));

            bossHpMulti = data2.GetFloat("rm_boss_hp_multiply", 2f);
            maxMods = data2.GetInt("rm_max_mods", Enum.GetValues(typeof(ModType)).Length);
            //maxMods = Config.GetInt("rm_max_mods", Enum.GetValues(typeof(ModType)).Length);
            foreach (ModType item in Enum.GetValues(typeof(ModType)))
            {
                translations.Add(item, data2.GetString("rm_translation_" + item.ToString(), item.ToString()));
                //translations.Add(item, Config.GetString("rm_translation_" + item.ToString(), item.ToString()));
                //if (Config.GetBool("rm_allow_" + item.ToString(), true))
                if (data2.GetBool("rm_allow_" + item.ToString(), true))
                {
                    //int chance = Config.GetInt("rm_chance_" + item.ToString(), 1);
                    int chance = data2.GetInt("rm_chance_" + item.ToString(), 1);
                    for (int i = 0; i < chance; i++)
                        allowedTypes.Add(item);
                    enabledTypes.Add(item);
                }
                else
                {
                    Log.Debug("not allowing " + item.ToString());
                }
            }
            if (allowedTypes.Count == 0)
            {
                Log.Debug("no types defined, using no allowed types.");
                allowedTypes.Add(ModType.NONE);
                enabledTypes.Add(ModType.NONE);
            }
        }*/
    }
}
