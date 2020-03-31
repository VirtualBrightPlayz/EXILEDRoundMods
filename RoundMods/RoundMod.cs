﻿using System;
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
            instance = this;
            HarmonyInstance instance2 = HarmonyInstance.Create("net.virtualbrightplayz.roundmod");
            instance2.PatchAll();
        }

        public override void OnReload()
        {
            ConfigLoad();
        }

        public void ConfigLoad()
        {
            string data = File.ReadAllText(Path.Combine(pluginDir, "config-" + typeof(ServerStatic).GetField("ServerPort", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString() + ".yml"));
            var des = new DeserializerBuilder().Build();
            var data2 = (IDictionary<object, object>)des.Deserialize<object>(data);

            allowedTypes.Clear();
            translations.Clear();
            translations2.Clear();
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
        }
    }

    public static class ConfigExtentions
    {
        public static bool GetBool(this IDictionary<object, object> data2, string key, bool defaultstr)
        {
            bool ret = defaultstr;
            if (!bool.TryParse(GetString(data2, key, defaultstr.ToString()), out ret))
            {
                Log.Error("Config " + key + " is the wrong type (expected type: bool).");
                return defaultstr;
            }
            return ret;
        }

        public static int GetInt(this IDictionary<object, object> data2, string key, int defaultstr)
        {
            int ret = defaultstr;
            if (!int.TryParse(GetString(data2, key, defaultstr.ToString()), out ret))
            {
                Log.Error("Config " + key + " is the wrong type (expected type: int).");
                return defaultstr;
            }
            return ret;
        }

        public static float GetFloat(this IDictionary<object, object> data2, string key, float defaultstr)
        {
            float ret = defaultstr;
            if (!float.TryParse(GetString(data2, key, defaultstr.ToString()), out ret))
            {
                Log.Error("Config " + key + " is the wrong type (expected type: int).");
                return defaultstr;
            }
            return ret;
        }

        public static string GetString(this IDictionary<object, object> data2, string key, string defaultstr)
        {
            if (data2 == null)
            {
                Log.Error("Config data was not found. Did you make a config?");
                return defaultstr;
            }
            string ret = defaultstr;
            try
            {
                ret = (string)data2[key];
            }
            catch (KeyNotFoundException)
            {
                Log.Debug("Config " + key + " was not found.");
                return defaultstr;
            }
            catch (InvalidCastException)
            {
                Log.Error("Config " + key + " is invalid.");
                return defaultstr;
            }
            return ret;
        }
    }

    [Flags]
    public enum ModType
    {
        NONE = 1,
        SINGLESCPTYPE = 2,
        PLAYERSIZE = 4,
        SCPBOSS = 8,
        NOWEAPONS = 16,
        NORESPAWN = 64,
        EXPLODEONDEATH = 128,
        FINDWEAPONS = 256,
        ITEMRANDOMIZER = 512,
        CLASSINFECT = 1024,
    }

    public enum ModCategory
    {
        NONE,
        SCPMODS,
        PLAYERMODS,
        DEATHMODS,
        TEAMMODS,
        ITEMMODS,
    }

    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    public class CassieFix
    {
        public static bool Prefix(NineTailedFoxAnnouncer __instance, Role scp, PlayerStats.HitInfo hit, string groupId)
        {
            if (RoundMod.instance.curMod.HasFlag(ModType.CLASSINFECT) && RoundMod.instance.allowedTypes.Contains(ModType.CLASSINFECT))
            {
                if (PlayerManager.players.FindAll((g) =>
                {
                    if (g.GetComponent<ReferenceHub>().GetTeam() == Team.SCP)
                        return true;
                    return false;
                }).Count == 0)
                {
                    return true;
                }
                return false;
            }
            /*if (RoundMod.instance.curMod.HasFlag(ModType.SINGLESCPTYPE) && RoundMod.instance.allowedTypes.Contains(ModType.SINGLESCPTYPE))
            {
                if (PlayerManager.players.FindAll((g) =>
                {
                    if (g.GetComponent<ReferenceHub>().GetTeam() == Team.SCP)
                        return true;
                    return false;
                }).Count == 0)
                {
                    return true;
                }
                return false;
            }*/
            return true;
        }
    }

    public class PluginEvents
    {
        internal RoundMod plugin;
        internal bool roundStarted = false;
        internal bool respawning = false;
        internal List<ModType> curMods = new List<ModType>();
        internal RoleType rngRole = RoleType.Scp173;
        internal RoleType rngRoleBoss = RoleType.Scp173;
        internal List<RoleType> allowedrngRoles = new List<RoleType>();
        internal List<RoleType> allowedrngRolesBoss = new List<RoleType>();
        internal List<ItemType> allowedRngWeapons = new List<ItemType>();
        internal List<ItemType> allowedRngMeds = new List<ItemType>();
        internal Dictionary<ModType, ModCategory> mods = new Dictionary<ModType, ModCategory>();
        internal GameObject boss;

        public PluginEvents(RoundMod mod)
        {
            plugin = mod;
            roundStarted = false;
            respawning = false;

            mods.Add(ModType.NONE, ModCategory.NONE);
            mods.Add(ModType.SINGLESCPTYPE, ModCategory.SCPMODS);
            mods.Add(ModType.PLAYERSIZE, ModCategory.PLAYERMODS);
            mods.Add(ModType.SCPBOSS, ModCategory.SCPMODS);
            mods.Add(ModType.NOWEAPONS, ModCategory.ITEMMODS);
            mods.Add(ModType.NORESPAWN, ModCategory.TEAMMODS);
            mods.Add(ModType.EXPLODEONDEATH, ModCategory.DEATHMODS);
            mods.Add(ModType.FINDWEAPONS, ModCategory.ITEMMODS);
            mods.Add(ModType.ITEMRANDOMIZER, ModCategory.NONE);
            mods.Add(ModType.CLASSINFECT, ModCategory.DEATHMODS);

            allowedrngRoles.Add(RoleType.Scp049);
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
            allowedRngMeds.Add(ItemType.Painkillers);

            GetRandom();
        }

        internal void RoundRestart()
        {
            roundStarted = false;
            if (plugin.curMod.HasFlag(ModType.NONE) && plugin.enabledTypes.Contains(ModType.NONE))
            {
                return; // cuz none means none
            }
            if (plugin.curMod.HasFlag(ModType.SINGLESCPTYPE) && plugin.enabledTypes.Contains(ModType.SINGLESCPTYPE))
            {
            }
            if (plugin.curMod.HasFlag(ModType.PLAYERSIZE) && plugin.enabledTypes.Contains(ModType.PLAYERSIZE))
            {
            }
            if (plugin.curMod.HasFlag(ModType.SCPBOSS) && plugin.enabledTypes.Contains(ModType.SCPBOSS))
            {
            }
            if (plugin.curMod.HasFlag(ModType.NOWEAPONS) && plugin.enabledTypes.Contains(ModType.NOWEAPONS))
            {
            }
            if (plugin.curMod.HasFlag(ModType.NORESPAWN) && plugin.enabledTypes.Contains(ModType.NORESPAWN))
            {
            }
            if (plugin.curMod.HasFlag(ModType.EXPLODEONDEATH) && plugin.enabledTypes.Contains(ModType.EXPLODEONDEATH))
            {
            }
            if (plugin.curMod.HasFlag(ModType.FINDWEAPONS) && plugin.enabledTypes.Contains(ModType.FINDWEAPONS))
            {
            }
        }

        internal void RoundStart()
        {
            Timing.RunCoroutine(SetRoundStarted());
            roundStarted = false;
            boss = null;
            if (plugin.curMod.HasFlag(ModType.NONE) && plugin.enabledTypes.Contains(ModType.NONE))
            {
                return; // cuz none means none
            }
            if (plugin.curMod.HasFlag(ModType.NOWEAPONS) && plugin.enabledTypes.Contains(ModType.NOWEAPONS))
            {
                Timing.RunCoroutine(DeleteWeapons());
            }
            if (plugin.curMod.HasFlag(ModType.FINDWEAPONS) && plugin.enabledTypes.Contains(ModType.FINDWEAPONS))
            {
                Timing.RunCoroutine(SpawnFindWeapons());
            }
            if (plugin.curMod.HasFlag(ModType.ITEMRANDOMIZER) && plugin.enabledTypes.Contains(ModType.ITEMRANDOMIZER))
            {
                Timing.RunCoroutine(RandomItems());
            }
        }

        private IEnumerator<float> SpawnFindWeapons()
        {
            yield return Timing.WaitForSeconds(0.2f);
            List<Room> rooms = new List<Room>();
            foreach (Room room in Map.GetRooms())
            {
                if (UnityEngine.Random.Range(0, 3) == 0)
                    rooms.Add(room);
            }
            foreach (Room room in rooms)
            {
                ItemType weapon = allowedRngWeapons[UnityEngine.Random.Range(0, allowedRngWeapons.Count)];
                Map.SpawnItem(weapon, 0f, room.Position + Vector3.up * 2f).RefreshDurability(true, true);
                //room.Position
            }
        }

        private IEnumerator<float> RandomItems()
        {
            yield return Timing.WaitForSeconds(0.1f);
            ItemType[] items = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>();
            foreach (Pickup item in GameObject.FindObjectsOfType<Pickup>())
            {
                if (item.ItemId == ItemType.KeycardZoneManager || item.ItemId == ItemType.KeycardScientist || item.ItemId == ItemType.KeycardJanitor)
                {
                }
                else
                {
                    Map.SpawnItem(items[UnityEngine.Random.Range(0, items.Length)], 0f, item.transform.position + Vector3.up * 0.1f).RefreshDurability(true, true);
                    item.Delete();
                    //item.SetIDFull(items[UnityEngine.Random.Range(0, items.Length)]);
                }
            }
        }

        private IEnumerator<float> DeleteWeapons()
        {
            yield return Timing.WaitForSeconds(0.2f);
            foreach (Pickup item in GameObject.FindObjectsOfType<Pickup>())
            {
                if (item.ItemId == ItemType.GunCOM15 || item.ItemId == ItemType.GunE11SR || item.ItemId == ItemType.GunLogicer || item.ItemId == ItemType.GunMP7 || item.ItemId == ItemType.GunProject90 || item.ItemId == ItemType.GunUSP || item.ItemId == ItemType.GrenadeFlash || item.ItemId == ItemType.GrenadeFrag || item.ItemId == ItemType.MicroHID || item.ItemId == ItemType.GunCOM15)
                {
                    item.Delete();
                }
            }
        }

        private IEnumerator<float> SetRoundStarted()
        {
            yield return Timing.WaitForSeconds(1f);
            roundStarted = true;
        }

        private IEnumerator<float> SetRespawnStop()
        {
            yield return Timing.WaitForSeconds(1f);
            respawning = false;
        }

        public void GetRandom()
        {
            rngRole = allowedrngRoles[UnityEngine.Random.Range(0, allowedrngRoles.Count)];
            rngRoleBoss = allowedrngRolesBoss[UnityEngine.Random.Range(0, allowedrngRolesBoss.Count)];
            plugin.curMod = 0;
            List<ModCategory> curModCategories = new List<ModCategory>();
            int rec = 0;
            for (int i = 0; i < plugin.maxMods; i++)
            {
                rec++;
                ModType modtype = plugin.allowedTypes[UnityEngine.Random.Range(0, plugin.allowedTypes.Count)];
                if (mods[modtype] != ModCategory.NONE && curModCategories.Contains(mods[modtype]))
                {
                    if (rec < 5) //prevent lag
                        i--;
                    continue;
                }
                plugin.curMod |= modtype;
                curModCategories.Add(mods[modtype]);
            }
            Log.Debug("curMod: " + plugin.curMod.ToString());
            Log.Debug("rngRole: " + rngRole.ToString());
            Log.Debug("rngRoleBoss: " + rngRoleBoss.ToString());
        }

        internal void WaitForPlayers()
        {
            roundStarted = false;
            GetRandom();
            try
            {
                ServerConsole.singleton.NameFormatter.Commands.Add("rm_current_mods", (e) =>
                {
                    if (plugin.curMod.HasFlag(ModType.NONE) && plugin.enabledTypes.Contains(ModType.NONE))
                    {
                        return plugin.translations[ModType.NONE];
                    }
                    else
                    {
                        string str = string.Empty;
                        int j = 0;
                        for (int i = 0; i < Enum.GetValues(typeof(ModType)).Length; i++)
                        {
                            ModType item = Enum.GetValues(typeof(ModType)).ToArray<ModType>()[i];
                            if (plugin.curMod.HasFlag(item))
                            {
                                j++;
                                if (j == 1)
                                    str += plugin.translations[item];
                                else
                                    str += ", " + plugin.translations[item];
                            }
                        }
                        return str;
                    }
                });
            }
            catch (Exception e)
            { }
        }

        internal void PlayerSpawn(PlayerSpawnEvent ev)
        {
            //Log.Debug("roundStarted " + roundStarted);
            if (plugin.curMod.HasFlag(ModType.NONE) && plugin.enabledTypes.Contains(ModType.NONE))
            {
                return; // cuz none means none
            }
            if (plugin.curMod.HasFlag(ModType.SINGLESCPTYPE) && plugin.enabledTypes.Contains(ModType.SINGLESCPTYPE) && !(plugin.curMod.HasFlag(ModType.SCPBOSS) && plugin.enabledTypes.Contains(ModType.SCPBOSS)))
            {
                if (!roundStarted || respawning)
                {
                    if (ev.Role == RoleType.Scp049 || ev.Role == RoleType.Scp0492 || ev.Role == RoleType.Scp079 || ev.Role == RoleType.Scp096 || ev.Role == RoleType.Scp106 || ev.Role == RoleType.Scp173 || ev.Role == RoleType.Scp93953 || ev.Role == RoleType.Scp93989)
                    {
                        ev.Role = rngRole;
                        Timing.RunCoroutine(ChangeClassLate(ev.Player, rngRole, -1));
                    }
                }
            }
            if (plugin.curMod.HasFlag(ModType.PLAYERSIZE) && plugin.enabledTypes.Contains(ModType.PLAYERSIZE))
            {
                if (!roundStarted || respawning)
                {
                    Timing.RunCoroutine(ChangeSizeLate(ev.Player));
                }
            }
            if (plugin.curMod.HasFlag(ModType.SCPBOSS) && plugin.enabledTypes.Contains(ModType.SCPBOSS))
            {
                if (!roundStarted || respawning)
                {
                    if (ev.Player.gameObject.Equals(boss))
                    { }
                    else
                    {
                        if (ev.Role == RoleType.Scp049 || ev.Role == RoleType.Scp0492 || ev.Role == RoleType.Scp079 || ev.Role == RoleType.Scp096 || ev.Role == RoleType.Scp106 || ev.Role == RoleType.Scp173 || ev.Role == RoleType.Scp93953 || ev.Role == RoleType.Scp93989)
                        {
                            if (boss == null)
                            {
                                boss = ev.Player.gameObject;
                                ev.Role = rngRoleBoss;
                                Timing.RunCoroutine(ChangeClassLate(ev.Player, rngRoleBoss, 2.5f, true));
                                //Timing.RunCoroutine(ChangeSizeLate(ev.Player));
                            }
                            else
                            {
                                ev.Role = RoleType.ClassD;
                                Timing.RunCoroutine(ChangeClassLate(ev.Player, RoleType.ClassD, -1));
                            }
                        }
                    }
                }
            }
            if (plugin.curMod.HasFlag(ModType.NOWEAPONS) && plugin.enabledTypes.Contains(ModType.NOWEAPONS))
            {
                if (!roundStarted)
                {
                    if (ev.Role == RoleType.ChaosInsurgency || ev.Role == RoleType.NtfCadet || ev.Role == RoleType.NtfCommander || ev.Role == RoleType.NtfLieutenant || ev.Role == RoleType.NtfScientist || ev.Role == RoleType.FacilityGuard)
                    {
                        ev.Role = RoleType.ClassD;
                        Timing.RunCoroutine(ChangeClassLate(ev.Player, RoleType.ClassD, -1));
                    }
                }
            }
            if (plugin.curMod.HasFlag(ModType.FINDWEAPONS) && plugin.enabledTypes.Contains(ModType.FINDWEAPONS))
            {
                //delete weapons
                Timing.RunCoroutine(DeletePlayerWeaponsLate(ev.Player));
            }
        }

        private IEnumerator<float> DeletePlayerWeaponsLate(ReferenceHub player)
        {
            yield return Timing.WaitForSeconds(1f);
            List<Inventory.SyncItemInfo> delete = new List<Inventory.SyncItemInfo>();
            for (int i = 0; i < player.inventory.items.Count; i++)
            {
                Inventory.SyncItemInfo item = player.inventory.items[i];
                if (item.id == ItemType.GunCOM15 || item.id == ItemType.GunE11SR || item.id == ItemType.GunLogicer || item.id == ItemType.GunMP7 || item.id == ItemType.GunProject90 || item.id == ItemType.GunUSP || item.id == ItemType.GrenadeFlash || item.id == ItemType.GrenadeFrag || item.id == ItemType.MicroHID || item.id == ItemType.GunCOM15)
                {
                    delete.Add(item);
                }
            }
            foreach (Inventory.SyncItemInfo item in delete)
            {
                player.inventory.items.Remove(item);
            }
        }

        private IEnumerator<float> ChangeClassLate(ReferenceHub player, RoleType @class, float hp, bool multiply = false)
        {
            yield return Timing.WaitForSeconds(1f);
            player.inventory.Clear();
            player.characterClassManager.SetClassID(@class);
            if (hp > 0)
            {
                yield return Timing.WaitForSeconds(10f);
                if (multiply)
                {
                    player.playerStats.maxHP = (int)(hp * player.playerStats.maxHP);
                    player.playerStats.health = (player.playerStats.maxHP);
                }
                else
                {
                    player.playerStats.maxHP = (int)hp;
                    player.playerStats.health = hp;
                }
                SetPlayerScaleGalaxy119(player.gameObject, 1f, 1f, 1f);
                SetPlayerScaleGalaxy119NoClient(player.gameObject, 2f, 2f, 2f);
            }
            else
            {
                yield return Timing.WaitForSeconds(7f);
                player.playerStats.health = player.playerStats.maxHP;
            }
        }

        private IEnumerator<float> ChangeSizeLate(ReferenceHub player)
        {
            yield return Timing.WaitForSeconds(10f);
            //player.characterClassManager.SetClassID(player.characterClassManager.CurClass);
            float scl = UnityEngine.Random.Range(0.4f, 1.1f);
            //SetPlayerScaleGalaxy119(player.gameObject, scl, scl, scl);
            player.playerStats.maxHP = (int)(player.playerStats.maxHP * scl);
            player.playerStats.health = player.playerStats.maxHP;
            yield return Timing.WaitForSeconds(UnityEngine.Random.Range(0.1f, 2.5f));
            if (boss != player.gameObject && Vector3.Distance(Vector3.one, player.gameObject.transform.localScale) <= 0.05f)
            {
                SetPlayerScaleGalaxy119(player.gameObject, scl, scl, scl);
                yield return Timing.WaitForSeconds(0.5f);
                SetPlayerScaleGalaxy119(player.gameObject, scl, scl, scl);
            }
        }

        public void SetPlayerScaleGalaxy119NoClient(GameObject target, float x, float y, float z)
        {
            try
            {
                NetworkIdentity identity = target.GetComponent<NetworkIdentity>();


                target.transform.localScale = new Vector3(1 * x, 1 * y, 1 * z);

                ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage();
                destroyMessage.netId = identity.netId;


                foreach (GameObject player in PlayerManager.players)
                {
                    if (player == target)
                        continue;

                    NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;

                    if (player != target)
                        playerCon.Send(destroyMessage, 0);

                    object[] parameters = new object[] { identity, playerCon };
                    typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }

        // credit to galaxy119, taken from AdminTools (https://github.com/galaxy119/AdminTools)
        public void SetPlayerScaleGalaxy119(GameObject target, float x, float y, float z)
        {
            try
            {
                NetworkIdentity identity = target.GetComponent<NetworkIdentity>();


                target.transform.localScale = new Vector3(1 * x, 1 * y, 1 * z);

                ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage();
                destroyMessage.netId = identity.netId;


                foreach (GameObject player in PlayerManager.players)
                {
                    NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;

                    if (player != target)
                        playerCon.Send(destroyMessage, 0);

                    object[] parameters = new object[] { identity, playerCon };
                    typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }

        internal void TeamSpawn(ref TeamRespawnEvent ev)
        {
            respawning = true;
            Timing.RunCoroutine(SetRespawnStop());
            if (plugin.curMod.HasFlag(ModType.NORESPAWN) && plugin.enabledTypes.Contains(ModType.NORESPAWN))
            {
                ev.ToRespawn.Clear();
            }
            if (plugin.curMod.HasFlag(ModType.FINDWEAPONS) && plugin.enabledTypes.Contains(ModType.FINDWEAPONS))
            {
                Timing.RunCoroutine(SpawnFindWeapons());
            }
            if (plugin.curMod.HasFlag(ModType.ITEMRANDOMIZER) && plugin.enabledTypes.Contains(ModType.ITEMRANDOMIZER))
            {
                Timing.RunCoroutine(RandomItems());
            }
        }

        internal void PlayerJoin(PlayerJoinEvent ev)
        {
            Timing.RunCoroutine(BroadcastCurrentMods(ev.Player));
        }

        private IEnumerator<float> BroadcastCurrentMods(ReferenceHub player)
        {
            yield return Timing.WaitForSeconds(0.1f);
            player.GetComponent<Broadcast>().TargetAddElement(player.characterClassManager.connectionToClient, "Current Round Modifier(s):", 5, false);
            if (plugin.curMod.HasFlag(ModType.NONE) && plugin.allowedTypes.Contains(ModType.NONE))
            {
                player.GetComponent<Broadcast>().TargetAddElement(player.characterClassManager.connectionToClient, plugin.translations[ModType.NONE], 5, false);
            }
            else
            {
                foreach (ModType item in Enum.GetValues(typeof(ModType)))
                {
                    if (plugin.curMod.HasFlag(item))
                        player.GetComponent<Broadcast>().TargetAddElement(player.characterClassManager.connectionToClient, plugin.translations[item], 1, false);
                }
                //player.GetComponent<Broadcast>().TargetAddElement(player.characterClassManager.connectionToClient, plugin.curMod.ToString(), 5, false);
            }
        }

        internal void PlayerEscape(ref CheckEscapeEvent ev)
        {
            if (plugin.curMod.HasFlag(ModType.PLAYERSIZE) && plugin.enabledTypes.Contains(ModType.PLAYERSIZE))
            {
                Timing.RunCoroutine(ChangeSizeLate(ev.Player));
            }
        }

        internal void PlayerDeath(ref PlayerDeathEvent ev)
        {
            if (plugin.curMod.HasFlag(ModType.EXPLODEONDEATH) && plugin.enabledTypes.Contains(ModType.EXPLODEONDEATH))
            {
                if (ev.Killer == null || (ev.Killer != null && ev.Killer.GetTeam() != Team.SCP && ev.Killer.GetTeam() != Team.TUT))
                {
                    GrenadeManager gm = ev.Player.GetComponent<GrenadeManager>();
                    GrenadeSettings set = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFrag);
                    if (set == null)
                    { }
                    else
                    {
                        Grenade comp = UnityEngine.Object.Instantiate(set.grenadeInstance).GetComponent<Grenade>();
                        comp.fuseDuration = 0.1f;
                        comp.InitData(gm, Vector3.zero, Vector3.zero, 10f);
                        //comp.fuseTime = 0f;
                        NetworkServer.Spawn(comp.gameObject);
                    }
                }
            }
            if (plugin.curMod.HasFlag(ModType.CLASSINFECT) && plugin.enabledTypes.Contains(ModType.CLASSINFECT))
            {
                if (ev.Killer != null && ev.Killer != ev.Player && ev.Killer.characterClassManager.CurClass != RoleType.Scp079)
                {
                    Timing.RunCoroutine(InfectLate(ev.Player, ev.Killer.characterClassManager.CurClass, ev.Killer.transform.position));
                }
            }
        }

        private IEnumerator<float> InfectLate(ReferenceHub player, RoleType curClass, Vector3 position)
        {
            yield return Timing.WaitForSeconds(0.5f);
            player.playerStats.maxHP = 100;
            player.playerStats.health = 100;
            player.characterClassManager.SetClassIDAdv(curClass, true);
            yield return Timing.WaitForSeconds(1.5f);
            player.plyMovementSync.TargetForcePosition(player.characterClassManager.connectionToClient, position);
            player.plyMovementSync.OverridePosition(position, 0f, true);
            yield return Timing.WaitForSeconds(0.5f);
            player.playerStats.maxHP = 100;
            player.playerStats.health = 100;
        }

        internal void PDDie(PocketDimDeathEvent ev)
        {
            if (plugin.curMod.HasFlag(ModType.CLASSINFECT) && plugin.enabledTypes.Contains(ModType.CLASSINFECT))
            {
                Timing.RunCoroutine(InfectLate(ev.Player, RoleType.Scp106, Map.GetRandomSpawnPoint(RoleType.Scp106)));
            }
        }

        internal void RACmd(ref RACommandEvent ev)
        {
            string[] args = ev.Command.Split(' ');
            //Player.StrHubs[ev.Sender.SenderId].CheckPermission("roundmod.setmod");
            ReferenceHub player = ev.Sender.SenderId == "SERVER CONSOLE" || ev.Sender.SenderId == "GAME CONSOLE" ? PlayerManager.localPlayer.GetPlayer() : Player.GetPlayer(ev.Sender.SenderId);
            if (player.CheckPermission("roundmod.setmod"))
            {
                /*if (args[0].ToUpper().Equals("RM_SET_BOSS"))
                {
                    if (args.Length > 1)
                    {
                        List<string> vs = new List<string>(args);
                        vs.RemoveAt(0);
                        string arg = vs.Join(null, " ");
                        plugin.curMod = (ModType)Enum.Parse(typeof(ModType), arg);
                        ev.Sender.RAMessage("Set Active Mods to: " + plugin.curMod.ToString(), pluginName: plugin.getName);
                    }
                    else
                    {
                        ev.Sender.RAMessage("Active Mods: " + plugin.curMod.ToString(), pluginName: plugin.getName);

                    }
                }*/
                if (args[0].ToUpper().Equals("RM_SET_MOD"))
                {
                    if (args.Length > 1)
                    {
                        List<string> vs = new List<string>(args);
                        vs.RemoveAt(0);
                        string arg = vs.Join(null, " ");
                        plugin.curMod = (ModType)Enum.Parse(typeof(ModType), arg);
                        ev.Sender.RAMessage("Set Active Mods to: " + plugin.curMod.ToString(), pluginName: plugin.getName);
                    }
                    else
                    {
                        ev.Sender.RAMessage("Active Mods: " + plugin.curMod.ToString(), pluginName: plugin.getName);

                    }
                }
                if (args[0].ToUpper().StartsWith("RM_MOD"))
                {
                    bool found = false;
                    foreach (ModType item in Enum.GetValues(typeof(ModType)))
                    {
                        if (args[0].ToUpper().Equals(("RM_MOD_" + item.ToString()).ToUpper()))
                        {
                            found = true;
                            if (args.Length != 2)
                            {
                                if (plugin.curMod.HasFlag(item))
                                    plugin.curMod &= ~item;
                                else
                                    plugin.curMod |= item;
                            }
                            else
                            {
                                bool res;
                                if (bool.TryParse(args[1], out res))
                                {
                                    if (res)
                                        plugin.curMod |= item;
                                    else
                                        plugin.curMod &= ~item;
                                }
                            }
                            break;
                        }
                    }
                    if (!found)
                    {
                        //Enum.GetValues(typeof(ModType)).ToArray<ModType>().ToList().FindAll((sel) => plugin.allowedTypes.Contains(sel));
                        ev.Sender.RAMessage("RoundMod by VirtualBrightPlayz/Brian Zulch\nUsable ModTypes:\n" + plugin.enabledTypes.Join((t) => t.ToString(), "\n") + "\nCommands: RM_MOD\nRM_MOD_<MODTYPE> [true|false]\nRM_SET_MOD <MODTYPEFLAGS>", pluginName: plugin.getName);
                    }
                }
            }
        }
    }

    // credit to galaxy119, taken from AdminTools (https://github.com/galaxy119/AdminTools)
    public static class ExtensionsGalaxy119
    {
        public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Public;
            MethodInfo info = type.GetMethod(methodName, flags);
            info?.Invoke(null, param);
        }
    }
}