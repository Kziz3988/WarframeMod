using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Extensions;

[HarmonyPatch(typeof(TheArchitect))]
public static class TheArchitectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("WinRun")]
    static bool WinRunPatch(TheArchitect __instance)
    {
        if (__instance.Owner == null || __instance.Owner.Character.IsWarframe())
        {
            RunManager.Instance.WinRun();
            //RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
            return false;
        }
        return true;
    }
}