using System;
using Harmony;
using UnityEngine;

namespace RHHarmony.A17LowDegradationHighlightsBorderRed
{
    public static class A17LowDegradationHighlightsBorderRed
    {
        [HarmonyPatch(typeof(XUiC_ItemStack))]
        [HarmonyPatch("Update")]
        [HarmonyPatch(new Type[] { typeof(float) })]
        class PatchXUiC_ItemStackUpdate
        {
            static void Postfix(XUiC_ItemStack __instance, ref float _dt)
            {
                if (__instance.itemClass != null)
                {
                    if (__instance.itemStack.itemValue.HasQuality || __instance.itemStack.itemValue.HasModSlots || __instance.itemStack.itemValue.IsMod)
                    {
                        if (__instance.durability != null && __instance.lockType != XUiC_ItemStack.LockTypes.Crafting)
                        {
                            if (__instance.itemStack.itemValue.PercentUsesLeft < 0.2f)
                            {
                                __instance.background.Color = new Color(1f, 0f, 0f);
                            }
                        }
                    }
                }
            }
        }
    }
}
