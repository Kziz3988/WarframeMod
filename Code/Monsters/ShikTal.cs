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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Buff;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace WarframeMod.Code.Monsters;

public sealed class ShikTal : CustomMonsterModel
{
    public override string? CustomVisualPath => "shik_tal.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 43, 36);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 49, 42);

    private int MarelokDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);

    private int MarelokRepeat => 2;

    private int BlockGain => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 20, 18);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

	private LocString GetNextBanter()
	{
		int lineIndex = RunRng.MonsterAi.NextInt(1, 6);
		return MonsterModel.L10NMonsterLookup($"WARFRAMEMOD-SHIK_TAL.banter{lineIndex}");
	}

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
		await PowerCmd.Apply<RiotShieldPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != base.Creature && creature.Side == base.Creature.Side)
		{
			TalkCmd.Play(MonsterModel.L10NMonsterLookup("WARFRAMEMOD-SHIK_TAL.teammateDeathLine"), base.Creature, VfxColor.DarkGray);
		}
		return Task.CompletedTask;
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState marelokMove = new MoveState("MARELOK_MOVE", MarelokMove, new MultiAttackIntent(MarelokDamage, MarelokRepeat));
		MoveState riotShieldMove = new MoveState("RIOT_SHIELD_MOVE", RiotShieldMove, new DefendIntent());
		marelokMove.FollowUpState = riotShieldMove;
        riotShieldMove.FollowUpState = marelokMove;
		list.Add(marelokMove);
        list.Add(riotShieldMove);
		return new MonsterMoveStateMachine(list, riotShieldMove);
	}

	private async Task MarelokMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(GetNextBanter(), base.Creature, VfxColor.DarkGray);
        await DamageCmd.Attack(MarelokDamage).WithHitCount(MarelokRepeat).FromMonster(this)
			.Execute(null);
	}

	private async Task RiotShieldMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(GetNextBanter(), base.Creature, VfxColor.DarkGray);
		await CreatureCmd.GainBlock(base.Creature, BlockGain, ValueProp.Move, null);
	}
}
