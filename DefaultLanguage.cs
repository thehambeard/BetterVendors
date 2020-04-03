using Kingmaker.EntitySystem.Persistence.JsonUtility;
using ModMaker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BetterVendors
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DefaultLanguage : ILanguage
    {
        [JsonProperty]
        public string Language { get; set; } = "English (Default)";

        [JsonProperty]
        public Version Version { get; set; }

        [JsonProperty]
        public string Contributors { get; set; }

        [JsonProperty]
        public string HomePage { get; set; }

        [JsonProperty]
        public Dictionary<string, string> Strings { get; set; } = new Dictionary<string, string>()
        {
            { "Menu_Tab_Settings", "Settings" },
            { "Menu_Hassuf", "Hassuf Settings" },
            { "Menu_Label_Language", "Current: {0}" },
            { "Menu_Label_Export_Lang", "Export the default language to JSON: " },
            { "Menu_Btn_Export_Lang", "Export" },
            { "Menu_Label_Import_Lang_Failed", "Import of {0} failed." },
            { "Menu_Btn_Default_Refresh_Lang", "Refresh Languages" },
            { "Menu_Btn_Default_Lang", "Reset to Default Language" },
            { "Menu_Txt_Available", "Available Languages:" },
            { "Menu_Txt_Search", "Search for item by name/type:" },
            { "Menu_Btn_Search", "Search" },
            { "Menu_Txt_VendorPick", "Who's inventory to add to:" },
            { "Menu_Txt_AddToVendor", "Add {0} to {1}'s inventory."},
            { "Menu_Txt_ItemAdded", " Item Added!" },
            { "Menu_Tab_Inject", "Vendor Inject" },
            { "Menu_Tab_TRV", "Throne Room Vendors" },
            { "Menu_Tog_AutoSell", "Autosell trash items upon entering your throne room. The items are lost to the void and are gone forever in exchange for coin." },
            { "Menu_Tog_HLScrolls", "Highlight unlearned scolls for the current character" },
            { "Menu_Txt_Unlearned", "unlearned scroll" },
            { "Menu_Tog_VendorTrash", "Vendor Trash: Enables you to ctrl click items in your inventory to set them as trash. They will move to the to sell box when you hit the offer button in the vendor screen or if you have the autosell enabled above it will sell trash items when you enter your throne room" },
            { "Menu_Txt_VendorTrash", "vendor trash" },
            { "Menu_Txt_Color", "Select {0} highlight <color={1}>color</color>:" },
            { "Menu_Txt_Red", "Red: {0}" },
            { "Menu_Txt_Green", "Green: {0}" },
            { "Menu_Txt_Blue", "Blue: {0}" },
            { "Menu_Txt_Alpha", "Alpha {0}" },
            { "Menu_Btn_Enable", "Enable" },
            { "Menu_Btn_Disable", "Disable" },
            { "Menu_Btn_Spawn", "Set Spawn" },
            { "Menu_Tog_VenProgress", "Vendor Progress: Enables the fixed vendor progress mod, it functions exactly how it used to and is based on your kingdom stats." },
            { "Menu_Txt_NotInThrone", "Not in throne room." },
            { "Menu_Btn_Remove", "Remove" },
            { "Menu_Tog_ShowTrash", "Show Trash Items" },
            { "Menu_Tog_HideTrash", "Hide Trash Items" },
            { "Menu_Btn_AddAll", "Add all search results to {0}'s inventory" },
            { "Menu_Lbl_Noresult", "No results found" }
        };

        public T Deserialize<T>(TextReader reader)
        {
            DefaultJsonSettings.Initialize();
            return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
        }

        public void Serialize<T>(TextWriter writer, T obj)
        {
            DefaultJsonSettings.Initialize();
            writer.Write(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}