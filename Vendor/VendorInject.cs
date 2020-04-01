using Harmony12;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Loot;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace BetterVendors.Vendor
{
    class VendorInject
    {
        public static readonly Dictionary<string, string> VendorTableIds = new Dictionary<string, string>
        {
            {"Hassuf", "8c17a31b6a9a6eb4cbb668902e9edcb1"},
            {"Verdel", "7de959347266092448d8a72089ef9778"},
            {"Arsinoe","afa2c7f292b8e1c4d9c835f0e8047dd3"},
            {"Oleg", "f720440559fc00949900bfa1575196ac" },
            {"Dragon (Campaign)", "b9aa51df9966dd74287f52c2f7f4780a" },
            {"Dragon (Standalone)", "08e090bb2038e3d47be56d8752d5dcaf" }
        };

        public static readonly List<string> invalidItems = new List<string>
        {
            "ad834fc7ac6ac2d4180e07f3f50d36e2",
            "48a8934b35abf4b448e8db36a3b9b49d",
            "749d6f028bd41724597ce1f2ec8dcf68",
            "8d8e61ba5006ffe4b83dd5e501e8fe41",
            "724ef74ced3f39649bbfd7affe340f45",
            "7349eb6b255251f41be0dc9431b0bdc5",
            "2c4910d557da3ad48ad28b9fe625949f",
            "decf5257e111ffd489db3eecafcf1814",
            "0650857100500ba4485d05a1a8108f44",
            "bae2b2a9b5666f543b09711e60a4f763",
            "fd2c8cbaf3e6b37458d9b123fbc5d582",
            "6e6a6896966fce04e966cb57e3e05536",
            "4297c9c8839414a4e903bf1ff3ad5694",
            "4ca9981171c90db47852bd07efcd226b",
            "0f00208b435be3947a8b069249abe361",
            "4a2c8b0cace24674caeb9f28709543ca",
            "7415ead250354de4490b353918bfb7d9",
            "c5d4962385e0e9c439ab935d83361947",
            "f6773bcb9aebb2a4b87216404840e353",
            "b60252a8ae028ba498340199f48ead67",
            "fb379e61500421143b52c739823b4082",
            "daad77fb90218914eaa68c24068c946c",
            "dea2c61ad3503864db6189db96ddef65",
            "2e3280bf21ec832418f51bee5136ec7a"
        };

        public static readonly List<string> invalidSubTypes = new List<string>
        {
            "Claw",
            "Bite",
            "Unarmed Strike"
        };

        public static readonly List<string> validItemTypes = new List<string>
        {
            "BlueprintItemWeapon",
            "BlueprintItemShield",
            "BlueprintItemArmor",
            "BlueprintItemEquipmentRing",
            "BlueprintItemEquipmentBelt",
            "BlueprintItemEquipmentFeet",
            "BlueprintItemEquipmentGloves",
            "BlueprintItemEquipmentHead",
            "BlueprintItemEquipmentNeck",
            "BlueprintItemEquipmentShoulders",
            "BlueprintItemEquipmentWrist",
            "BlueprintItemEquipmentHandSimple",
            "BlueprintItemKey",
            "BlueprintItem",
            "BlueprintItemNote",
            "BlueprintItemEquipmentUsable",
            "BlueprintItemEquipmentSimple"
        };

        public static Dictionary<string, string> SearchItems(string item)
        {
            Dictionary<string, string> searchResults = new Dictionary<string, string>();
            List<BlueprintScriptableObject> blueprints = ResourcesLibrary.LibraryObject.GetAllBlueprints();
            if (item != "" && blueprints != null)
            {
                for (int i = 0; i < blueprints.Count; i++)
                {
                    BlueprintScriptableObject blueprintScriptableObject = blueprints[i];
                    if (validItemTypes.Contains(blueprintScriptableObject.GetType().Name))
                    {
                        BlueprintItem blueprintItem = blueprintScriptableObject as BlueprintItem;
                        if (((blueprintItem.Name.IndexOf(item, StringComparison.InvariantCultureIgnoreCase) >= 0) ||
                            (blueprintItem.SubtypeName.IndexOf(item, StringComparison.InvariantCultureIgnoreCase) >= 0)) &&
                            !(blueprintItem.AssetGuid.IndexOf("#CraftMagicItems", StringComparison.InvariantCultureIgnoreCase) >= 00))
                        {
                            if (!(invalidItems.Contains(blueprintItem.AssetGuid)) && !(invalidSubTypes.Contains(blueprintItem.SubtypeName)))
                                searchResults.Add(blueprintItem.AssetGuid, blueprintItem.Name);
                        }
                    }
                }
            }
            return searchResults;
        }

        public static void addItemToVendor(string itemId, string vendorId)
        {
            BlueprintSharedVendorTable sharedVendorTable = ResourcesLibrary.TryGetBlueprint<BlueprintSharedVendorTable>(vendorId);
            BlueprintScriptableObject blueprintScriptableObject = ResourcesLibrary.TryGetBlueprint<BlueprintScriptableObject>(itemId);
            BlueprintUnitLoot blueprintUnitLoot = ScriptableObject.CreateInstance<BlueprintUnitLoot>();
            LootItemsPackFixed lootItemsPackFixed = ScriptableObject.CreateInstance<LootItemsPackFixed>();
            LootItem lootItem = new LootItem();
            Traverse.Create(lootItem).Field("m_Item").SetValue(blueprintScriptableObject as BlueprintItem);
            Traverse.Create(lootItem).Field("m_Type").SetValue(LootItemType.Item);
            Traverse.Create(lootItemsPackFixed).Field("m_Item").SetValue(lootItem);
            Traverse.Create(lootItemsPackFixed).Field("m_Count").SetValue(1);
            blueprintUnitLoot.ComponentsArray = new BlueprintComponent[] { lootItemsPackFixed };
            AddItemsToVendor add = ScriptableObject.CreateInstance<AddItemsToVendor>();
            add.SharedVendor = sharedVendorTable;
            add.Loot = blueprintUnitLoot.GenerateItems();
            add.RunAction();
        }
    }
}