using BetterVendors.Vendor;
using ModMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using static BetterVendors.Main;
using GL = UnityEngine.GUILayout;


namespace BetterVendors.VMenu
{
    class MenuVendorInject : IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_Inject"];

        public int Priority => 300;

        string searchString = "";
        private static GUILayoutOption[] falseWidth = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
        Dictionary<string, string> results = new Dictionary<string, string>();
        private static int vendorToolbar = 0;
        string[] vendors = VendorInject.VendorTableIds.Keys.ToArray<string>();

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!Mod.Enabled) return;
            using (new GL.VerticalScope("box"))
            {
                GUILayout.Label(Local["Menu_Txt_Search"]);
                using (new GL.HorizontalScope())
                {
                    searchString = GUILayout.TextField(searchString, 30, Array.Empty<GUILayoutOption>());
                    if (GUILayout.Button(Local["Menu_Btn_Search"], falseWidth) && searchString != "")
                    {
                        results = VendorInject.SearchItems(searchString);
                    }
                }
                try
                {
                    if (results != null)
                    {
                        
                            GUILayout.Label(Local["Menu_Txt_VendorPick"], falseWidth);
                            vendorToolbar = GUILayout.Toolbar(vendorToolbar, vendors, new GUIStyle(GUI.skin.button) {wordWrap = true, fixedHeight = 50f }, new GUILayoutOption[] {GL.MaxWidth(800f)});
                        foreach (KeyValuePair<string, string> item in results.OrderBy(x => x.Value))
                        {
                            using (new GL.HorizontalScope())
                            {
                                GUILayout.TextField(item.Key, new GUIStyle(GUI.skin.textField) { fixedWidth = 250f });
                                bool flagAdd = GUILayout.Button(string.Format(Local["Menu_Txt_AddToVendor"], item.Value, vendors[vendorToolbar]), falseWidth);
                                if (flagAdd)
                                {
                                    VendorInject.addItemToVendor(item.Key, VendorInject.VendorTableIds[vendors[vendorToolbar]]);
                                    GUILayout.Label(Local["Menu_Txt_ItemAdded"], falseWidth);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Main.Mod.Error(ex.Message);
                }
            }
        }
    }
}
