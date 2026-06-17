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
using WarframeMod.Code.Powers.Debuff;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Monsters;

public sealed class Jackal : CustomMonsterModel
{
    public override string? CustomVisualPath => "jackal.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 320, 300);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 330, 310);

    private int ShockwaveDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 22, 19);

    private int PlasmaGrenadeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);

    private int PlasmaGrenadeRepeat => 3;

	private int BuffStrengthGain => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);

    private int GridWallBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 9, 5);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.ArmorBig;

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState shockwaveMove = new MoveState("SHOCKWAVE_MOVE", ShockwaveMove, new SingleAttackIntent(ShockwaveDamage), new BuffIntent());
		MoveState plasmaGrenadeMove = new MoveState("PLASMA_GRENADE_MOVE", PlasmaGrenadeMove, new MultiAttackIntent(PlasmaGrenadeDamage, PlasmaGrenadeRepeat), new DefendIntent());
		MoveState gridWallMove = new MoveState("GRID_WALL_MOVE", GridWallMove, new BuffIntent());
        shockwaveMove.FollowUpState = plasmaGrenadeMove;
        plasmaGrenadeMove.FollowUpState = gridWallMove;
        gridWallMove.FollowUpState = shockwaveMove;
		list.Add(shockwaveMove);
        list.Add(plasmaGrenadeMove);
        list.Add(gridWallMove);
		return new MonsterMoveStateMachine(list, shockwaveMove);
	}

	private async Task ShockwaveMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup("WARFRAMEMOD-JACKAL.banter1");
        TalkCmd.Play(line, base.Creature, VfxColor.Cyan);
        await DamageCmd.Attack(ShockwaveDamage).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<ElectrifiedPower>(base.Creature, 2m, base.Creature, null);
	}

	private async Task PlasmaGrenadeMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup("WARFRAMEMOD-JACKAL.banter2");
		TalkCmd.Play(line, base.Creature, VfxColor.Cyan);
		await DamageCmd.Attack(PlasmaGrenadeDamage).WithHitCount(PlasmaGrenadeRepeat).FromMonster(this)
			.Execute(null);
        await CreatureCmd.GainBlock(base.Creature, GridWallBlock, ValueProp.Move, null);
	}

    private async Task GridWallMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup("WARFRAMEMOD-JACKAL.banter3");
		TalkCmd.Play(line, base.Creature, VfxColor.Cyan);
		await PowerCmd.Apply<StrengthPower>(base.Creature, BuffStrengthGain, base.Creature, null);
	}
}
