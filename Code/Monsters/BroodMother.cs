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
using System.Linq;
using System;

namespace WarframeMod.Code.Monsters;

public sealed class BroodMother : CustomMonsterModel
{
    public override string? CustomVisualPath => "brood_mother.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 150, 140);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 160, 150);

    private int ChargeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 16);

    private int SpawnDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);

    private int MaggotSpawnRepeat => 4;

    private int MaggotSpawnMax => 8;

	private int BuffStrengthGain => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    
    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState chargeMove = new MoveState("CHARGE_MOVE", ChargeMove, new SingleAttackIntent(ChargeDamage));
		MoveState spawnMove = new MoveState("SPAWN_MOVE", SpawnMove, new SummonIntent(), new BuffIntent());
		MoveState spawnAttackMove = new MoveState("SPAWN_ATTACK_MOVE", SpawnAttackMove, new SingleAttackIntent(SpawnDamage), new SummonIntent());
        ConditionalBranchState spawnBranchState = new ConditionalBranchState("SPAWN_BRANCH");
		chargeMove.FollowUpState = spawnBranchState;
        spawnMove.FollowUpState = spawnBranchState;
        spawnAttackMove.FollowUpState = spawnBranchState;
        spawnBranchState.AddState(chargeMove, () => CountMinions() >= MaggotSpawnMax);
		spawnBranchState.AddState(spawnMove, () => CountMinions() + MaggotSpawnRepeat <= MaggotSpawnMax && base.Creature.CurrentHp >= base.Creature.MaxHp / 2);
		spawnBranchState.AddState(spawnAttackMove, () => (CountMinions() < MaggotSpawnMax && CountMinions() + MaggotSpawnRepeat > MaggotSpawnMax) || base.Creature.CurrentHp < base.Creature.MaxHp / 2);
		list.Add(chargeMove);
        list.Add(spawnMove);
        list.Add(spawnAttackMove);
        list.Add(spawnBranchState);
		return new MonsterMoveStateMachine(list, spawnMove);
	}

    private int CountMinions()
    {
        return base.CombatState.Enemies.Count(e => e.GetPower<MinionPower>() != null);
    }

	private async Task ChargeMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(ChargeDamage).FromMonster(this)
			.Execute(null);
	}

	private async Task SpawnMove(IReadOnlyList<Creature> targets)
	{
		for (int i = 0; i < MaggotSpawnRepeat; i++)
        {
            await CreatureCmd.Add<Maggot>(base.CombatState, base.CombatState.Encounter.GetNextSlot(base.CombatState));
        }
		await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffStrengthGain, base.Creature, null);
	}

    private async Task SpawnAttackMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(SpawnDamage).FromMonster(this)
			.Execute(null);
        for (int i = 0; i < Math.Min(MaggotSpawnMax - CountMinions(), MaggotSpawnRepeat); i++)
        {
            await CreatureCmd.Add<Maggot>(base.CombatState, base.CombatState.Encounter.GetNextSlot(base.CombatState));
        }
	}
}
