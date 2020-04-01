using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;
namespace BetterVendors
{
    public class Settings : UnityModManager.ModSettings
    {
        public string localizationFileName;
        public string modPath;
        public HashSet<string> garbage = new HashSet<string>();
        public Color trashColor;
        public Color scrollColor;
        public bool toggleHighlightScrolls;
        public bool toggleVendorTrash;
        public bool toggleAutoSell;
        public bool toggleVendorProgression;
        public bool toggleShowTrash;
    }
}