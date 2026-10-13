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
using WarframeMod.Code.Powers.Buff;
using MegaCrit.Sts2.Core.Models.Singleton;
using MegaCrit.Sts2.Core.Runs;

namespace WarframeMod.Code.Monsters;

public sealed class Ambulas : CustomMonsterModel
{
    public override string? CustomVisualPath => "ambulas.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 280, 270);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 295, 285);

    private int FusionThrowerDamage => 4;

    private int FusionThrowerRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private int ElectroMortarDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 8);

    private int ElectroMortarRepeat => 3;

    private int CapitalShipLaserDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 40, 35);
    
    private int InitialShield => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 40, 30);

	private int InitialShieldRecharge => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 5, 4);

    private int BuffStrengthGain => 3;

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

    public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		await PowerCmd.Apply<ShieldPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
		
		int scale = (int)(CombatState.Players.Count * MultiplayerScalingModel.GetMultiplayerScaling(CombatState.Encounter, RunManager.Instance.DebugOnlyGetState()?.CurrentActIndex ?? 0));
		await ShieldPower.ApplyShield(base.Creature, InitialShield * scale, InitialShield * scale, InitialShieldRecharge * scale, base.Creature, null);
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState capitalShipLaserMove = new MoveState("CAPITAL_SHIP_LASER_MOVE", CapitalShipLaserMove, new SingleAttackIntent(CapitalShipLaserDamage));
        MoveState electroMortarMove = new MoveState("ELECTRO_MORTAR_MOVE", ElectroMortarMove, new BuffIntent(), new MultiAttackIntent(ElectroMortarDamage, ElectroMortarRepeat));
		MoveState fusionThrowerMove = new MoveState("FUSION_THROWER_MOVE", FusionThrowerMove, new BuffIntent(), new MultiAttackIntent(FusionThrowerDamage, FusionThrowerRepeat));
		MoveState remechSummonMove = new MoveState("REMECH_SUMMON_MOVE", RemechSummonMove, new SummonIntent(), new BuffIntent());
		ConditionalBranchState summonBranch = new ConditionalBranchState("SUMMON_BRANCH");
        capitalShipLaserMove.FollowUpState = electroMortarMove;
        electroMortarMove.FollowUpState = fusionThrowerMove;
        fusionThrowerMove.FollowUpState = summonBranch;
        remechSummonMove.FollowUpState = capitalShipLaserMove;
        summonBranch.AddState(capitalShipLaserMove, () => base.Creature.CurrentHp > base.Creature.MaxHp / 2 || CountMinions() > 0);
		summonBranch.AddState(remechSummonMove, () => base.Creature.CurrentHp <= base.Creature.MaxHp / 2 && CountMinions() <= 0);
        list.Add(capitalShipLaserMove);
        list.Add(electroMortarMove);
        list.Add(fusionThrowerMove);
        list.Add(remechSummonMove);
        list.Add(summonBranch);
		return new MonsterMoveStateMachine(list, fusionThrowerMove);
	}

    private int CountMinions()
    {
        return base.CombatState.Enemies.Count(e => e.GetPower<MinionPower>() != null);
    }

	private async Task CapitalShipLaserMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(CapitalShipLaserDamage).FromMonster(this)
			.Execute(null);
	}

	private async Task ElectroMortarMove(IReadOnlyList<Creature> targets)
	{
        await PowerCmd.Apply<ElectrifiedPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        await DamageCmd.Attack(ElectroMortarDamage).WithHitCount(ElectroMortarRepeat).FromMonster(this)
			.Execute(null);
	}

    private async Task FusionThrowerMove(IReadOnlyList<Creature> targets)
	{
        await PowerCmd.Apply<ShieldChargePower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        await DamageCmd.Attack(FusionThrowerDamage).WithHitCount(FusionThrowerRepeat).FromMonster(this)
			.Execute(null);
	}

    private async Task RemechSummonMove(IReadOnlyList<Creature> targets)
	{
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffStrengthGain, base.Creature, null);
        await CreatureCmd.Add<RemechOsprey>(base.CombatState);
	}
}
