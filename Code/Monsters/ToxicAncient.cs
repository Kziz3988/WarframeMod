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
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Monsters;

public sealed class ToxicAncient : CustomMonsterModel
{
    public override string? CustomVisualPath => "toxic_ancient.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 67, 62);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 70, 65);

    private int GrapplingHookDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private int GrapplingHookRepeat => 2;
    
    private int DebuffWeakApply => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

    private int BuffToxicApply => 99;
    
    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState grapplingHookMove = new MoveState("GRAPPLING_HOOK_MOVE", GrapplingHookMove, new MultiAttackIntent(GrapplingHookDamage, GrapplingHookRepeat), new DebuffIntent());
		MoveState toxicPulseMove = new MoveState("TOXIC_PULSE_MOVE", ToxicPulseMove, new BuffIntent());
		grapplingHookMove.FollowUpState = grapplingHookMove;
        toxicPulseMove.FollowUpState = grapplingHookMove;
        list.Add(grapplingHookMove);
        list.Add(toxicPulseMove);
		return new MonsterMoveStateMachine(list, toxicPulseMove);
	}

	private async Task GrapplingHookMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(GrapplingHookDamage).WithHitCount(GrapplingHookRepeat).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, DebuffWeakApply, this.Creature, null);
	}

	private async Task ToxicPulseMove(IReadOnlyList<Creature> targets)
	{
		await PowerCmd.Apply<ToxicPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffToxicApply, base.Creature, null);
	}
}
