using Kingmaker;
using ModMaker;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityModManagerNet;
using static BetterVendors.Main;
using GL = UnityEngine.GUILayout;
using TRV = BetterVendors.Vendor.ThroneRoomVendors;


namespace BetterVendors.VMenu
{
    class MenuTRV : IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_TRV"];

        public int Priority => 200;
        GUIStyle buttonStyle;
        GUIStyle lableStyle;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button) { fixedWidth = 150f };
                lableStyle = new GUIStyle(GUI.skin.label) { fixedWidth = 60f };
            }

            using (new GL.VerticalScope())
            {
                if (!Mod.Enabled) return;
                if (Game.Instance.CurrentlyLoadedArea.AssetGuid.Equals("173c1547502bb7243ad94ef8eec980d0") ||
                Game.Instance.CurrentlyLoadedArea.AssetGuid.Equals("c39ed0e2ceb98404b811b13cb5325092"))
                {
                    OnGUIMenuVendor(TRV.VendorSelect.Hassuf);
                OnGUIMenuVendor(TRV.VendorSelect.Verdel);
                OnGUIMenuVendor(TRV.VendorSelect.Arsinoe);
                OnGUIMenuVendor(TRV.VendorSelect.Zarcie);
                }
                else
                {
                    GL.Label(Local["Menu_Txt_NotInThrone"]);
                }
            }
        }

        private void OnGUIMenuVendor(TRV.VendorSelect vendor)
        {
            using (new GL.HorizontalScope())
            {
                GL.Label(string.Format("{0}: ", TRV.TRVendors[vendor].Name), lableStyle, GL.ExpandWidth(false));
                if (GL.Button(Local["Menu_Btn_Enable"], buttonStyle, GL.ExpandWidth(false)))
                {
                    TRV.Enable(vendor);
                }

                if (GL.Button(Local["Menu_Btn_Disable"], buttonStyle, GL.ExpandWidth(false)))
                {
                    TRV.DespawnVendor(vendor);
                }
                if (GL.Button(Local["Menu_Btn_Spawn"], buttonStyle, GL.ExpandWidth(false)))
                {
                    Vector3 v = Game.Instance.Player.MainCharacter.Value.OrientationDirection;
                    TRV.SetSpawnPoint(vendor);
                    TRV.Enable(vendor);
                }
            }
        }
    }
}
