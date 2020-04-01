using ModMaker;
using ModMaker.Utility;
using System.IO;
using UnityEngine;
using UnityModManagerNet;
using static TheLootChecklist.Main;
using static TheLootChecklist.Utilities.SettingsWrapper;

namespace TheLootChecklist.Menu
{
    public class Language : IMenuSelectablePage, IMenuPage
    {
        public string Name => Local["Menu Tab_Language"];

        public int Priority => 200;

        string fileName = Local.FileName ?? "Default.json";
        string exMessage, inMessage;
        string[] files;

        Language()
        {
            RefreshList();
        }
        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Mod == null || !Mod.Enabled)
                return;


            using (new GUISubScope(Local["Menu_Label_Lang_Opt"]))
                this.OnGUILang();
        }
        private void OnGUILang()
        {
            using (new GUILayout.VerticalScope("box"))
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
                    GUILayout.Label("Available Languages:");
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
