using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Harmony12;
using Kingmaker;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UI.Vendor;
using Kingmaker.UI.Common;
using Kingmaker.UI.GenericSlot;
using ModMaker;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static BetterVendors.Main;
using static BetterVendors.VUtilities.SettingsWrapper;
using Kingmaker.EntitySystem.Entities;

namespace BetterVendors.Vendor
{
    internal class VendorTrashController : IModEventHandler, ISlotsController, IAreaLoadingStagesHandler
    {
        public int Priority => 600;

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            EventBus.Unsubscribe(this);
        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            if (TrashColor.Equals(Color.clear))
                TrashColor = new Color(1f, 0f, 0f, .2f);
            if (ScrollColor.Equals(Color.clear))
                ScrollColor = new Color(0f, 1f, 0f, 1f);
            EventBus.Subscribe(this);
        }

        public void HandleSlotClick(Kingmaker.UI.ServiceWindow.ItemSlot slot)
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
           
            if (control && ToggleVendorTrash && Mod.Enabled)
            {
                if (!slot.Item.IsNonRemovable && VendorTrashItems.Contains(slot.Item.Blueprint.AssetGuid) && slot.Index != -1)
                {
                    VendorTrashItems.Remove(slot.Item.Blueprint.AssetGuid);
                    Mod.Debug(string.Format("{0} removed from trash list.", slot.Item.Blueprint.Name));
                    
                    foreach (ItemTypicalSlot itp in slot.ParentGroup.Slots)
                    {
                        HighlightItemSlotHelper.HighlightSlots(itp);
                    }
                }
                else
                {
                    if (!slot.Item.IsNonRemovable && !VendorInject.invalidItems.Contains(slot.Item.Blueprint.AssetGuid) && slot.Index != -1)
                    {
                        VendorTrashItems.Add(slot.Item.Blueprint.AssetGuid);
                        Mod.Debug(string.Format("{0} added to trash list.", slot.Item.Blueprint.Name));
                        foreach(ItemTypicalSlot itp in slot.ParentGroup.Slots)
                        {
                            HighlightItemSlotHelper.HighlightSlots(itp);
                        }
                    }
                }
            }
        }

        public void HandleSlotDoubleClick(Kingmaker.UI.ServiceWindow.ItemSlot slot) { }

        public void HandleSlotDragEnd(Kingmaker.UI.ServiceWindow.ItemSlot from, Kingmaker.UI.ServiceWindow.ItemSlot to) { }

        public void HandleSlotDragStart(Kingmaker.UI.ServiceWindow.ItemSlot slot) { }

        public void HandleSlotDrop(Kingmaker.UI.ServiceWindow.ItemSlot slot) { }

        public void HandleSlotHoverEnd(Kingmaker.UI.ServiceWindow.ItemSlot slot) { }

        public void HandleSlotHoverStart(Kingmaker.UI.ServiceWindow.ItemSlot slot) { }

        public void HandleSlotSplit(Kingmaker.UI.ServiceWindow.ItemSlot from, Kingmaker.UI.ServiceWindow.ItemSlot to, int count = 0) { }

        public void HandleSlotsSorted(SlotsGroup slotsGroup) { }

        public void OnAreaLoadingComplete()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if ((Game.Instance.CurrentlyLoadedArea.AssetGuid.Equals("173c1547502bb7243ad94ef8eec980d0") ||
                Game.Instance.CurrentlyLoadedArea.AssetGuid.Equals("c39ed0e2ceb98404b811b13cb5325092"))
                && ToggleAutoSell && ToggleVendorTrash && Mod.Enabled)
            {
                Dictionary<ItemEntity, int> itemRemove = new Dictionary<ItemEntity, int>();
                foreach (ItemEntity item in Game.Instance.Player.Inventory)
                {
                    if (VendorTrashItems.Contains(item.Blueprint.AssetGuid) && item.IsInStash)
                    {
                        Game.Instance.Player.GainMoney(item.Blueprint.SellPrice * (long)item.Count);
                        itemRemove.Add(item, item.Count);
                    }
                }
                foreach (KeyValuePair<ItemEntity, int> item in itemRemove)
                {
                    Game.Instance.Player.Inventory.Remove(item.Key, item.Value);
                }
            }
        }

        public void OnAreaScenesLoaded() { }
    }

    public static class HighlightItemSlotHelper
    {
        public static void HighlightSlots(Kingmaker.UI.ServiceWindow.ItemSlot itemSlot)
        {

            if (itemSlot.Index != -1 && itemSlot.HasItem && VendorTrashItems.Contains(itemSlot.Item.Blueprint.AssetGuid) && ToggleVendorTrash && Mod.Enabled)
            {
                itemSlot.ItemImage.color = TrashColor;
            }
            else if (itemSlot.Index != -1 && itemSlot.HasItem && itemSlot.IsScroll && ToggleHighlightScrolls && Mod.Enabled)
            {
                itemSlot.ItemImage.color = ScrollColor;
            }
            else
            {
                if (itemSlot.HasItem)
                    itemSlot.ItemImage.color = Color.white;
                else
                    itemSlot.ItemImage.color = Color.clear;
            }
        }
    }

    [HarmonyPatch(typeof(VendorMassSale))]
    [HarmonyPatch("PushSale")]
    internal static class VenderMassSale_PushSale_Patch
    {
        public static bool Prefix()
        {
            if (!ToggleVendorTrash && !Mod.Enabled)
                return true;
            List<ItemEntity> listSell = new List<ItemEntity>();
            foreach (ItemEntity item in Game.Instance.Player.Inventory)
            {
                if (VendorTrashItems.Contains(item.Blueprint.AssetGuid) && item.IsInStash)
                {
                    listSell.Add(item);
                }
            }
            foreach (ItemEntity i in listSell)
            {
                Game.Instance.Vendor.AddForSell(i, i.Count);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Kingmaker.UI.ServiceWindow.ItemSlot))]
    [HarmonyPatch("CopyItem")]
    internal static class ItemSlot_CopyItem_Patch
    {
        public static void Postfix(ItemSlot __instance)
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            HighlightItemSlotHelper.HighlightSlots(__instance);
            __instance.UpdateCount();
        }
    }

    [HarmonyPatch(typeof(Kingmaker.UI.ServiceWindow.ItemSlot))]
    [HarmonyPatch("SetupEquipPossibility")]
    internal static class ItemSlot_SetupEquipPossibility_Patch
    {
        public static void Postfix(Kingmaker.UI.ServiceWindow.ItemSlot __instance)
        {
            HighlightItemSlotHelper.HighlightSlots(__instance);
        }
    }

}
