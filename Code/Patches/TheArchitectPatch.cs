using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;

[HarmonyPatch(typeof(TheArchitect))]
public static class TheArchitectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("WinRun")]
    static bool WinRunPatch(TheArchitect __instance)
    {
        if (__instance.Owner == null || __instance.Owner.Character.Id.ToString() == "CHARACTER.WARFRAMEMOD-EXCALIBUR")
        {
            RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
            return false;
        }
        return true;
    }
}