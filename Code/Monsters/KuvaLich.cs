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
using MegaCrit.Sts2.Core.Models;
using System;
using WarframeMod.Code.Powers.Debuff;
using MegaCrit.Sts2.Core.MonsterMoves;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Models.Singleton;

namespace WarframeMod.Code.Monsters;

public sealed class KuvaLich : CustomMonsterModel
{
    private static IReadOnlyList<Func<IReadOnlyList<Creature>, Task>>? _offensiveMoves;

    private static IReadOnlyList<Func<IReadOnlyList<Creature>, Task>>? _defensiveMoves;
    
    private bool _isLastMoveAttack = false;
    
    public override string? CustomVisualPath => "kuva_lich.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 216, 202);

	public override int MaxInitialHp => MinInitialHp;

    private int RequiemHp => MinInitialHp / 4;

    private int IceBlastDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 25, 21);

    private int LightningBoltDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 18);

    private int MiasmaDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private int MiasmaRepeat => 3;

    private int PlasmaFlareDamage => 15;

    private int CarapaceBlockGain => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 30, 25);

    private int FrozenFortressBlockGain => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 15, 10);

    private int BuffArtifactGain => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

    private int DebuffColdApply => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

    private int DebuffHeatApply => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
    
    private int DebuffVulnerableApply => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    private int DebuffWeakApply => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

    private int DebuffDrawReductionApply => -1;

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;
    
    private IReadOnlyList<Func<IReadOnlyList<Creature>, Task>> OffensiveMoves
    {
        get
        {
            if (_offensiveMoves == null)
            {
                _offensiveMoves = [
                    IceBlastMove,
                    LightningBoltMove,
                    MiasmaMove,
                    PlasmaFlareMove
                ];
            }
            return _offensiveMoves;
        }
    }

    private IReadOnlyList<Func<IReadOnlyList<Creature>, Task>> DeffensiveMoves
    {
        get
        {
            if (_defensiveMoves == null)
            {
                _defensiveMoves = [
                    CarapaceMove,
                    CursedGroundMove,
                    FrozenFortressMove,
                    LichStompMove
                ];
            }
            return _defensiveMoves;
        }
    }

    private bool IsLastMoveAttack
    {
        get
        {
            return _isLastMoveAttack;
        }
        set
        {
            AssertMutable();
            _isLastMoveAttack = value;
        }
    }

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        KuvaLichPower power = await PowerCmd.Apply<KuvaLichPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        int scale = (int)(CombatState.Players.Count * MultiplayerScalingModel.GetMultiplayerScaling(CombatState.Encounter, RunManager.Instance.DebugOnlyGetState()?.CurrentActIndex ?? 0));
        power.InitDamageAmount(RequiemHp * scale);
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState iceBlastMove = new MoveState("ICE_BLAST_MOVE", IceBlastMove, new SingleAttackIntent(IceBlastDamage), new DebuffIntent());
		MoveState lightningBoltMove = new MoveState("LIGHTNING_BOLT_MOVE", LightningBoltMove, new SingleAttackIntent(LightningBoltDamage), new BuffIntent());
		MoveState miasmaMove = new MoveState("MIASMA_MOVE", MiasmaMove,  new MultiAttackIntent(MiasmaDamage, MiasmaRepeat), new DebuffIntent());
		MoveState plasmaFlareMove = new MoveState("PLASMA_FLARE_MOVE", PlasmaFlareMove, new SingleAttackIntent(PlasmaFlareDamage), new DebuffIntent());
		MoveState carapaceMove = new MoveState("CARAPACE_MOVE", CarapaceMove, new DefendIntent());
		MoveState cursedGroundMove = new MoveState("CURSED_GROUND_MOVE", CursedGroundMove, new BuffIntent());
		MoveState frozenFortressMove = new MoveState("FROZEN_FORTRESS_MOVE", FrozenFortressMove, new DebuffIntent(), new DefendIntent());
		MoveState lichStompMove = new MoveState("LICH_STOMP_MOVE", LichStompMove, new DebuffIntent());
		RandomBranchState attackBranch = new RandomBranchState("ATTACK_BRANCH");
        RandomBranchState defendBranch = new RandomBranchState("DEFEND_BRANCH");
        ConditionalBranchState moveBranch = new ConditionalBranchState("MOVE_BRANCH");
        iceBlastMove.FollowUpState = moveBranch;
        lightningBoltMove.FollowUpState = moveBranch;
        miasmaMove.FollowUpState = moveBranch;
        plasmaFlareMove.FollowUpState = moveBranch;
        carapaceMove.FollowUpState = moveBranch;
        cursedGroundMove.FollowUpState = moveBranch;
        frozenFortressMove.FollowUpState = moveBranch;
        lichStompMove.FollowUpState = moveBranch;
        moveBranch.AddState(attackBranch, () => !IsLastMoveAttack);
        moveBranch.AddState(defendBranch, () => IsLastMoveAttack);
        attackBranch.AddBranch(iceBlastMove, MoveRepeatType.CannotRepeat);
        attackBranch.AddBranch(lightningBoltMove, MoveRepeatType.CannotRepeat);
        attackBranch.AddBranch(miasmaMove, MoveRepeatType.CannotRepeat);
        attackBranch.AddBranch(plasmaFlareMove, MoveRepeatType.CannotRepeat);
        defendBranch.AddBranch(carapaceMove, MoveRepeatType.CannotRepeat);
        defendBranch.AddBranch(cursedGroundMove, MoveRepeatType.CannotRepeat);
        defendBranch.AddBranch(frozenFortressMove, MoveRepeatType.CannotRepeat);
        defendBranch.AddBranch(lichStompMove, MoveRepeatType.CannotRepeat);
        list.Add(iceBlastMove);
        list.Add(lightningBoltMove);
        list.Add(miasmaMove);
        list.Add(plasmaFlareMove);
        list.Add(carapaceMove);
        list.Add(cursedGroundMove);
        list.Add(frozenFortressMove);
        list.Add(lichStompMove);
        list.Add(moveBranch);
        list.Add(attackBranch);
        list.Add(defendBranch);
        return new MonsterMoveStateMachine(list, moveBranch);
	}

	private async Task IceBlastMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(IceBlastDamage).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<ColdPower>(new ThrowingPlayerChoiceContext(), targets, DebuffColdApply, base.Creature, null);
        IsLastMoveAttack = true;
	}

    private async Task LightningBoltMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(LightningBoltDamage).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<ElectrifiedPower>(new ThrowingPlayerChoiceContext(), base.Creature, 3m, base.Creature, null);
        IsLastMoveAttack = true;
	}

    private async Task MiasmaMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(MiasmaDamage).WithHitCount(MiasmaRepeat).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, DebuffVulnerableApply, base.Creature, null);
        IsLastMoveAttack = true;
	}

    private async Task PlasmaFlareMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(PlasmaFlareDamage).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<HeatPower>(new ThrowingPlayerChoiceContext(), targets, DebuffHeatApply, base.Creature, null);
        IsLastMoveAttack = true;
	}

    private async Task CarapaceMove(IReadOnlyList<Creature> targets)
	{
        await CreatureCmd.GainBlock(base.Creature, CarapaceBlockGain, ValueProp.Move, null);
        IsLastMoveAttack = false;
	}

    private async Task CursedGroundMove(IReadOnlyList<Creature> targets)
	{
        List<PowerModel> debuffs = [];
        foreach (PowerModel power in base.Creature.Powers)
        {
            if (power.Type == PowerType.Debuff)
            {
                debuffs.Add(power);
            }
        }
        foreach (PowerModel power in debuffs)
        {
            await PowerCmd.Remove(power);
        }
        await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffArtifactGain, base.Creature, null);
        IsLastMoveAttack = false;
	}

    private async Task FrozenFortressMove(IReadOnlyList<Creature> targets)
	{
        await PowerCmd.Apply<ColdPower>(new ThrowingPlayerChoiceContext(), targets, DebuffColdApply, base.Creature, null);
        await CreatureCmd.GainBlock(base.Creature, FrozenFortressBlockGain, ValueProp.Move, null);
        IsLastMoveAttack = false;
	}

    private async Task LichStompMove(IReadOnlyList<Creature> targets)
	{
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, DebuffWeakApply, base.Creature, null);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(new ThrowingPlayerChoiceContext(), targets, DebuffDrawReductionApply, base.Creature, null);
        IsLastMoveAttack = false;
	}
}
