using System;
using Harmony;
using System.Linq;
using System.Reflection;

namespace RHHarmony.A17FixQuestRepeatableBug
{
    public static class A17FixQuestRepeatableBug
    {
        [HarmonyPatch(typeof(QuestJournal))]
        [HarmonyPatch("FindQuest")]
        [HarmonyPatch(new Type[] { typeof(string) })]
        class PatchQuestJournalFindQuest
        {
            static bool Prefix(QuestJournal __instance, ref Quest __result, ref string questName)
            {
                // TODO : Complete a quest, read again so its active again, you can keep re-learning!!! The bug is now because there is 2 quests in your list, one active and once completed.
                // Need to check all instances of FindQuest method and see what we can /cannot return to fix this issue
                for (int i = 0; i < __instance.quests.Count; i++)
                {
                    if (__instance.quests[i].ID.ToLower() == questName.ToLower())
                    {
                        __result = __instance.quests[i];
                        return false;
                    }
                }

                __result = null;
                return false;
            }
        }

        [HarmonyPatch(typeof(QuestJournal))]
        [HarmonyPatch("FindNonSharedQuest")]
        [HarmonyPatch(new Type[] { typeof(string) })]
        class PatchQuestJournalFindNonSharedQuest
        {
            static bool Prefix(QuestJournal __instance, ref Quest __result, ref string questName)
            {
                for (int i = 0; i < __instance.quests.Count; i++)
                {
                    if (__instance.quests[i].ID == questName.ToLower() && __instance.quests[i].SharedOwnerID == -1)
                    {
                        __result = __instance.quests[i];
                        return false;
                    }
                }
                __result = null;
                return false;
            }
        }

        [HarmonyPatch(typeof(QuestJournal))]
        [HarmonyPatch("FindLatestNonSharedQuest")]
        [HarmonyPatch(new Type[] { typeof(string) })]
        class PatchQuestJournalFindLatestNonSharedQuest
        {
            static bool Prefix(QuestJournal __instance, ref Quest __result, ref string questName)
            {
                Quest quest = null;
                for (int i = 0; i < __instance.quests.Count; i++)
                {
                    if (__instance.quests[i].ID == questName && (quest == null || __instance.quests[i].Active || __instance.quests[i].FinishTime > quest.FinishTime) && __instance.quests[i].SharedOwnerID == -1)
                    {
                        quest = __instance.quests[i];
                    }
                }

                __result = quest;
                return false;
            }
        }
    }
}
