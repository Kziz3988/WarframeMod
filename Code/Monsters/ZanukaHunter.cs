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
using WarframeMod.Code.Powers.Debuff;
using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace WarframeMod.Code.Monsters;

public sealed class ZanukaHunter : CustomMonsterModel
{
    public override string? CustomVisualPath => "zanuka_hunter.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 130, 120);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 140, 130);

    private int MissleStrikeDamage => 4;

    private int MissleStrikeRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);

    private int FrostBombDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 21, 19);

	private int DebuffColdApply => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 4);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.ArmorBig;

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState missileStrikeMove = new MoveState("MISSILE_STRIKE_MOVE", MissileStrikeMove, new MultiAttackIntent(MissleStrikeDamage, MissleStrikeRepeat));
		MoveState frostBombMove = new MoveState("FROST_BOMB_MOVE", FrostBombMove, new SingleAttackIntent(FrostBombDamage), new DebuffIntent());
		missileStrikeMove.FollowUpState = frostBombMove;
        frostBombMove.FollowUpState = missileStrikeMove;
		list.Add(missileStrikeMove);
        list.Add(frostBombMove);
		return new MonsterMoveStateMachine(list, missileStrikeMove);
	}

	private async Task MissileStrikeMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(MissleStrikeDamage).WithHitCount(MissleStrikeRepeat).FromMonster(this)
			.Execute(null);
	}

	private async Task FrostBombMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(FrostBombDamage).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<ColdPower>(new ThrowingPlayerChoiceContext(), targets, DebuffColdApply, base.Creature, null);
	}
}
