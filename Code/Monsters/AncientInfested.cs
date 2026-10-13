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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace WarframeMod.Code.Monsters;

public sealed class AncientInfested : CustomMonsterModel
{
    public override string? CustomVisualPath => "ancient_infested.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 210, 200);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 220, 210);

    private int SporeBombDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 11, 9);

    private int SporeBombRepeat => 2;

	private int BuffStrengthGain => 2;

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState sporeBombMove = new MoveState("SPORE_BOMB_MOVE", SporeBombMove, new MultiAttackIntent(SporeBombDamage, SporeBombRepeat));
		MoveState goUndergroundMove = new MoveState("GO_UNDERGROUND_MOVE", GoUndergroundMove, new BuffIntent());
		MoveState emergeFromGroundMove = new MoveState("EMERGE_FROM_GROUND_MOVE", EmergeFromGroundMove, new BuffIntent());
		sporeBombMove.FollowUpState = goUndergroundMove;
        goUndergroundMove.FollowUpState = emergeFromGroundMove;
        emergeFromGroundMove.FollowUpState = sporeBombMove;
		list.Add(sporeBombMove);
        list.Add(goUndergroundMove);
        list.Add(emergeFromGroundMove);
		return new MonsterMoveStateMachine(list, emergeFromGroundMove);
	}

	private async Task SporeBombMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup($"WARFRAMEMOD-LEPHANTIS.banter{base.Rng.NextInt(1, 5)}");
        TalkCmd.Play(line, base.Creature, VfxColor.Swamp);
        await DamageCmd.Attack(SporeBombDamage).WithHitCount(SporeBombRepeat).FromMonster(this)
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
