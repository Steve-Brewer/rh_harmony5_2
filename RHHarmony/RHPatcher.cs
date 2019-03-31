using System;
using Harmony;
using System.Reflection;

namespace RHHarmony
{
    public static class RHPatcher
    {
        public static void Load()
        {
            Log.Out("RHPatcher: Starting Harmony Patch");
            var harmony = HarmonyInstance.Create("RHHarmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Out("RHPatcher: Harmony Patch complete");
        }

    }
}
