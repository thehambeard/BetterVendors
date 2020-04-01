using ModMaker;
using System;
using System.Reflection;
using static BetterVendors.Main;

namespace BetterVendors
{
    class Core : IModEventHandler
    {
        public int Priority => 0;

        public static void FailedToPatch(MethodBase patch)
        {
            Type type = patch.DeclaringType;
            Mod.Warning($"Failed to patch '{type.DeclaringType?.Name}.{type.Name}.{patch.Name}'");
        }

        public void HandleModDisable()
        {
        }
        public void HandleModEnable()
        {

        }
    }
}
