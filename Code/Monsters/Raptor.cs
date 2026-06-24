using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Monsters;

public sealed class Raptor : CustomMonsterModel
{
    private enum RaptorType
    {
        NS,
        MT,
        RV,
        Count
    }

    public override string? CustomVisualPath => "raptor.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 198, 190);

	public override int MaxInitialHp => MinInitialHp;

    private int LaserBoltDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 22, 20);

    private int EnergyMortarDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);

    private int EnergyMortarRepeat => 5;

    private int LaserBarrageDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 39, 36);

    private int BuffStrengthGain => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);

    private int BuffSoarGain => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

    public override LocString Title
	{
		get
		{
			LocString title = base.Title;
			title.Add(new StringVar("Type", CurrentType.ToString()));
			return title;
		}
	}

    public override bool ShouldDisappearFromDoom => false;
    
    private MoveState _deadState;
    
    private MoveState DeadState
	{
		get
		{
			return _deadState;
		}
		set
		{
			AssertMutable();
			_deadState = value;
		}
	}

    private RaptorType CurrentType { get; set; }
    private RaptorType NextType { get; set; }

    //Placeholder
    private void SwitchTypeVisual(RaptorType type)
    {
        NCreature? creatureNode = base.Creature.GetCreatureNode();
        if (creatureNode == null)
		{
			Log.Error($"Attempted to play animation on creature {base.Creature} but its creature node doesn't exist!");
			return;
		}
        foreach(Node child in creatureNode.GetChildren())
		{
            Log.Info($"[Raptor] Node Type: {child.GetType().Name}");
			if (child is NCreatureVisuals visual)
			{
                visual.GetNodeOrNull<Sprite2D>("Visuals").Texture = GD.Load<Texture2D>($"raptor_{type.ToString().ToLowerInvariant()}.png".MonsterTexturePath());
				break;
			}
		}
    }

    private void SwitchToNextType()
    {
        List<RaptorType> types = Enum.GetValues<RaptorType>().Cast<RaptorType>().Where(t => t != RaptorType.Count && t != NextType).ToList();
        CurrentType = NextType;
        NextType = base.Rng.NextItem(types);
        SwitchTypeVisual(CurrentType);
    }

    private async Task Revive(int baseRespawnHp)
	{
        SwitchToNextType();
		AssertMutable();
		decimal scaledHp = Creature.ScaleHpForMultiplayer(baseRespawnHp, base.CombatState.Encounter, base.CombatState.Players.Count, base.CombatState.RunState.CurrentActIndex);
		await CreatureCmd.SetMaxHp(base.Creature, scaledHp);
		await CreatureCmd.Heal(base.Creature, scaledHp);
	}

    public Task TriggerDeadState()
	{
		SetMoveImmediate(DeadState, forceTransition: true);
        return Task.CompletedTask;
	}
    
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<GravityConveyorPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1, base.Creature, null);
        NextType = RaptorType.NS;
        SwitchToNextType();
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> list = new List<MonsterState>();
        DeadState = new MoveState("RESPAWN_MOVE", RespawnMove, new HealIntent())
        {
			MustPerformOnceBeforeTransitioning = true
		};
        MoveState laserBoltMove = new MoveState("LASER_BOLT_MOVE", LaserBoltMove, new SingleAttackIntent(LaserBoltDamage), new BuffIntent());
        MoveState energyMortarMove = new MoveState("ENERGY_MORTAR_MOVE", EnergyMortarMove, new MultiAttackIntent(EnergyMortarDamage, EnergyMortarRepeat), new BuffIntent());
        MoveState nemesRtMove = new MoveState("NEMES_RT_MOVE", NemesRtMove, new SummonIntent(), new BuffIntent());
        MoveState laserBarrageMove = new MoveState("LASER_BARRAGE_MOVE", LaserBarrageMove, new SingleAttackIntent(LaserBarrageDamage), new BuffIntent());
        ConditionalBranchState raptorTypeBranchState = new ConditionalBranchState("RAPTOR_TYPE_BRANCH");

        DeadState.FollowUpState = laserBoltMove;
        laserBoltMove.FollowUpState = raptorTypeBranchState;
        energyMortarMove.FollowUpState = laserBoltMove;
        nemesRtMove.FollowUpState = laserBoltMove;
        laserBarrageMove.FollowUpState = laserBoltMove;
		raptorTypeBranchState.AddState(energyMortarMove, () => CurrentType == RaptorType.NS);
		raptorTypeBranchState.AddState(nemesRtMove, () => CurrentType == RaptorType.MT);
        raptorTypeBranchState.AddState(laserBarrageMove, () => CurrentType == RaptorType.RV);
        
        list.Add(DeadState);
        list.Add(laserBoltMove);
        list.Add(energyMortarMove);
        list.Add(nemesRtMove);
        list.Add(laserBarrageMove);
        list.Add(raptorTypeBranchState);
        return new MonsterMoveStateMachine(list, laserBoltMove);
    }

    private async Task RespawnMove(IReadOnlyList<Creature> targets)
	{
		if (base.Creature.CombatState != null)
		{
			base.Creature.GetPower<GravityConveyorPower>()?.DoRevive();
            await Revive(MinInitialHp);
		}
	}

    private async Task LaserBoltMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(LaserBoltDamage).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffStrengthGain, base.Creature, null);
	}

    private async Task EnergyMortarMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(EnergyMortarDamage).WithHitCount(EnergyMortarRepeat).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<NewSoarPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffSoarGain, base.Creature, null);
	}

    private async Task NemesRtMove(IReadOnlyList<Creature> targets)
	{
        await CreatureCmd.Add<NemesRt>(base.CombatState);
        await PowerCmd.Apply<NewSoarPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffSoarGain, base.Creature, null);
	}

    private async Task LaserBarrageMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(LaserBarrageDamage).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<NewSoarPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffSoarGain, base.Creature, null);
	}
}
