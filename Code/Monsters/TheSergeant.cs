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

namespace WarframeMod.Code.Monsters;

public sealed class TheSergeant : CustomMonsterModel
{
    public override string? CustomVisualPath => "the_sergeant.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 70, 60);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 80, 70);

    private int LankaShotDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 19, 17);

    private int InitialShield => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 15, 10);

	private int InitialShieldRecharge => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 3, 2);

	private int BuffInvisibleGain => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

    private int DebuffWeakApply => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

	private LocString GetNextBanter()
	{
		int lineIndex = RunRng.MonsterAi.NextInt(1, 5);
		return MonsterModel.L10NMonsterLookup($"WARFRAMEMOD-THE_SERGEANT.banter{lineIndex}");
	}

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		await PowerCmd.Apply<ShieldPower>(base.Creature, 1m, base.Creature, null);
		
		int scale = (int)(CombatState.Players.Count * MultiplayerScalingModel.GetMultiplayerScaling(CombatState.Encounter, RunManager.Instance.DebugOnlyGetState()?.CurrentActIndex ?? 0));
		await ShieldPower.ApplyShield(base.Creature, InitialShield * scale, InitialShield * scale, InitialShieldRecharge * scale, base.Creature, null);
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState lankaShotMove = new MoveState("LANKA_SHOT_MOVE", LankaShotMove, new SingleAttackIntent(LankaShotDamage));
		MoveState flashBangMove = new MoveState("FLASH_BANG_MOVE", FlashBangMove, new DebuffIntent(), new BuffIntent());
		lankaShotMove.FollowUpState = flashBangMove;
        flashBangMove.FollowUpState = lankaShotMove;
		list.Add(lankaShotMove);
        list.Add(flashBangMove);
		return new MonsterMoveStateMachine(list, flashBangMove);
	}

	private async Task LankaShotMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(GetNextBanter(), base.Creature, VfxColor.Cyan);
        await DamageCmd.Attack(LankaShotDamage).FromMonster(this)
			.Execute(null);
	}

	private async Task FlashBangMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(GetNextBanter(), base.Creature, VfxColor.Cyan);
		await PowerCmd.Apply<WeakPower>(targets, DebuffWeakApply, base.Creature, null);
		await PowerCmd.Apply<InvisiblePower>(base.Creature, BuffInvisibleGain, base.Creature, null);
	}
}
