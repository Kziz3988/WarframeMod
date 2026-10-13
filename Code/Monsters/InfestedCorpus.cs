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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Combat;

namespace WarframeMod.Code.Monsters;

public sealed class InfestedCorpus : CustomMonsterModel
{
    private static IReadOnlyList<MonsterModel>? _spawnableMinions;
    
    public override string? CustomVisualPath => "infested_corpus.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 210, 200);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 220, 210);

    private int PoisonGrenadeDamage => 2;

    private int PoisonGrenadeRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);

	private int SpawnPodDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private int SpawnPodRepeat => 3;
    
    private int BuffStrengthGain => 1;

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;
    
    private static IReadOnlyList<MonsterModel> SpawnableMinions
    {
        get
        {
            if (_spawnableMinions == null)
            {
                _spawnableMinions = [
                    ModelDb.Monster<Charger>(),
                    ModelDb.Monster<Leaper>(),
                    ModelDb.Monster<VolatileRunner>()
                ];
            }
            return _spawnableMinions;
        }
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState poisonGrenadeMove = new MoveState("POISON_GRENADE_MOVE", PoisonGrenadeMove, new MultiAttackIntent(PoisonGrenadeDamage, PoisonGrenadeRepeat));
		MoveState spawnPodMove = new MoveState("SPAWN_POD_MOVE", SpawnPodMove, new MultiAttackIntent(SpawnPodDamage, SpawnPodRepeat), new SummonIntent());
		MoveState goUndergroundMove = new MoveState("GO_UNDERGROUND_MOVE", GoUndergroundMove, new BuffIntent());
		MoveState emergeFromGroundMove = new MoveState("EMERGE_FROM_GROUND_MOVE", EmergeFromGroundMove, new BuffIntent());
		ConditionalBranchState attackBranchState = new ConditionalBranchState("ATTACK_BRANCH");
        poisonGrenadeMove.FollowUpState = goUndergroundMove;
        spawnPodMove.FollowUpState = goUndergroundMove;
        goUndergroundMove.FollowUpState = emergeFromGroundMove;
        emergeFromGroundMove.FollowUpState = attackBranchState;
        attackBranchState.AddState(poisonGrenadeMove, () => CountMinions() > 0);
        attackBranchState.AddState(spawnPodMove, () => CountMinions() <= 0);
		list.Add(poisonGrenadeMove);
        list.Add(spawnPodMove);
        list.Add(goUndergroundMove);
        list.Add(emergeFromGroundMove);
        list.Add(attackBranchState);
		return new MonsterMoveStateMachine(list, spawnPodMove);
	}

    private int CountMinions()
    {
        return base.CombatState.Enemies.Count(e => e.GetPower<MinionPower>() != null);
    }

	private async Task PoisonGrenadeMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup($"WARFRAMEMOD-LEPHANTIS.banter{base.Rng.NextInt(1, 5)}");
        TalkCmd.Play(line, base.Creature, VfxColor.Swamp);
        await DamageCmd.Attack(PoisonGrenadeDamage).WithHitCount(PoisonGrenadeRepeat).FromMonster(this)
			.Execute(null);
	}

    private async Task SpawnPodMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup($"WARFRAMEMOD-LEPHANTIS.banter{base.Rng.NextInt(1, 5)}");
        TalkCmd.Play(line, base.Creature, VfxColor.Swamp);
        await DamageCmd.Attack(SpawnPodDamage).WithHitCount(SpawnPodRepeat).FromMonster(this)
			.Execute(null);
        for (int i = 0; i < SpawnPodRepeat; i++)
        {
            await CreatureCmd.Add((base.Rng.NextItem(SpawnableMinions) ?? SpawnableMinions[0]).ToMutable(), base.CombatState, CombatSide.Enemy, $"infested{i}");
        }
	}

	private async Task GoUndergroundMove(IReadOnlyList<Creature> targets)
	{
        await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), base.Creature, 2m, base.Creature, null);
	}

    private async Task EmergeFromGroundMove(IReadOnlyList<Creature> targets)
	{
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffStrengthGain, base.Creature, null);
        await PowerCmd.Apply<ToxicPower>(new ThrowingPlayerChoiceContext(), base.Creature, 2m, base.Creature, null);
	}
}
