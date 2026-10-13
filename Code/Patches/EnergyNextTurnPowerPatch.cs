using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(EnergyNextTurnPower))]
public static class EnergyNextTurnPowerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(EnergyNextTurnPower), nameof(EnergyNextTurnPower.AfterEnergyReset))]
    static bool AfterEnergyResetPatch(EnergyNextTurnPower __instance, Player player, ref Task __result)
    {
        if (__instance.Amount > 0 || player != __instance.Owner.Player)
        {
            return true;
        }
        __result = LoseEnergy(__instance, player);
        return false;
    }

    static async Task LoseEnergy(EnergyNextTurnPower __instance, Player player)
    {
        await PlayerCmd.LoseEnergy(-__instance.Amount, player);
		await PowerCmd.Remove(__instance);
    }
}