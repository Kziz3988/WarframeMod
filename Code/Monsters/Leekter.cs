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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace WarframeMod.Code.Monsters;

public sealed class Leekter : CustomMonsterModel
{
    public override string? CustomVisualPath => "leekter.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 46, 40);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 51, 45);

    private int BrokkDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 16);

    private int DebuffWeakApply => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 3, 2);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

	private LocString GetNextBanter()
	{
		int lineIndex = RunRng.MonsterAi.NextInt(1, 3);
		return MonsterModel.L10NMonsterLookup($"WARFRAMEMOD-LEEKTER.banter{lineIndex}");
	}

	public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != base.Creature && creature.Side == base.Creature.Side)
		{
			TalkCmd.Play(MonsterModel.L10NMonsterLookup("WARFRAMEMOD-LEEKTER.teammateDeathLine"), base.Creature, VfxColor.DarkGray);
		}
		return Task.CompletedTask;
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState brokkMove = new MoveState("BROKK_MOVE", BrokkMove, new SingleAttackIntent(BrokkDamage));
		MoveState flashGrenadeMove = new MoveState("FLASH_GRENADE_MOVE", FlashGrenadeMove, new DebuffIntent());
		brokkMove.FollowUpState = flashGrenadeMove;
        flashGrenadeMove.FollowUpState = brokkMove;
		list.Add(brokkMove);
        list.Add(flashGrenadeMove);
		return new MonsterMoveStateMachine(list, brokkMove);
	}

	private async Task BrokkMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(GetNextBanter(), base.Creature, VfxColor.DarkGray);
        await DamageCmd.Attack(BrokkDamage).FromMonster(this)
			.Execute(null);
	}

	private async Task FlashGrenadeMove(IReadOnlyList<Creature> targets)
	{
		await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, DebuffWeakApply, base.Creature, null);
	}
}
