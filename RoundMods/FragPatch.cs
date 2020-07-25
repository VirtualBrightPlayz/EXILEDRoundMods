using Grenades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RoundMods
{
    // commented for stable release
    /*[HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ServersideExplosion))]
    public class FragPatch
    {
        public static MethodInfo getShootPerms = typeof(WeaponManager).GetMethod(nameof(WeaponManager.GetShootPermission), BindingFlags.Public | BindingFlags.Instance);

        public static IEnumerable<CodeInstruction> Transpiler(FragGrenade __instance, IEnumerable<CodeInstruction> instructions)
        {
            if (!PluginEvents.deathGrenades.Contains(__instance))
            {
                foreach (var item in instructions)
                {
                    yield return item;
                }
                yield break;
            }
            PluginEvents.deathGrenades.Remove(__instance);
            var found = false;
            List<CodeInstruction> list = new List<CodeInstruction>();
            for (int i = 0; i < instructions.ToArray().Length; i++)
            {
                var instruction = instructions.ToArray()[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand == getShootPerms)
                {
                    list.RemoveAt(i - 1);
                    list.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
                    found = true;
                }
                list.Add(instruction);
            }
            if (found == false)
                Log.Error("Cannot find  in FragGrenade.ServersideExplosion");
            foreach (var item in list)
            {
                yield return item;
            }
        }
    }*/
}
