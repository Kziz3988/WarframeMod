using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(NGameOverScreen))]
public static class NGameOverScreenPatch
{
	[HarmonyPostfix]
	[HarmonyPatch("MoveCreaturesToDifferentLayerAndDisableUi")]
	public static void MoveCreaturesToDifferentLayerAndDisableUiPatch(NGameOverScreen __instance)
	{
		var _creatureContainer = __instance.GetNodeOrNull<Control>("%CreatureContainer");
		if (_creatureContainer == null || !_creatureContainer.IsInsideTree())
		{
			return;
		}

		var children = _creatureContainer.GetChildren();
		foreach(Node child in children)
		{
			if (child is Node2D visual && visual.Name.ToString().StartsWith("Excalibur"))
			{
				var anim = visual.GetNodeOrNull<AnimatedSprite2D>("Visuals");
				anim?.Play("Dead");
			}
		}
	}
}