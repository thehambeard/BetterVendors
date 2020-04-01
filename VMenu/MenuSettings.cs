using Kingmaker.Cheats;
using Kingmaker.Blueprints.Items;
using ModMaker;
using ModMaker.Utility;
using System;
using System.IO;
using UnityEngine;
using UnityModManagerNet;
using static BetterVendors.Main;
using static BetterVendors.VUtilities.SettingsWrapper;
using GL = UnityEngine.GUILayout;
using GLO = UnityEngine.GUILayoutOption;

namespace BetterVendors.VMenu
{
    class MenuSettings : IMenuSelectablePage
    {
        public string Name => Local["Menu_Tab_Settings"];

        public int Priority => 400;

        string fileName = Local.FileName ?? "Default.json";
        string exMessage, inMessage;
        string[] files;

        GUIStyle toggleStyle;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!Mod.Enabled) return;
            if (toggleStyle == null)
                toggleStyle = new GUIStyle(GUI.skin.toggle) { wordWrap = true};
            using (new GL.VerticalScope("box"))
            {
                ToggleVendorProgression = GL.Toggle(ToggleVendorProgression, Local["Menu_Tog_VenProgress"], Array.Empty<GLO>());
            }
                using (new GL.VerticalScope("box"))
            {
                ToggleHighlightScrolls = GL.Toggle(ToggleHighlightScrolls, Local["Menu_Tog_HLScrolls"], Array.Empty<GLO>());
                using (new GUISubScope())
                    ScrollColor = OnGUIColorSlider(ScrollColor, Local["Menu_Txt_Unlearned"]);
            }
            using (new GL.VerticalScope("box"))
            {
                ToggleVendorTrash = GL.Toggle(ToggleVendorTrash, Local["Menu_Tog_VendorTrash"], toggleStyle, GL.MaxWidth(800f));
                using (new GUISubScope())
                {
                    TrashColor = OnGUIColorSlider(TrashColor, Local["Menu_Txt_VendorTrash"]);
                    ToggleAutoSell = GL.Toggle(ToggleAutoSell, Local["Menu_Tog_AutoSell"], Array.Empty<GLO>());
                    if (GL.Button(ToggleShowTrash ? Local["Menu_Tog_HideTrash"] : Local["Menu_Tog_ShowTrash"], GL.ExpandWidth(false)))
                    {
                        ToggleShowTrash = !ToggleShowTrash;
                    }
                    if (ToggleShowTrash)
                    {
                        using (new GUISubScope())
                        {
                            string remove = "";
                            foreach (string trash in VendorTrashItems)
                            {
                                using (new GL.VerticalScope("box", GL.Width(420f))) 
                                {
                                    using (new GL.HorizontalScope())
                                    {
                                        GL.Label(Utilities.GetBlueprintByGuid<BlueprintItem>(trash).Name, GL.Width(350f));
                                        if (GL.Button(Local["Menu_Btn_Remove"], GL.ExpandWidth(false)))
                                            remove = trash;
                                    }
                                }
                            }
                            if (remove != "")
                                VendorTrashItems.Remove(remove);
                        }
                    }
                }
            }
            using (new GL.VerticalScope("box"))
            {
                using (new GUISubScope())
                    OnGUILang();
            }
        }

        private string ToHtmlStringRGBA(Color color)
        {
            int r = (int)Math.Round(color.r * 255);
            int g = (int)Math.Round(color.g * 255);
            int b = (int)Math.Round(color.b * 255);
            int a = (int)Math.Round(color.a * 255);
            return (string.Format("#{0}{1}{2}{3}", r.ToString("X2"), g.ToString("X2"), b.ToString("X2"), a.ToString("X2")));
        }

        private Color OnGUIColorSlider(Color color, string name)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { fixedWidth = 70f, };
            GUIStyle slideStyle = new GUIStyle(GUI.skin.horizontalSlider) { fixedWidth = 200f };

            using (new GL.VerticalScope())
            {
                GL.Label(string.Format(Local["Menu_Txt_Color"], name, ToHtmlStringRGBA(color)), Array.Empty<GLO>());
                using (new GUISubScope())
                {
                    using (new GL.HorizontalScope())
                    {
                        GL.Label(string.Format(Local["Menu_Txt_Red"], Math.Round(color.r * 100).ToString()), labelStyle);
                        color.r = GL.HorizontalSlider(Mathf.Clamp(color.r * 100, 0, 100), 0, 100, slideStyle, new GUIStyle(GUI.skin.horizontalSliderThumb), Array.Empty<GLO>());
                    }
                    using (new GL.HorizontalScope())
                    {
                        GL.Label(string.Format(Local["Menu_Txt_Green"], Math.Round(color.g * 100).ToString()), labelStyle);
                        color.g = GL.HorizontalSlider(Mathf.Clamp(color.g * 100, 0, 100), 0, 100, slideStyle, new GUIStyle(GUI.skin.horizontalSliderThumb), Array.Empty<GLO>());
                    }
                    using (new GL.HorizontalScope())
                    {
                        GL.Label(string.Format(Local["Menu_Txt_Blue"], Math.Round(color.b * 100).ToString()), labelStyle);
                        color.b = GL.HorizontalSlider(Mathf.Clamp(color.b * 100, 0, 100), 0, 100, slideStyle, new GUIStyle(GUI.skin.horizontalSliderThumb), Array.Empty<GLO>());
                    }
                    using (new GL.HorizontalScope())
                    {
                        GL.Label(string.Format(Local["Menu_Txt_Alpha"], Math.Round(color.a * 100).ToString()), labelStyle);
                        color.a = GL.HorizontalSlider((Mathf.Clamp(color.a * 100, 0, 100)), 0, 100, slideStyle, new GUIStyle(GUI.skin.horizontalSliderThumb), Array.Empty<GLO>());
                    }
                }
                return (new Color(color.r / 100f, color.g / 100f, color.b / 100f, color.a / 100f));
            }
        }
        private void OnGUILang()
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label(string.Format(Local["Menu_Label_Language"], Local.Language));
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(Local["Menu_Label_Export_Lang"], GUILayout.ExpandWidth(false));
                    if (GUILayout.Button(Local["Menu_Btn_Export_Lang"], GUILayout.ExpandWidth(false)))
                    {
                        this.exMessage = Local.Export(fileName, e => Mod.Error(e)) ? null : string.Format(Local["Menu_Label_Export_Fail"], fileName);
                    }
                    if (!string.IsNullOrEmpty(this.exMessage))
                    {
                        GUILayout.Label(this.exMessage);
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(Local["Menu_Btn_Default_Lang"], GUILayout.ExpandWidth(false)))
                    {
                        this.inMessage = null;
                        Local.Reset();
                        LocalizationFileName = Local.FileName;
                    }
                    if (GUILayout.Button(Local["Menu_Btn_Default_Refresh_Lang"], GUILayout.ExpandWidth(false)))
                    {
                        RefreshList();
                    }
                }

                if (files != null)
                {
                    GUILayout.Label(Local["Menu_Txt_Available"]);
                    foreach (string f in files)
                    {
                        if (GUILayout.Button(Path.GetFileNameWithoutExtension(f), GUILayout.ExpandWidth(false)))
                        {
                            inMessage = Local.Import(f, e => Mod.Error(e)) ? null : string.Format(Local["Menu_Label_Import_Lang_Failed"], f);
                            LocalizationFileName = Local.FileName;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(inMessage))
                {
                    GUILayout.Label(inMessage);
                }
            }
        }
        private void RefreshList()
        {
            files = Local.GetFileNames("*.json");
        }
    }
}
