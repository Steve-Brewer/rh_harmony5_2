using System;
using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace RHHarmony.A17FixClosestPOIPopulatingPOINameInDescription
{
    public static class A17FixClosestPOIPopulatingPOINameInDescription
    {
        [HarmonyPatch(typeof(ObjectiveClosestPOIGoto))]
        [HarmonyPatch("ParseBinding")]
        [HarmonyPatch(new Type[] { typeof(string) })]
        class PatchObjectiveClosestPOIGotoParseBinding
        {
            static bool Prefix(ObjectiveClosestPOIGoto __instance, ref string __result, ref string bindingName)
            {
                string id = __instance.ID;
                string value = __instance.Value;
                if (bindingName == null || !(bindingName == "name"))
                {
                    __result = string.Empty;
                    return false;
                }
                if (!Steam.Network.IsServer)
                {
                    if (__instance.OwnerQuest.DataVariables.ContainsKey("POIName"))
                    {
                        __result = __instance.OwnerQuest.DataVariables["POIName"];
                        return false;
                    }
                }
                else
                {
                    if (__instance.OwnerQuest.DataVariables.ContainsKey("POIName"))
                    {
                        __result = __instance.OwnerQuest.DataVariables["POIName"];
                        return false;
                    }
                    if (__instance.OwnerQuest.QuestPrefab != null)
                    {
                        __result = __instance.OwnerQuest.QuestPrefab.filename;
                        return false;
                    }
                }
                __result = string.Empty;
                return false;
            }
        }


        [HarmonyPatch(typeof(ObjectiveClosestPOIGoto))]
        [HarmonyPatch("GetPosition")]
        [HarmonyPatch(new Type[] { typeof(EntityNPC), typeof(List<Vector2>), typeof(int) })]
        class PatchObjectiveClosestPOIGotoGetPosition
        {
            static bool Prefix(ObjectiveClosestPOIGoto __instance, ref Vector3 __result, ref EntityNPC ownerNPC, ref List<Vector2> usedPOILocations, ref int entityIDforQuests)
            {
                if (__instance.OwnerQuest.GetPositionData(out __instance.position, Quest.PositionDataTypes.POIPosition))
                {
                    __instance.OwnerQuest.Position = __instance.position;
                    __instance.positionSet = true;
                    __instance.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.POIPosition, __instance.icon);
                    __instance.CurrentValue = 2;
                    __result = __instance.position;
                    return false;
                }
                EntityAlive entityAlive = (!(ownerNPC == null)) ? ownerNPC : __instance.OwnerQuest.OwnerJournal.OwnerPlayer as EntityAlive;
                if (Steam.Network.IsServer)
                {
                    PrefabInstance closestPOIToWorldPos = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator().GetClosestPOIToWorldPos(__instance.OwnerQuest.QuestTags, new Vector2(entityAlive.position.x, entityAlive.position.z), -1);
                    if (closestPOIToWorldPos == null)
                    {
                        __result = Vector3.zero;
                        return false;
                    }
                    Vector2 vector = new Vector2((float)closestPOIToWorldPos.boundingBoxPosition.x + (float)closestPOIToWorldPos.boundingBoxSize.x / 2f, (float)closestPOIToWorldPos.boundingBoxPosition.z + (float)closestPOIToWorldPos.boundingBoxSize.z / 2f);
                    if (vector.x == -0.1f && vector.y == -0.1f)
                    {
                        Log.Error("ObjectiveClosestPOIGoto: No POI found.");
                        __result = Vector3.zero;
                        return false;
                    }
                    int num = (int)vector.x;
                    int num2 = (int)entityAlive.position.y;
                    int num3 = (int)vector.y;
                    if (GameManager.Instance.World.IsPositionInBounds(__instance.position))
                    {
                        __instance.FinalizePoint(new Vector3((float)closestPOIToWorldPos.boundingBoxPosition.x, (float)closestPOIToWorldPos.boundingBoxPosition.y, (float)closestPOIToWorldPos.boundingBoxPosition.z), new Vector3((float)closestPOIToWorldPos.boundingBoxSize.x, (float)closestPOIToWorldPos.boundingBoxSize.y, (float)closestPOIToWorldPos.boundingBoxSize.z));
                        __instance.position = new Vector3((float)num, (float)num2, (float)num3);
                        __instance.OwnerQuest.Position = __instance.position;
                        string text = Localization.Get(closestPOIToWorldPos.filename, string.Empty);
                        __instance.OwnerQuest.DataVariables.Add("POIName", string.IsNullOrEmpty(text) ? closestPOIToWorldPos.filename : text);
                        __result = __instance.position;
                        return false;
                    }
                }
                else
                {
                    SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(new NetPackageQuestGotoPoint(entityAlive.entityId, __instance.OwnerQuest.QuestTags, __instance.OwnerQuest.QuestCode, NetPackageQuestGotoPoint.QuestGotoTypes.Closest, __instance.OwnerQuest.QuestClass.DifficultyTier, 0, -1, 0f, 0f, 0f, -1f), false);
                    __instance.CurrentValue = 1;
                }
                __result = Vector3.zero;
                return false;
            }
        }

        [HarmonyPatch(typeof(ObjectiveRandomPOIGoto))]
        [HarmonyPatch("GetPosition")]
        [HarmonyPatch(new Type[] { typeof(EntityNPC), typeof(List<Vector2>), typeof(int) })]
        class PatchObjectiveRandomPOIGotoGetPosition
        {
            static bool Prefix(ObjectiveRandomPOIGoto __instance, ref Vector3 __result, ref EntityNPC ownerNPC, ref List<Vector2> usedPOILocations, ref int entityIDforQuests)
            {
                if (__instance.OwnerQuest.GetPositionData(out __instance.position, Quest.PositionDataTypes.POIPosition))
                {
                    __instance.OwnerQuest.Position = __instance.position;
                    Vector3 distanceOffset;
                    __instance.OwnerQuest.GetPositionData(out distanceOffset, Quest.PositionDataTypes.POISize);
                    __instance.SetDistanceOffset(distanceOffset);
                    __instance.positionSet = true;
                    __instance.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.POIPosition, __instance.icon);
                    __instance.CurrentValue = 2;
                    __result = __instance.position;
                    return false;
                }
                EntityAlive entityAlive = (!(ownerNPC == null)) ? ownerNPC : __instance.OwnerQuest.OwnerJournal.OwnerPlayer as EntityAlive;
                if (Steam.Network.IsServer)
                {
                    PrefabInstance randomPOINearWorldPos = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator().GetRandomPOINearWorldPos(new Vector2(entityAlive.position.x, entityAlive.position.z), 1000, 50000000, __instance.OwnerQuest.QuestTags, __instance.OwnerQuest.QuestClass.DifficultyTier, usedPOILocations, entityIDforQuests);
                    if (randomPOINearWorldPos != null)
                    {
                        Vector2 vector = new Vector2((float)randomPOINearWorldPos.boundingBoxPosition.x + (float)randomPOINearWorldPos.boundingBoxSize.x / 2f, (float)randomPOINearWorldPos.boundingBoxPosition.z + (float)randomPOINearWorldPos.boundingBoxSize.z / 2f);
                        if (vector.x == -0.1f && vector.y == -0.1f)
                        {
                            Log.Error("ObjectiveRandomGoto: No POI found.");
                            __result = Vector3.zero;
                            return false;
                        }
                        int num = (int)vector.x;
                        int num2 = (int)entityAlive.position.y;
                        int num3 = (int)vector.y;
                        __instance.position = new Vector3((float)num, (float)num2, (float)num3);
                        if (GameManager.Instance.World.IsPositionInBounds(__instance.position))
                        {
                            __instance.OwnerQuest.Position = __instance.position;
                            __instance.FinalizePoint(new Vector3((float)randomPOINearWorldPos.boundingBoxPosition.x, (float)randomPOINearWorldPos.boundingBoxPosition.y, (float)randomPOINearWorldPos.boundingBoxPosition.z), new Vector3((float)randomPOINearWorldPos.boundingBoxSize.x, (float)randomPOINearWorldPos.boundingBoxSize.y, (float)randomPOINearWorldPos.boundingBoxSize.z));
                            __instance.OwnerQuest.QuestPrefab = randomPOINearWorldPos;
                            string text = Localization.Get(__instance.OwnerQuest.QuestPrefab.filename, string.Empty);
                            __instance.OwnerQuest.DataVariables.Add("POIName", string.IsNullOrEmpty(text) ? __instance.OwnerQuest.QuestPrefab.filename : text);
                            if (usedPOILocations != null)
                            {
                                usedPOILocations.Add(new Vector2((float)randomPOINearWorldPos.boundingBoxPosition.x, (float)randomPOINearWorldPos.boundingBoxPosition.z));
                            }
                            __instance.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.POIPosition, __instance.icon);
                            __result = __instance.position;
                            return false;
                        }
                    }
                }
                else
                {
                    SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(new NetPackageQuestGotoPoint(entityAlive.entityId, __instance.OwnerQuest.QuestTags, __instance.OwnerQuest.QuestCode, NetPackageQuestGotoPoint.QuestGotoTypes.RandomPOI, __instance.OwnerQuest.QuestClass.DifficultyTier, 0, -1, 0f, 0f, 0f, -1f), false);
                    __instance.CurrentValue = 1;
                }
                __result = Vector3.zero;
                return false;
            }
        }

        [HarmonyPatch(typeof(Quest))]
        [HarmonyPatch("GetVariableText")]
        [HarmonyPatch(new Type[] { typeof(string), typeof(int), typeof(string) })]
        class PatchQuestGetVariableText
        {
            static bool Prefix(Quest __instance, ref string __result, string field, int index, string variableName)
            {
                int num = 0;
                if (field == "poi")
                {
                    for (int j = 0; j < __instance.Objectives.Count; j++)
                    {
                        if ((__instance.Objectives[j] is ObjectiveRandomPOIGoto || __instance.Objectives[j] is ObjectiveClosestPOIGoto) && (++num == index || index == -1))
                        {
                            __result = __instance.Objectives[j].ParseBinding(variableName);
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
