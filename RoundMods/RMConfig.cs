using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RoundMods
{
    public class RMConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("Items considered medical items")]
        public List<int> meds { get; set; } = new List<int>() { (int)ItemType.Adrenaline, (int)ItemType.Medkit, (int)ItemType.Painkillers };
        [Description("Items considered weapons")]
        public List<int> weapons { get; set; } = new List<int>() { (int)ItemType.GunCOM15, (int)ItemType.GunE11SR, (int)ItemType.GunLogicer, (int)ItemType.GunMP7, (int)ItemType.GunProject90, (int)ItemType.GunUSP, (int)ItemType.MicroHID, (int)ItemType.GrenadeFlash, (int)ItemType.GrenadeFrag };
        [Description("Roles to be used for Same SCP Type")]
        public List<int> samescps { get; set; } = new List<int>() { (int)RoleType.Scp049, (int)RoleType.Scp096, (int)RoleType.Scp106, (int)RoleType.Scp173, (int)RoleType.Scp93953, (int)RoleType.Scp93989 };
        [Description("Roles to be used for Boss SCP")]
        public List<int> bossscps { get; set; } = new List<int>() { (int)RoleType.Scp096, (int)RoleType.Scp106, (int)RoleType.Scp173, (int)RoleType.Scp93953, (int)RoleType.Scp93989 };
        [Description("Roles excluded from Class Infect")]
        public List<int> noinfect { get; set; } = new List<int>() { (int)RoleType.Scp079 };
        [Description("Items not randomized in item randomizer")]
        public List<int> notrandomized { get; set; } = new List<int>() { (int)ItemType.KeycardScientist, (int)ItemType.KeycardZoneManager, (int)ItemType.KeycardJanitor };
        [Description("Boss HP Multiplier")]
        public float bossHpMulti { get; set; } = 2f;
        [Description("Max mods per round")]
        public int maxMods { get; set; } = Enum.GetValues(typeof(ModType)).Length;
        [Description("The mods and their settings.")]
        public List<ModConfig> mods { get; set; } = new List<ModConfig>()
        {
            new ModConfig(ModType.NONE),
            new ModConfig(ModType.SINGLESCPTYPE),
            new ModConfig(ModType.PLAYERSIZE),
            new ModConfig(ModType.SCPBOSS),
            new ModConfig(ModType.UPSIDEDOWN),
            new ModConfig(ModType.NORESPAWN),
            new ModConfig(ModType.EXPLODEONDEATH),
            new ModConfig(ModType.FINDWEAPONS),
            new ModConfig(ModType.ITEMRANDOMIZER),
            new ModConfig(ModType.CLASSINFECT),
        };
    }

    public class ModConfig
    {
        [Description("Is this mod allowed?")]
        public bool allow { get; set; } = true;
        [Description("Visible name of the mod")]
        public string name { get; set; } = "Name";
        [Description("chance of the mod happening. There isn't a max for this. Min is 0.")]
        public int chance { get; set; } = 1;
        [Description("The mod type.")]
        public ModType mod { get; set; } = ModType.NONE;

        public ModConfig()
        { }

        public ModConfig(ModType type)
        {
            name = type.ToString();
            mod = type;
        }
    }
}