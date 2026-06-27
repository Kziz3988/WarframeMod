using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using BaseLib.Abstracts;
using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using WarframeMod.Code.Powers.Buff;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace WarframeMod.Code.Monsters;

public sealed class Phorid : CustomMonsterModel
{
    public override string? CustomVisualPath => "phorid.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 220, 210);

	public override int MaxInitialHp => MinInitialHp;

    private int BuffInfestedGain => MinInitialHp / 5;

    private int ClawSwipeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 15);

    private int SonicScreamCards => 5;

    private int SpineStrikeDamage => 5;

    private int SpineStrikeRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

    public override async Task AfterAddedToRoom()
    {
		await PowerCmd.Apply<InfestedPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffInfestedGain, base.Creature, null);
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState clawSwipeMove = new MoveState("CLAW_SWIPE_MOVE", ClawSwipeMove, new SingleAttackIntent(ClawSwipeDamage));
		MoveState sonicScreamMove = new MoveState("SONIC_SCREAM_MOVE", SonicScreamMove, new BuffIntent(), new StatusIntent(SonicScreamCards));
		MoveState spineStrikeMove = new MoveState("SPINE_STRIKE_MOVE", SpineStrikeMove, new MultiAttackIntent(SpineStrikeDamage, SpineStrikeDamage));
        clawSwipeMove.FollowUpState = sonicScreamMove;
        sonicScreamMove.FollowUpState = spineStrikeMove;
        spineStrikeMove.FollowUpState = clawSwipeMove;
		list.Add(clawSwipeMove);
        list.Add(sonicScreamMove);
        list.Add(spineStrikeMove);
		return new MonsterMoveStateMachine(list, clawSwipeMove);
	}

	private async Task ClawSwipeMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(ClawSwipeDamage).FromMonster(this)
			.Execute(null);
	}

	private async Task SonicScreamMove(IReadOnlyList<Creature> targets)
	{
        await CardPileCmd.AddToCombatAndPreview<Dazed>(targets, PileType.Discard, SonicScreamCards, null);
        await PowerCmd.Apply<ToxicPower>(new ThrowingPlayerChoiceContext(), base.Creature, 2m, base.Creature, null);
	}

    private async Task SpineStrikeMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(SpineStrikeDamage).WithHitCount(SpineStrikeRepeat).FromMonster(this)
			.Execute(null);
	}
}
