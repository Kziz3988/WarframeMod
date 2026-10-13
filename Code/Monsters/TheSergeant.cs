using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Powers.Buff;
using MegaCrit.Sts2.Core.Models.Singleton;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace WarframeMod.Code.Monsters;

public sealed class TheSergeant : CustomMonsterModel
{
    public override string? CustomVisualPath => "the_sergeant.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 90, 80);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 100, 90);

    private int LankaDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 22, 19);

    private int InitialShield => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 15, 10);

	private int InitialShieldRecharge => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 3, 2);

	private int BuffInvisibleGain => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

    private int DebuffWeakApply => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

	private void PlayNextBanter()
	{
		int lineIndex = RunRng.MonsterAi.NextInt(1, 6);
		LocString banter = MonsterModel.L10NMonsterLookup($"WARFRAMEMOD-THE_SERGEANT.banter{lineIndex}");
		TalkCmd.Play(banter, base.Creature, VfxColor.Cyan);
		SfxCmd.Play($"the_sergeant_banter{lineIndex}.mp3".SFXPath());
	}

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		await PowerCmd.Apply<ShieldPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
		
		int scale = (int)(CombatState.Players.Count * MultiplayerScalingModel.GetMultiplayerScaling(CombatState.Encounter, RunManager.Instance.DebugOnlyGetState()?.CurrentActIndex ?? 0));
		await ShieldPower.ApplyShield(base.Creature, InitialShield * scale, InitialShield * scale, InitialShieldRecharge * scale, base.Creature, null);
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState lankaMove = new MoveState("LANKA_MOVE", LankaMove, new SingleAttackIntent(LankaDamage));
		MoveState flashBangMove = new MoveState("FLASH_BANG_MOVE", FlashBangMove, new DebuffIntent(), new BuffIntent());
		lankaMove.FollowUpState = flashBangMove;
        flashBangMove.FollowUpState = lankaMove;
		list.Add(lankaMove);
        list.Add(flashBangMove);
		return new MonsterMoveStateMachine(list, flashBangMove);
	}

	private async Task LankaMove(IReadOnlyList<Creature> targets)
	{
        PlayNextBanter();
        await DamageCmd.Attack(LankaDamage).FromMonster(this)
			.Execute(null);
	}

	private async Task FlashBangMove(IReadOnlyList<Creature> targets)
	{
        PlayNextBanter();
		await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, DebuffWeakApply, base.Creature, null);
		await PowerCmd.Apply<InvisiblePower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffInvisibleGain, base.Creature, null);
	}
}
