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

public sealed class VolatileRunner : CustomMonsterModel
{
    private bool _hasExploded;
    
    public override string? CustomVisualPath => "volatile_runner.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 7, 6);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 8, 7);

    private int ExplodeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 18);

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

    private bool HasExploded
	{
		get
		{
			return _hasExploded;
		}
		set
		{
			AssertMutable();
			_hasExploded = value;
		}
	}

	public override bool ShouldFadeAfterDeath => false;
    
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
		await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState moveState = new MoveState("EXPLODE_MOVE", ExplodeMove, new DeathBlowIntent(() => ExplodeDamage));
		list.Add(moveState);
		return new MonsterMoveStateMachine(list, moveState);
	}

	private async Task ExplodeMove(IReadOnlyList<Creature> targets)
	{
        HasExploded = true;
		await DamageCmd.Attack(ExplodeDamage).FromMonster(this)
			.Execute(null);
		await CreatureCmd.Kill(base.Creature);
	}
}
