using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EXILED;
using EXILED.ApiObjects;
using EXILED.Extensions;
using Grenades;
using MEC;
using Mirror;
using UnityEngine;
using Harmony;
using System.IO;
using YamlDotNet.Serialization;

namespace RoundMods
{
    public class RoundMod : Plugin
    {
        public override string getName => "RoundMods";

        internal PluginEvents PLEV;
        public static RoundMod instance = null;

        public ModType curMod = ModType.NONE;

        public List<ModType> allowedTypes = new List<ModType>();
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
        public static string pluginDir;

        public override void OnDisable()
        {
            Events.RoundStartEvent -= PLEV.RoundStart;
            Events.RoundRestartEvent -= PLEV.RoundRestart;
            Events.WaitingForPlayersEvent -= PLEV.WaitForPlayers;
            Events.PlayerSpawnEvent -= PLEV.PlayerSpawn;
            Events.TeamRespawnEvent -= PLEV.TeamSpawn;
            Events.PlayerJoinEvent -= PLEV.PlayerJoin;
            Events.CheckEscapeEvent -= PLEV.PlayerEscape;
            Events.PlayerDeathEvent -= PLEV.PlayerDeath;
            Events.PocketDimDeathEvent -= PLEV.PDDie;
            Events.RemoteAdminCommandEvent -= PLEV.RACmd;
            Events.PlayerHurtEvent -= PLEV.PlayerHurt;
            PLEV = null;
            instance = null;
        }

        public override void OnEnable()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            pluginDir = Path.Combine(appData, "Plugins", "RoundMod");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!File.Exists(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml")))
                File.WriteAllText(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"), "");
            ConfigLoad();
            PLEV = new PluginEvents(this);
            Events.RoundStartEvent += PLEV.RoundStart;
            Events.RoundRestartEvent += PLEV.RoundRestart;
            Events.WaitingForPlayersEvent += PLEV.WaitForPlayers;
            Events.PlayerSpawnEvent += PLEV.PlayerSpawn;
            Events.TeamRespawnEvent += PLEV.TeamSpawn;
            Events.PlayerJoinEvent += PLEV.PlayerJoin;
            Events.CheckEscapeEvent += PLEV.PlayerEscape;
            Events.PlayerDeathEvent += PLEV.PlayerDeath;
            Events.PocketDimDeathEvent += PLEV.PDDie;
            Events.RemoteAdminCommandEvent += PLEV.RACmd;
            Events.PlayerHurtEvent += PLEV.PlayerHurt;
            instance = this;
            HarmonyInstance instance2 = HarmonyInstance.Create("net.virtualbrightplayz.roundmod");
            instance2.PatchAll();
        }

        public override void OnReload()
        {
            //ConfigLoad();
        }

        public void ConfigLoad()
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

            /*allowedrngRoles.Add(RoleType.Scp049);
            allowedrngRoles.Add(RoleType.Scp096);
            allowedrngRoles.Add(RoleType.Scp106);
            allowedrngRoles.Add(RoleType.Scp173);
            allowedrngRoles.Add(RoleType.Scp93953);
            allowedrngRoles.Add(RoleType.Scp93989);

            allowedrngRolesBoss.Add(RoleType.Scp049);
            allowedrngRolesBoss.Add(RoleType.Scp173);
            allowedrngRolesBoss.Add(RoleType.Scp096);
            allowedrngRolesBoss.Add(RoleType.Scp106);
            allowedrngRolesBoss.Add(RoleType.Scp93953);
            allowedrngRolesBoss.Add(RoleType.Scp93989);

            allowedRngWeapons.Add(ItemType.GunCOM15);
            allowedRngWeapons.Add(ItemType.GunE11SR);
            allowedRngWeapons.Add(ItemType.GunLogicer);
            allowedRngWeapons.Add(ItemType.GunMP7);
            allowedRngWeapons.Add(ItemType.GunProject90);
            allowedRngWeapons.Add(ItemType.GunUSP);

            allowedRngMeds.Add(ItemType.Medkit);
            allowedRngMeds.Add(ItemType.Adrenaline);
            allowedRngMeds.Add(ItemType.Painkillers);*/
        }
    }
}
