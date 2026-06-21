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
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Monsters;

public sealed class VemTabook : CustomMonsterModel
{
    public override string? CustomVisualPath => "vem_tabook.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 34, 28);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 37, 31);

    private int HekDamage => 1;

    private int HekRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 11, 10);

    private int BuffStrengthGain => 1;

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

	private LocString GetNextBanter()
	{
		int lineIndex = RunRng.MonsterAi.NextInt(1, 2);
		return MonsterModel.L10NMonsterLookup($"WARFRAMEMOD-VEM_TABOOK.banter{lineIndex}");
	}

	public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
		await PowerCmd.Apply<GrustragLeaderPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }

	public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != base.Creature && creature.Side == base.Creature.Side)
		{
			TalkCmd.Play(MonsterModel.L10NMonsterLookup("WARFRAMEMOD-VEM_TABOOK.teammateDeathLine"), base.Creature, VfxColor.DarkGray);
		}
		return Task.CompletedTask;
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState hekMove = new MoveState("HEK_MOVE", HekMove, new MultiAttackIntent(HekDamage, HekRepeat));
		MoveState leaderMove = new MoveState("LEADER_MOVE", LeaderMove, new BuffIntent());
		hekMove.FollowUpState = leaderMove;
        leaderMove.FollowUpState = hekMove;
		list.Add(hekMove);
        list.Add(leaderMove);
		return new MonsterMoveStateMachine(list, hekMove);
	}

	private async Task HekMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(HekDamage).WithHitCount(HekRepeat).FromMonster(this)
			.Execute(null);
	}

	private async Task LeaderMove(IReadOnlyList<Creature> targets)
	{
        TalkCmd.Play(GetNextBanter(), base.Creature, VfxColor.DarkGray);
		await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffStrengthGain, base.Creature, null);
	}
}
