using System;
using Harmony;
using UnityEngine;

namespace RHHarmony.A17HighlightBloodmoonDays
{
    public static class A17HighlightBloodmoonDays
    {
        [HarmonyPatch(typeof(XUiC_CompassWindow))]
        [HarmonyPatch("GetBindingValue")]
        [HarmonyPatch(new Type[] { typeof(string), typeof(BindingItem) }, new ArgumentType[] { ArgumentType.Ref, ArgumentType.Normal })]
        class PatchXUiC_CompassWindowGetBindingValue
        {
            //NOTE : Requires  [{daycolor|always}] variable in xml element 'windowCompass' in file windows.xml

            static void Postfix(XUiC_CompassWindow __instance, ref string value, ref BindingItem binding)
            {
                string fieldName = binding.FieldName;
                if (fieldName != null && fieldName == "daycolor")
                {
                    value = "FFFFFF";
                    if (XUi.IsGameRunning())
                    {
                        int v4 = GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
                        int warning = GameStats.GetInt(EnumGameStats.BloodMoonWarning);
                        if (warning != -1)
                        {
                           
                            int bloodMoon = GameStats.GetInt(EnumGameStats.BloodMoonDay);
                            if (v4 == bloodMoon)
                            {
                                value = "FF0000";
                            }

                            // Only highlight previous 2 days if the BM is on a fixed cycle, i.e. every 3 days otherwise the warning willm spoil the suprise!
                            if (GamePrefs.GetInt(EnumGamePrefs.BloodMoonRange) == 0)
                            {
                                if (v4 == bloodMoon - 1)
                                {
                                    value = "FFA500";
                                }
                                else if (v4 == bloodMoon - 2)
                                {
                                    value = "FFFF00";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}