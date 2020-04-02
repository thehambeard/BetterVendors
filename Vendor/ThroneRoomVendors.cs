using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Cheats;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.GameModes;
using Kingmaker.Localization;
using Kingmaker.PubSubSystem;
using ModMaker;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BetterVendors.Main;


namespace BetterVendors.Vendor
{
    public class ThroneRoomVendors : IModEventHandler, IAreaLoadingStagesHandler
    {

        public enum VendorSelect
        {
            Hassuf,
            Verdel,
            Zarcie,
            Arsinoe,
        }
        public int Priority => 100;
        public static Dictionary<VendorSelect, Vendor> TRVendors { get; set; } = new Dictionary<VendorSelect, Vendor>()
        {
            { VendorSelect.Hassuf, new Vendor("e0449cfcf8ad6084ebfc161fb73e9a27",  new Vector3(-5.0f, 0.6f, 7.2f), Quaternion.LookRotation(new Vector3(-0.46f, 0, -0.88f))) },
            { VendorSelect.Verdel, new Vendor("478862ab88b8ef24385cb386c1644dc2", new Vector3(-1.1f, 0.6f, 7.5f), Quaternion.LookRotation(new Vector3(0.46f, 0, -0.88f))) },
            { VendorSelect.Zarcie, new Vendor("dbd0b3fced8738247b7c87dc77ef74f6", new Vector3(0.5f, 1.6f, 7.6f), Quaternion.LookRotation(new Vector3(-0.48f, 0, -0.87f))) },
            { VendorSelect.Arsinoe, new Vendor("3c7ad1ac37ba5224b93d77dd9b6ab723", new Vector3(-7.2f, 0.6f, 8.5f), Quaternion.LookRotation(new Vector3(-0.07f, 0, -0.99f))) }
        };

        private static void SpawnVendor(VendorSelect vendor)
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            GameModeType currentMode = Game.Instance.CurrentMode;


            if (currentMode == GameModeType.Default || currentMode == GameModeType.Pause && SceneManager.GetActiveScene().name.Contains("CapitalThroneRoom"))
            {
                DespawnVendor(vendor);
                TRVendors[vendor].EntityData = Game.Instance.EntityCreator.SpawnUnit((BlueprintUnit)Utilities.GetBlueprintByGuid<BlueprintUnit>(TRVendors[vendor].Guid), TRVendors[vendor].Position, TRVendors[vendor].Rotation, Game.Instance.CurrentScene.MainState);
                TRVendors[vendor].IsSpawned = true;
                TRVendors[vendor].Enabled = true;
            }
        }

        public static void Enable(VendorSelect vendor)
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            if (!(TRVendors[vendor].Enabled))
                SpawnVendor(vendor);
        }

        public static void SetSpawnPoint(VendorSelect vendor)
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            UnitEntityData player = Game.Instance.Player.MainCharacter.Value;
            TRVendors[vendor].Position = player.Position;
            TRVendors[vendor].Rotation = Quaternion.LookRotation(player.OrientationDirection);
            SpawnVendor(vendor);
        }
        public static void DespawnVendor(VendorSelect vendor)
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (TRVendors[vendor].EntityData != null)
                TRVendors[vendor].EntityData.Destroy();
            else
            {
                foreach (UnitEntityData unit in Game.Instance.State.Units)
                {
                    if (unit.Blueprint.AssetGuid == TRVendors[vendor].Guid)
                        unit.Destroy();
                }
            }
            TRVendors[vendor].IsSpawned = false;
            TRVendors[vendor].Enabled = false;
        }
        private void CheckForVendorEntities()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            int DictSize = TRVendors.Count;
            foreach (UnitEntityData unit in Game.Instance.State.Units)
            {
                foreach (KeyValuePair<VendorSelect, Vendor> vendor in TRVendors)
                {
                    if (unit.Blueprint.AssetGuid.Contains(vendor.Value.Guid))
                    {
                        vendor.Value.EntityData = unit;
                        vendor.Value.IsSpawned = true;
                        DictSize--;
                        break;
                    }
                }
                if (DictSize == 0)
                    break;
            }
        }
        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            EventBus.Subscribe(this);
        }
        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            EventBus.Unsubscribe(this);
        }
        public void OnAreaScenesLoaded()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

        }
        public void OnAreaLoadingComplete()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (SceneManager.GetActiveScene().name.Equals("CapitalThroneRoom"))
            { 
                CheckForVendorEntities();
                foreach(VendorSelect vendor in Enum.GetValues(typeof(VendorSelect)))
                {
                    TRVendors[vendor].Name = Utilities.GetBlueprintByGuid<BlueprintUnit>(TRVendors[vendor].Guid).CharacterName;
                }
            }
        }
        public class Vendor
        {
            public string Guid { get; set; }
            public bool Enabled { get; set; }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
            public bool IsSpawned { get; set; }
            public UnitEntityData EntityData { get; set; }
            public string Name { get; set; }

            public Vendor(string guid, Vector3 pos, Quaternion rot)
            {
                Mod.Debug(MethodBase.GetCurrentMethod());
                Guid = guid;
                Enabled = false;
                Position = pos;
                IsSpawned = false;
                Rotation = rot;
            }
        }
    }
}
