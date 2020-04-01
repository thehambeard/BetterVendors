using UnityEngine.SceneManagement;
using Harmony12;
using Kingmaker;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UI.Vendor;
using ModMaker;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static BetterVendors.Main;
using static BetterVendors.VUtilities.SettingsWrapper;

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

        public void HandleSlotClick(ItemSlot slot)
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (control && ToggleVendorTrash && Mod.Enabled)
            {
                if (VendorTrashItems.Contains(slot.Item.Blueprint.AssetGuid))
                {
                    VendorTrashItems.Remove(slot.Item.Blueprint.AssetGuid);
                    Mod.Debug(string.Format("{0} removed from trash list.", slot.Item.Blueprint.Name));
                    foreach (ItemTypicalSlot itp in slot.ParentGroup.Slots)
                    {
                        if (itp.Item.Blueprint.AssetGuid.Equals(slot.Item.Blueprint.AssetGuid))
                        {
                            itp.ItemImage.color = Color.white;
                            itp.UpdateCount();
                        }
                    }
                }
                else
                {
                    Mod.Debug(!slot.Item.IsNonRemovable && !VendorInject.invalidItems.Contains(slot.Item.Blueprint.AssetGuid));
                    if (!slot.Item.IsNonRemovable && !VendorInject.invalidItems.Contains(slot.Item.Blueprint.AssetGuid))
                    {
                        VendorTrashItems.Add(slot.Item.Blueprint.AssetGuid);
                        Mod.Debug(string.Format("{0} added to trash list.", slot.Item.Blueprint.Name));

                        foreach(ItemTypicalSlot itp in slot.ParentGroup.Slots)
                        {
                            if(itp.Item.Blueprint.AssetGuid.Equals(slot.Item.Blueprint.AssetGuid))
                            {
                                itp.ItemImage.color = TrashColor;
                                itp.UpdateCount();
                            }
                        }
                    }
                }
            }
        }

        public void HandleSlotDoubleClick(ItemSlot slot) { }

        public void HandleSlotDragEnd(ItemSlot from, ItemSlot to) { }

        public void HandleSlotDragStart(ItemSlot slot) { }

        public void HandleSlotDrop(ItemSlot slot) { }

        public void HandleSlotHoverEnd(ItemSlot slot) { }

        public void HandleSlotHoverStart(ItemSlot slot) { }

        public void HandleSlotSplit(ItemSlot from, ItemSlot to, int count = 0) { }

        public void HandleSlotsSorted(SlotsGroup slotsGroup) { }

        public void OnAreaLoadingComplete()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (SceneManager.GetActiveScene().name.Equals("CapitalThroneRoom_Light") && ToggleAutoSell && ToggleVendorTrash && Mod.Enabled)
            {
                Dictionary<ItemEntity, int> itemRemove = new Dictionary<ItemEntity, int>();
                foreach (ItemEntity item in Game.Instance.Player.Inventory)
                {
                    if (VendorTrashItems.Contains(item.Blueprint.AssetGuid))
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
                if (VendorTrashItems.Contains(item.Blueprint.AssetGuid))
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

    [HarmonyPatch(typeof(ItemSlot))]
    [HarmonyPatch("SetupEquipPossibility")]
    internal static class ItemSlot_SetupEquipPossibility_Patch
    {
        public static void Postfix(ItemSlot __instance)
        {
            if (__instance.HasItem && VendorTrashItems.Contains(__instance.Item.Blueprint.AssetGuid) && ToggleVendorTrash && Mod.Enabled)
            {
                __instance.ItemImage.color = TrashColor;
            }
            else if (__instance.HasItem && __instance.IsScroll && ToggleHighlightScrolls && Mod.Enabled)
            {
                __instance.ItemImage.color = ScrollColor;
            }
            else
            {
                if (__instance.HasItem)
                    __instance.ItemImage.color = Color.white;
                else
                    __instance.ItemImage.color = Color.clear;
            }
        }
    }

}
