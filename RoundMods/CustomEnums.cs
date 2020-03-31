using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoundMods
{

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
}
