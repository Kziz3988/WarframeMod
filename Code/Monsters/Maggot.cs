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
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Monsters;

public sealed class Maggot : CustomMonsterModel
{
    private static IReadOnlyList<PowerModel>? _appliablePowers;
    
    public override string? CustomVisualPath => "maggot.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 5, 4);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 6, 5);

    private int LatchDamage => 1;

    private int DebuffApply => 1;

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Insect;

    private static IReadOnlyList<PowerModel> AppliablePowers
    {
        get
        {
            if (_appliablePowers == null)
            {
                _appliablePowers = [
                    ModelDb.Power<FrailPower>(),
                    ModelDb.Power<VulnerablePower>(),
                    ModelDb.Power<WeakPower>()
                ];
            }
            return _appliablePowers;
        }
    }
    
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
		await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState latchMove = new MoveState("LATCH_MOVE", LatchMove, new SingleAttackIntent(LatchDamage), new DebuffIntent());
		latchMove.FollowUpState = latchMove;
		list.Add(latchMove);
		return new MonsterMoveStateMachine(list, latchMove);
	}

	private async Task LatchMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(LatchDamage).FromMonster(this)
			.Execute(null);
        foreach (Creature target in targets)
        {
            await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), (base.Rng.NextItem(AppliablePowers) ?? AppliablePowers[0]).ToMutable(), target, DebuffApply, base.Creature, null);     
        }
	}
}
