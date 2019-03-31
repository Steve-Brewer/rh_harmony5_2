using System;
using Harmony;
using System.Reflection;

namespace RHHarmony.A17AdjustSkillProgression
{
    public static class A17AdjustSkillProgression
    {
        [HarmonyPatch(typeof(Progression))]
        [HarmonyPatch("AddLevelExpRecursive")]
        [HarmonyPatch(new Type[] { typeof(int), typeof(string) })]
        class PatchProgressionAddLevelExpRecursive
        {
            static bool Prefix(Progression __instance, ref int exp, ref string _cvarXPName)
            {
                if (__instance.Level >= Progression.MaxLevel)
                {
                    if (__instance.Level > Progression.MaxLevel)
                    {
                        __instance.Level = Progression.MaxLevel;
                    }

                    // Max level so dont even need to run the original method!
                    return false;
                }

                var nextLevel = __instance.Level + 1;
                var xpLeftToGo = __instance.ExpToNextLevel - exp;

                if (nextLevel % 10 == 0 && xpLeftToGo <= 0)
                {
                    __instance.SkillPoints += 4;
                }

                return true;
            }
        }
    }
}
