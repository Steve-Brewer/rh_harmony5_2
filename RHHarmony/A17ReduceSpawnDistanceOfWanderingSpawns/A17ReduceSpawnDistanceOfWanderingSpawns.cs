using System;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace RHHarmony.A17ReduceSpawnDistanceOfWanderingSpawns
{
    public class A17ReduceSpawnDistanceOfWanderingSpawns
    {
        [HarmonyPatch(typeof(AIDirectorHordeComponent))]
        [HarmonyPatch("FindWanderingHordeTargets")]
        [HarmonyPatch(new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(List<AIDirectorPlayerState>) }, new ArgumentType[] { ArgumentType.Out, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal })]
        static class PatchAIDirectorHordeComponentFindWanderingHordeTargets
        {
            static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 92f)
                    {
                        codes[i].operand = 70f;
                    }
                }
                return codes.AsEnumerable();
            }
        }

    }
}
