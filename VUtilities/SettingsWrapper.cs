using System.Collections.Generic;
using UnityEngine;
using static BetterVendors.Main;

namespace BetterVendors.VUtilities
{
    public static class SettingsWrapper
    {
        public static string ModPath
        {
            get => Mod.Settings.modPath;
            set => Mod.Settings.modPath = value;
        }

        public static string LocalizationFileName
        {
            get => Mod.Settings.localizationFileName;
            set => Mod.Settings.localizationFileName = value;
        }

        public static HashSet<string> VendorTrashItems
        {
            get => Mod.Settings.garbage;
            set => Mod.Settings.garbage = value;
        }

        public static Color TrashColor
        {
            get => Mod.Settings.trashColor;
            set => Mod.Settings.trashColor = value;
        }

        public static Color ScrollColor
        {
            get => Mod.Settings.scrollColor;
            set => Mod.Settings.scrollColor = value;
        }
        public static bool ToggleHighlightScrolls
        {
            get => Mod.Settings.toggleHighlightScrolls;
            set => Mod.Settings.toggleHighlightScrolls = value;
        }
        public static bool ToggleVendorTrash
        {
            get => Mod.Settings.toggleVendorTrash;
            set => Mod.Settings.toggleVendorTrash = value;
        }

        public static bool ToggleAutoSell
        {
            get => Mod.Settings.toggleAutoSell;
            set => Mod.Settings.toggleAutoSell = value;
        }

        public static bool ToggleVendorProgression
        {
            get => Mod.Settings.toggleVendorProgression;
            set => Mod.Settings.toggleVendorProgression = value;
        }

        public static bool ToggleShowTrash
        {
            get => Mod.Settings.toggleShowTrash;
            set => Mod.Settings.toggleShowTrash = value;
        }
    }
}