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
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Monsters;

public sealed class Charger : CustomMonsterModel
{
    public override string? CustomVisualPath => "charger.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 10, 9);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 12, 11);

    private int PounceDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
		await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState pounceMove = new MoveState("POUNCE_MOVE", PounceMove, new SingleAttackIntent(PounceDamage));
		pounceMove.FollowUpState = pounceMove;
		list.Add(pounceMove);
		return new MonsterMoveStateMachine(list, pounceMove);
	}

	private async Task PounceMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(PounceDamage).FromMonster(this)
			.Execute(null);
	}
}
