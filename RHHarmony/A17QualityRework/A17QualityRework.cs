using System;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;

namespace RHHarmony.A17QualityRework
{
    public static class A17QualityRework
    {
        [HarmonyPatch(typeof(ItemValue), MethodType.Constructor)]
        [HarmonyPatch("ItemValue")]
        [HarmonyPatch(new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(FastTags), typeof(float) })]
        class PatchItemValueItemValue
        {
            static bool Prefix(ItemValue __instance, ref int _type, ref int minQuality, ref int maxQuality, ref bool _bCreateDefaultModItems, ref FastTags modsToInstall, ref float modInstallDescendingChance)
            {
                if (maxQuality == 6)
                    maxQuality = 120;

                return true;
            }
        }

        [HarmonyPatch(typeof(QualityInfo))]
        [HarmonyPatch("GetTierColor")]
        [HarmonyPatch(new Type[] { typeof(int) })]
        class PatchQualityInfoGetTierColor
        {
            static bool Prefix(QualityInfo __instance, ref int _tier)
            {
                if (_tier > 0)
                {
                    _tier = (int)Math.Floor(_tier / 20 + 1m);
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(QualityInfo))]
        [HarmonyPatch("GetQualityColorHex")]
        [HarmonyPatch(new Type[] { typeof(int) })]
        class PatchQualityInfoGetQualityColorHex
        {
            static bool Prefix(QualityInfo __instance, ref int _quality)
            {
                if (_quality > 0)
                {
                    _quality = (int)Math.Floor(_quality / 20 + 1m);
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(XUiC_RecipeStack))]
        [HarmonyPatch("SetRecipe")]
        [HarmonyPatch(new Type[] { typeof(Recipe), typeof(int), typeof(float), typeof(bool), typeof(int), typeof(int), typeof(float) })]
        class PatchXUiC_RecipeStackSetRecipe
        {
            static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 6f)
                    {
                        codes[i].operand = 120f;
                        break;
                    }
                }
                return codes.AsEnumerable();
            }
        }

        // This change covers 2 of the original changes in this class TraderInfo
        [HarmonyPatch(typeof(TraderInfo))]
        [HarmonyPatch("applyQuality")]
        [HarmonyPatch(new Type[] { typeof(ItemValue), typeof(int), typeof(int) }, new ArgumentType[] { ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal })]
        class PatchTraderInfoapplyQuality
        {
            static bool Prefix(ItemValue __instance, ref ItemValue _itemValue, int minQuality, ref int maxQuality)
            {
                if (maxQuality == 6)
                    maxQuality = 120;

                return true;
            }
        }

        [HarmonyPatch(typeof(XUiM_Trader))]
        [HarmonyPatch("GetBuyPrice")]
        [HarmonyPatch(new Type[] { typeof(XUi), typeof(ItemValue), typeof(int), typeof(ItemClass), typeof(int) })]
        class PatchXUiM_TraderGetBuyPrice
        {
            static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 5f)
                    {
                        codes[i].operand = 120f;
                        break;
                    }
                }
                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(XUiM_Trader))]
        [HarmonyPatch("GetSellPrice")]
        [HarmonyPatch(new Type[] { typeof(XUi), typeof(ItemValue), typeof(int), typeof(ItemClass) })]
        class PatchXUiM_TraderGetSellPrice
        {
            static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 5f)
                    {
                        codes[i].operand = 120f;
                        break;
                    }
                }
                return codes.AsEnumerable();
            }
        }

    }
}
