using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Monsters;

public sealed class NemesRt : CustomMonsterModel
{
	private bool _hasExploded;

	public override string? CustomVisualPath => "nemes_rt.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 28, 25);

	public override int MaxInitialHp => MinInitialHp;

	private int ExplodeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 12);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

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
		await PowerCmd.Apply<ElectrifiedPower>(new ThrowingPlayerChoiceContext(), base.Creature, 2m, base.Creature, null);
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
