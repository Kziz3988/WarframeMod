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
using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Localization;

namespace WarframeMod.Code.Monsters;

public sealed class InfestedGrineer : CustomMonsterModel
{
    public override string? CustomVisualPath => "infested_grineer.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 210, 200);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 220, 210);

    private int ScytheArmDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 37, 33);

    private int BuffStrengthGain => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState scytheArmMove = new MoveState("SCYTHE_ARM_MOVE", ScytheArmMove, new SingleAttackIntent(ScytheArmDamage));
		MoveState goUndergroundMove = new MoveState("GO_UNDERGROUND_MOVE", GoUndergroundMove, new BuffIntent());
		MoveState emergeFromGroundMove = new MoveState("EMERGE_FROM_GROUND_MOVE", EmergeFromGroundMove, new BuffIntent());
		scytheArmMove.FollowUpState = goUndergroundMove;
        goUndergroundMove.FollowUpState = emergeFromGroundMove;
        emergeFromGroundMove.FollowUpState = scytheArmMove;
        list.Add(scytheArmMove);
        list.Add(goUndergroundMove);
        list.Add(emergeFromGroundMove);
		return new MonsterMoveStateMachine(list, goUndergroundMove);
	}

	private async Task ScytheArmMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup($"WARFRAMEMOD-LEPHANTIS.banter{base.Rng.NextInt(1, 5)}");
        TalkCmd.Play(line, base.Creature, VfxColor.Swamp);
        await DamageCmd.Attack(ScytheArmDamage).FromMonster(this)
			.Execute(null);
	}

	private async Task GoUndergroundMove(IReadOnlyList<Creature> targets)
	{
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), base.Creature, 2m, base.Creature, null);
	}

    private async Task EmergeFromGroundMove(IReadOnlyList<Creature> targets)
	{
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffStrengthGain, base.Creature, null);
	}
}
