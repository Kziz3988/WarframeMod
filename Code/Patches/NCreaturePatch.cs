using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(NCreature))]
public static class NCreaturePatch
{
	[HarmonyPostfix]
    [HarmonyPatch("SetAnimationTrigger")]
	static void SetAnimationTriggerPatch(NCreature __instance, string trigger)
	{
		if (__instance.Entity == null || !__instance.Entity.IsPlayer)
		{
			return;
		}

		if (__instance.Entity.ModelId.ToString() == "CHARACTER.WARFRAMEMOD-EXCALIBUR")
			switch (trigger)
			{
				case "Hit":
					PlayAnim(__instance, "Hit", false);
					break;

				case "Attack":
					PlayAnim(__instance, "Attack", false);
					break;
				
				case "Cast":
					PlayAnim(__instance, "Cast", true);
					break;

				case "Dead":
					PlayAnim(__instance, "Dead", false);
					break;

				default:
					PlayAnim(__instance, "Idle", false);
					break;
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch("ImmediatelySetIdle")]
	static bool ImmediatelySetIdlePatch(NCreature __instance)
	{
		if (__instance.Entity != null && __instance.Entity.ModelId.ToString() == "CHARACTER.WARFRAMEMOD-EXCALIBUR")
		{
			PlayAnim(__instance, "Idle", false);
			return false;
		}
		return true;
	}

	[HarmonyPostfix]
	[HarmonyPatch("_Ready")]
	static void ReadyPatch(NCreature __instance)
	{
		if (__instance.Entity != null && __instance.Entity.ModelId.ToString() == "CHARACTER.WARFRAMEMOD-EXCALIBUR")
		{
			PlayAnim(__instance, "Idle", false);
		}
	}

	static void PlayAnim(NCreature node, string animName, bool fromEnd)
	{
		var visual = node.GetNodeOrNull<Node2D>("Excalibur");
		if (visual == null)
		{
			return;			
		}

		var anim = visual.GetNodeOrNull<AnimatedSprite2D>("Visuals");
		if (anim == null) {
			return;
		}

		anim.Frame = 0;
		anim.Play(animName, 1f, fromEnd);

		var animationFrames = anim.SpriteFrames;
		bool isLooping = animationFrames.GetAnimationLoop(animName);
		
		anim.Stop();
		anim.Disconnect("animation_finished", new Callable());
		anim.Frame = 0;
		anim.Play(animName, 1f, fromEnd);
		
		if (animName == "Dead") {
			return;
		}
		
		if (!isLooping)
		{
			anim.Connect("animation_finished",  Callable.From(() => 
			{
				if (anim.IsInsideTree() && animName != "Dead")
				{
					anim.Play("Idle");
				}
			}), (uint)GodotObject.ConnectFlags.OneShot);
		}
	}
}
