using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Config;
using WarframeMod.Code.Powers.Buff;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(MonsterModel))]
public static class CustomMonsterModelPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(MonsterModel.AfterAddedToRoom))]
    static void AfterAddedToRoomPatch(MonsterModel __instance)
    {
        if (__instance.Creature != null && __instance.CombatState.Players.Count > 1 && WarframeModConfig.EnableMultiplayerRebalance)
        {
            PowerCmd.Apply<AncientPower>(new ThrowingPlayerChoiceContext(), __instance.Creature, 1m, null, null);
        }
    }
}

[HarmonyPatch]
public static class MonsterModelPatch
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        var baseMethod = AccessTools.Method(typeof(MonsterModel), nameof(MonsterModel.AfterAddedToRoom));
        var assembly = Assembly.GetAssembly(typeof(MonsterModel));
        
        var monsterTypes = assembly?.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(MonsterModel).IsAssignableFrom(t));
        if (monsterTypes != null)
        {
            foreach (var type in monsterTypes)
            {
                var method = AccessTools.Method(type, nameof(MonsterModel.AfterAddedToRoom));
                if (method != null && method.DeclaringType == type)
                {
                    yield return method;
                }
            }            
        }
    }

    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase originalMethod)
    {
        var codes = new List<CodeInstruction>(instructions);
        
        bool hasBaseCall = codes.Any(code => 
            code.opcode == OpCodes.Call && 
            code.operand is MethodInfo mi && 
            mi.Name == nameof(MonsterModel.AfterAddedToRoom) &&
            mi.DeclaringType == typeof(MonsterModel)
        );

        if (!hasBaseCall)
        {
            var baseMethod = AccessTools.Method(typeof(MonsterModel), nameof(MonsterModel.AfterAddedToRoom));
            
            codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
            codes.Add(new CodeInstruction(OpCodes.Call, baseMethod));
        }

        return codes;
    }

    [HarmonyPrefix]
    static void AfterAddedToRoomPatch(MonsterModel __instance)
    {
        if (__instance.Creature != null && 
            __instance.CombatState.Players.Count > 1 && 
            WarframeModConfig.EnableMultiplayerRebalance)
        {
            PowerCmd.Apply<AncientPower>(new ThrowingPlayerChoiceContext(), __instance.Creature, 1m, null, null);
        }
    }
}