using EXILED.Extensions;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoundMods
{
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    public class CassieFix
    {
        public static bool Prefix(NineTailedFoxAnnouncer __instance, Role scp, PlayerStats.HitInfo hit, string groupId)
        {
            if (RoundMod.instance.curMod.HasFlag(ModType.NONE) && RoundMod.instance.enabledTypes.Contains(ModType.NONE))
            {
                return true; // cuz none means none
            }
            if (RoundMod.instance.curMod.HasFlag(ModType.CLASSINFECT) && RoundMod.instance.enabledTypes.Contains(ModType.CLASSINFECT))
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
            /*if (RoundMod.instance.curMod.HasFlag(ModType.SINGLESCPTYPE) && RoundMod.instance.enabledTypes.Contains(ModType.SINGLESCPTYPE))
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
}
