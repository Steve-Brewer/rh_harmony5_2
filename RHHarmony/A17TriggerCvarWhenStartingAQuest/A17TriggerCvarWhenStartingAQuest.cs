using System;
using Harmony;

namespace RHHarmony.A17TriggerCvarWhenStartingAQuest
{
    public static class A17TriggerCvarWhenStartingAQuest
    {
        // NOTE : To use add the following to an quest : <property name="CVar_Set" value="AS_MiningTools_Lvl|1" /> 
        // This is set the CVar 'AS_MiningTools_Lvl' to 1

        [HarmonyPatch(typeof(Quest))]
        [HarmonyPatch("StartQuest")]
        [HarmonyPatch(new Type[] { typeof(bool) })]
        class PatchQuestStartQuest
        {
            static bool Prefix(Quest __instance, ref bool newQuest)
            {
                if (__instance.QuestClass.Properties.Values.ContainsKey("CVar_Set"))
                {
                    string data = __instance.QuestClass.Properties.Values["CVar_Set"];
                    if (data.Contains("|"))
                    {
                        string[] array = data.Split(new char[]{'|'});
                        if (array.Length > 1)
                        {
                            float value = 0f;
                            if (float.TryParse(array[1], out value))
                            {
                                __instance.OwnerJournal.OwnerPlayer.SetCVar(array[0], value);
                            }
                        }
                    }
                }

                return true;
            }
        }
    }
}
