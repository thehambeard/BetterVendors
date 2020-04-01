using ModMaker;
using ModMaker.Utility;
using System;
using System.Reflection;
using UnityModManagerNet;
using static BetterVendors.VUtilities.SettingsWrapper;

namespace BetterVendors
{
#if (DEBUG)
    [EnableReloading]
#endif
    static class Main
    {
        public static LocalizationManager<DefaultLanguage> Local;
        public static ModManager<Core, Settings> Mod;
        public static MenuManager MenuMan;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            //HarmonyInstance.DEBUG = true;
            Local = new LocalizationManager<DefaultLanguage>();
            Mod = new ModManager<Core, Settings>();
            MenuMan = new MenuManager();
            modEntry.OnToggle = OnToggle;
#if (DEBUG)
            modEntry.OnUnload = Unload;
            return true;
        }

        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            Mod.Disable(modEntry, true);
            MenuMan = null;
            Mod = null;
            Local = null;
            return true;
        }
#else
            return true;
        }
#endif
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            try
            {
                if (value)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    Mod.Enable(modEntry, assembly);
                    MenuMan.Enable(modEntry, assembly);
                    Local.Enable(modEntry);
                    ModPath = modEntry.Path;
                }
                else
                {
                    MenuMan.Disable(modEntry);
                    Mod.Disable(modEntry, false);
                    Local.Disable(modEntry);
                    ReflectionCache.Clear();
                }
            }
            catch (Exception ex)
            {
                Mod.Error(ex.Message + ex.StackTrace);
            }
            return true;
        }
    }
}
