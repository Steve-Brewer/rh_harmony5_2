using System;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace RHHarmony.A17IncreaseWanderingSpawns
{
    public class A17IncreaseWanderingSpawns
    {
        [HarmonyPatch(typeof(AIWanderingHordeSpawner), MethodType.Constructor)]
        [HarmonyPatch("AIWanderingHordeSpawner")]
        [HarmonyPatch(new Type[] { typeof(World), typeof(AIWanderingHordeSpawner.HordeArrivedDelegate), typeof(List<AIDirectorPlayerState>), typeof(int), typeof(ulong), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(int), typeof(bool), })]
        static class PatchAIWanderingHordeSpawner
        {
            static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4_S)
                    {
                        codes[i].opcode = OpCodes.Ldc_I4;
                        codes[i].operand = (int)10000;
                        break;
                    }
                }
                return codes.AsEnumerable(); 
            }
        }

    }
}
