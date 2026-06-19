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
using WarframeMod.Code.Powers.Buff;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace WarframeMod.Code.Monsters;

public sealed class TuskThumper : CustomMonsterModel
{
    public override string? CustomVisualPath => "tusk_thumper.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 112, 100);

	public override int MaxInitialHp => MinInitialHp;

    private int TelescopicCannonDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 17, 15);

    private int AutocannonDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    private int AutocannonRepeat => 5;

    private int GroundpounderDamage => 5;

    private int GroundpounderRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    private int ArmorPlateHp => MinInitialHp / 4;

	private int BuffStrengthGain => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    private int DebuffWeakApply => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 3, 2);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
        ArmorPlatePower armorPlate = await PowerCmd.Apply<ArmorPlatePower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffStrengthGain, base.Creature, null);
        armorPlate.InitDamageAmount(ArmorPlateHp);
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState telescopicCannonMove = new MoveState("TELESCOPIC_CANNON_MOVE", TelescopicCannonMove, new SingleAttackIntent(TelescopicCannonDamage));
		MoveState autocannonMove = new MoveState("AUTOCANNON_MOVE", AutocannonMove, new MultiAttackIntent(AutocannonDamage, AutocannonRepeat));
		MoveState groundpounderMove = new MoveState("GROUNDPOUNDER_MOVE", GroundpounderMove, new MultiAttackIntent(GroundpounderDamage, GroundpounderRepeat), new DebuffIntent());
		telescopicCannonMove.FollowUpState = autocannonMove;
        autocannonMove.FollowUpState = groundpounderMove;
        groundpounderMove.FollowUpState = telescopicCannonMove;
        list.Add(telescopicCannonMove);
		list.Add(autocannonMove);
        list.Add(groundpounderMove);
		return new MonsterMoveStateMachine(list, telescopicCannonMove);
	}

	private async Task TelescopicCannonMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(TelescopicCannonDamage).FromMonster(this)
			.Execute(null);
	}

	private async Task AutocannonMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(AutocannonDamage).WithHitCount(AutocannonRepeat).FromMonster(this)
            .Execute(null);
	}

    private async Task GroundpounderMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(GroundpounderDamage).WithHitCount(GroundpounderRepeat).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, DebuffWeakApply, base.Creature, null);
	}
}
