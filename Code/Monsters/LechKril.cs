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
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves;

namespace WarframeMod.Code.Monsters;

public sealed class LechKril : CustomMonsterModel
{
    public override string? CustomVisualPath => "lech_kril.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 300, 290);

	public override int MaxInitialHp => MinInitialHp;

    private int GorgonDamage => 3;

    private int GorgonRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);

    private int BrokkDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 29, 26);

	private int DebuffColdApply => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    private int DebuffHeatApply => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.ArmorBig;

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState gorgonMove = new MoveState("GORGON_MOVE", GorgonMove, new MultiAttackIntent(GorgonDamage, GorgonRepeat), new DebuffIntent());
		MoveState coldBrokkMove = new MoveState("COLD_BROKK_MOVE", ColdBrokkMove, new SingleAttackIntent(BrokkDamage), new DebuffIntent());
		MoveState heatBrokkMove = new MoveState("HEAT_BROKK_MOVE", HeatBrokkMove, new SingleAttackIntent(BrokkDamage), new DebuffIntent());
		MoveState bodyArmorMove = new MoveState("BODY_ARMOR_MOVE", BodyArmorMove, new BuffIntent());
		ConditionalBranchState elementBranchState = new ConditionalBranchState("ELEMENT_BRANCH");
        RandomBranchState coldBranchState = new RandomBranchState("COLD_BRANCH");
        gorgonMove.FollowUpState = bodyArmorMove;
        coldBrokkMove.FollowUpState = bodyArmorMove;
        heatBrokkMove.FollowUpState = heatBrokkMove;
        bodyArmorMove.FollowUpState = elementBranchState;
        elementBranchState.AddState(coldBranchState, () => base.Creature.CurrentHp > base.Creature.MaxHp / 2);
        elementBranchState.AddState(heatBrokkMove, () => base.Creature.CurrentHp <= base.Creature.MaxHp / 2);
        coldBranchState.AddBranch(gorgonMove, MoveRepeatType.CanRepeatForever);
        coldBranchState.AddBranch(coldBrokkMove, MoveRepeatType.CanRepeatForever);
		list.Add(gorgonMove);
        list.Add(coldBrokkMove);
        list.Add(heatBrokkMove);
        list.Add(bodyArmorMove);
        list.Add(elementBranchState);
        list.Add(coldBranchState);
		return new MonsterMoveStateMachine(list, bodyArmorMove);
	}

	private async Task GorgonMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(MonsterModel.L10NMonsterLookup("WARFRAMEMOD-LECH_KRIL.banter4"), base.Creature, VfxColor.DarkGray);
        SfxCmd.Play("lech_kril_banter1.mp3".SFXPath());
        await DamageCmd.Attack(GorgonDamage).WithHitCount(GorgonRepeat).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<ColdPower>(new ThrowingPlayerChoiceContext(), targets, DebuffColdApply, base.Creature, null);
	}

	private async Task ColdBrokkMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(MonsterModel.L10NMonsterLookup("WARFRAMEMOD-LECH_KRIL.banter2"), base.Creature, VfxColor.DarkGray);
        SfxCmd.Play("lech_kril_banter2.mp3".SFXPath());
        await DamageCmd.Attack(BrokkDamage).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<ColdPower>(new ThrowingPlayerChoiceContext(), targets, DebuffColdApply, base.Creature, null);
	}

    private async Task HeatBrokkMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(MonsterModel.L10NMonsterLookup("WARFRAMEMOD-LECH_KRIL.banter3"), base.Creature, VfxColor.DarkGray);
        SfxCmd.Play("lech_kril_banter3.mp3".SFXPath());
        await DamageCmd.Attack(BrokkDamage).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<HeatPower>(new ThrowingPlayerChoiceContext(), targets, DebuffHeatApply, base.Creature, null);
	}

    private async Task BodyArmorMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(MonsterModel.L10NMonsterLookup("WARFRAMEMOD-LECH_KRIL.banter1"), base.Creature, VfxColor.DarkGray);
        SfxCmd.Play("lech_kril_banter4.mp3".SFXPath());
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), base.Creature, 2m, base.Creature, null);
	}
}
