using System;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Audio;

namespace RHHarmony.A17QualityDegradationOnRepair
{
    public static class A17QualityDegradationOnRepair
    {
        [HarmonyPatch(typeof(XUiC_RecipeStack))]
        [HarmonyPatch("outputStack")]
        class PatchXUiC_RecipeStackoutputStack
        {
            static bool Prefix(XUiC_RecipeStack __instance, ref bool __result)
            {
                if (__instance.recipe == null)
                {
                    __result = false;
                    return false;
                }
                EntityPlayerLocal entityPlayer = __instance.xui.playerUI.entityPlayer;
                if (entityPlayer == null)
                {
                    __result = false;
                    return false;
                }
                ItemValue itemValue = null;
                if (__instance.originalItem == null || __instance.originalItem.Equals(ItemValue.None))
                {
                    __instance.outputItemValue = new ItemValue(__instance.recipe.itemValueType, __instance.outputQuality, __instance.outputQuality, false, default(FastTags), 1f);
                    ItemClass itemClass = __instance.outputItemValue.ItemClass;
                    if (__instance.outputItemValue == null)
                    {
                        __result = false;
                        return false;
                    }
                    if (itemClass == null)
                    {
                        __result = false;
                        return false;
                    }
                    if (entityPlayer.entityId == __instance.startingEntityId)
                    {
                        __instance.giveExp(__instance.outputItemValue, itemClass);
                    }
                    if (__instance.recipe.GetName().Equals("meleeToolStoneAxe"))
                    {
                        UserProfile user = __instance.xui.playerUI.entityPlayer.user;
                        Platform.AchievementManager.SetAchievementStat(user, EnumAchievementDataStat.StoneAxeCrafted, 1);
                    }
                    else if (__instance.recipe.GetName().Equals("woodFrameBlock"))
                    {
                        UserProfile user2 = __instance.xui.playerUI.entityPlayer.user;
                        Platform.AchievementManager.SetAchievementStat(user2, EnumAchievementDataStat.WoodFrameCrafted, 1);
                    }
                }
                else if (__instance.amountToRepair > 0)
                {
                    ItemValue itemValue2 = __instance.originalItem.Clone();
                    itemValue2.UseTimes -= __instance.amountToRepair;
                    ItemClass itemClass2 = itemValue2.ItemClass;
                    if (itemValue2.UseTimes < 0)
                    {
                        itemValue2.UseTimes = 0;
                    }
                    __instance.outputItemValue = itemValue2.Clone();
                    if (__instance.originalItem.PercentUsesLeft == 0f && __instance.originalItem.Quality <= 10)
                    {
                        __instance.outputItemValue = null;
                        Manager.BroadcastPlay(itemClass2.Properties.Values[ItemClass.PropSoundDestroy]);
                    }
                    else if (__instance.originalItem.Quality > 10)
                    {
                        float num = 10f;
                        if (GameManager.Instance != null && GameManager.Instance.World != null && entityPlayer.entityId == __instance.startingEntityId)
                        {
                            int num2 = Mathf.FloorToInt(entityPlayer.GetCVar("AS_ConstructionTools_Lvl") / 10f);
                            num -= (float)num2;
                        }
                        __instance.outputItemValue.Quality = Mathf.Max(__instance.outputItemValue.Quality - (int)num, 10);
                        if (new ItemValue(__instance.outputItemValue.type, __instance.outputItemValue.Quality, __instance.outputItemValue.Quality, false, default(FastTags), 1f).Modifications.Length < __instance.outputItemValue.Modifications.Length)
                        {
                            if (__instance.outputItemValue.Modifications[__instance.outputItemValue.Modifications.Length - 1] != null)
                            {
                                itemValue = __instance.outputItemValue.Modifications[__instance.outputItemValue.Modifications.Length - 1].Clone();
                            }
                            ItemValue[] modifications = __instance.outputItemValue.Modifications;
                            Array.Resize(ref modifications, __instance.outputItemValue.Modifications.Length - 1);
                            __instance.outputItemValue.Modifications = modifications;
                        }
                    }
                    QuestEventManager.Current.RepairedItem(__instance.outputItemValue);
                    __instance.amountToRepair = 0;
                }
                XUiC_WorkstationOutputGrid childByType = __instance.windowGroup.Controller.GetChildByType<XUiC_WorkstationOutputGrid>();
                if (childByType != null && (__instance.originalItem == null || __instance.originalItem.Equals(ItemValue.None)))
                {
                    ItemStack itemStack = new ItemStack(__instance.outputItemValue, __instance.recipe.count);
                    ItemStack[] slots = childByType.GetSlots();
                    bool flag = false;
                    for (int i = 0; i < slots.Length; i++)
                    {
                        if (slots[i].CanStackWith(itemStack))
                        {
                            slots[i].count += __instance.recipe.count;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        for (int j = 0; j < slots.Length; j++)
                        {
                            if (slots[j].IsEmpty())
                            {
                                slots[j] = itemStack;
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        childByType.SetSlots(slots);
                        childByType.UpdateData(slots);
                        childByType.IsDirty = true;
                        QuestEventManager.Current.CraftedItem(itemStack);
                        if (__instance.playSound)
                        {
                            if (__instance.recipe.craftingArea != null)
                            {
                                WorkstationData workstationData = CraftingManager.GetWorkstationData(__instance.recipe.craftingArea);
                                if (workstationData != null)
                                {
                                    Manager.PlayInsidePlayerHead(workstationData.CraftCompleteSound, -1, 0f, false, false);
                                }
                            }
                            else
                            {
                                Manager.PlayInsidePlayerHead("craft_complete_item", -1, 0f, false, false);
                            }
                        }
                    }
                    else if (!__instance.AddItemToInventory())
                    {
                        __instance.isInventoryFull = true;
                        string text = "No room in workstation output, crafting has been halted until space is cleared.";
                        if (Localization.Exists("wrnWorkstationOutputFull", string.Empty))
                        {
                            text = Localization.Get("wrnWorkstationOutputFull", string.Empty);
                        }
                        GameManager.ShowTooltip(entityPlayer, text);
                        Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
                        __result = false;
                        return false;
                    }
                }
                else
                {
                    if (!__instance.xui.dragAndDrop.ItemStack.IsEmpty() && __instance.xui.dragAndDrop.ItemStack.itemValue.ItemClass is ItemClassQuest)
                    {
                        __result = false;
                        return false;
                    }
                    ItemStack itemStack2 = new ItemStack(__instance.outputItemValue, __instance.recipe.count);
                    if (!__instance.xui.PlayerInventory.AddItemNoPartial(itemStack2, false))
                    {
                        if (itemStack2.count != __instance.recipe.count)
                        {
                            __instance.xui.PlayerInventory.DropItem(itemStack2);
                            QuestEventManager.Current.CraftedItem(itemStack2);
                            __result = true;
                            return false;
                        }
                        __instance.isInventoryFull = true;
                        string text2 = "No room in inventory, crafting has been halted until space is cleared.";
                        if (Localization.Exists("wrnInventoryFull", string.Empty))
                        {
                            text2 = Localization.Get("wrnInventoryFull", string.Empty);
                        }
                        GameManager.ShowTooltip(entityPlayer, text2);
                        Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
                        __result = false;
                        return false;
                    }
                    else
                    {
                        if (__instance.originalItem != null && !__instance.originalItem.IsEmpty())
                        {
                            if (__instance.recipe.ingredients.Count > 0)
                            {
                                QuestEventManager.Current.ScrappedItem(__instance.recipe.ingredients[0]);
                            }
                        }
                        else
                        {
                            itemStack2.count = __instance.recipe.count - itemStack2.count;
                            QuestEventManager.Current.CraftedItem(itemStack2);
                        }
                        if (__instance.playSound)
                        {
                            Manager.PlayInsidePlayerHead("craft_complete_item", -1, 0f, false, false);
                        }
                    }
                }
                if (!__instance.isInventoryFull)
                {
                    __instance.originalItem = ItemValue.None.Clone();
                }
                if (itemValue != null)
                {
                    ItemStack itemStack3 = new ItemStack(itemValue, 1);
                    if (!__instance.xui.PlayerInventory.AddItemNoPartial(itemStack3, false))
                    {
                        __instance.xui.PlayerInventory.DropItem(itemStack3);
                    }
                }
                __result = true;
                return false;
            }
        }
    }
}
