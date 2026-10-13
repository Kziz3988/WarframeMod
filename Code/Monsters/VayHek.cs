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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace WarframeMod.Code.Monsters;

public sealed class VayHek : CustomMonsterModel
{
    private static IReadOnlyList<MonsterModel>? _spawnableMinions;
    
    public override string? CustomVisualPath => "vay_hek.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 220, 210);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 230, 220);

    private int HekCannonDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 22, 20);
    
    private int HekBarrageDamage => 3;

    private int HekBarrageRepeat => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
    
    private int DroneLaunchDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 15);

    private int BuffStrengthGain => 1;

    private int EnergyLose => -1;

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

    private static IReadOnlyList<MonsterModel> SpawnableMinions
    {
        get
        {
            if (_spawnableMinions == null)
            {
                _spawnableMinions = [
                    ModelDb.Monster<OrbitalStrikeDrone>(),
                    ModelDb.Monster<PropagandaDrone>()
                ];
            }
            return _spawnableMinions;
        }
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState droneLaunchMove = new MoveState("DRONE_LAUNCH_MOVE", DroneLaunchMove, new SingleAttackIntent(DroneLaunchDamage), new SummonIntent());
        MoveState propagandaMove = new MoveState("PROPAGANDA_MOVE", PropagandaMove, new BuffIntent(), new DebuffIntent());
        MoveState hekCannonMove = new MoveState("HEK_CANNON_MOVE", HekCannonMove, new SingleAttackIntent(HekCannonDamage));
		MoveState hekBarrageMove = new MoveState("HEK_BARRAGE_MOVE", HekBarrageMove, new MultiAttackIntent(HekBarrageDamage, HekBarrageRepeat));
		ConditionalBranchState droneBranch = new ConditionalBranchState("DRONE_BRANCH");
        ConditionalBranchState hekBranch = new ConditionalBranchState("HEK_BRANCH");
        droneLaunchMove.FollowUpState = hekBranch;
        propagandaMove.FollowUpState = droneBranch;
        hekCannonMove.FollowUpState = propagandaMove;
        hekBarrageMove.FollowUpState = propagandaMove;
        droneBranch.AddState(droneLaunchMove, () => CountMinions() <= 0);
        droneBranch.AddState(propagandaMove, () => CountMinions() > 0);
        hekBranch.AddState(hekCannonMove, () => base.Creature.CurrentHp > base.Creature.MaxHp / 2);
        hekBranch.AddState(hekBarrageMove, () => base.Creature.CurrentHp <= base.Creature.MaxHp / 2);
		list.Add(hekCannonMove);
        list.Add(hekBarrageMove);
		list.Add(droneLaunchMove);
        list.Add(propagandaMove);
        list.Add(droneBranch);
		list.Add(hekBranch);
		return new MonsterMoveStateMachine(list, droneLaunchMove);
	}

    private int CountMinions()
    {
        return base.CombatState.Enemies.Count(e => e.GetPower<MinionPower>() != null);
    }

	private async Task DroneLaunchMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup("WARFRAMEMOD-VAY_HEK.banter1");
        TalkCmd.Play(line, base.Creature, VfxColor.DarkGray);
        SfxCmd.Play("vay_hek_banter1.mp3".SFXPath());
        await DamageCmd.Attack(DroneLaunchDamage).FromMonster(this)
			.Execute(null);
        await CreatureCmd.Add((base.Rng.NextItem(SpawnableMinions) ?? SpawnableMinions[0]).ToMutable(), base.CombatState);
	}

	private async Task PropagandaMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup("WARFRAMEMOD-VAY_HEK.banter2");
        TalkCmd.Play(line, base.Creature, VfxColor.DarkGray);
        SfxCmd.Play("vay_hek_banter2.mp3".SFXPath());
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.CombatState.HittableEnemies, BuffStrengthGain, base.Creature, null);
        await PowerCmd.Apply<EnergyNextTurnPower>(new ThrowingPlayerChoiceContext(), targets, EnergyLose, base.Creature, null);
	}

    private async Task HekCannonMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup("WARFRAMEMOD-VAY_HEK.banter3");
        TalkCmd.Play(line, base.Creature, VfxColor.DarkGray);
        SfxCmd.Play("vay_hek_banter3.mp3".SFXPath());
        await DamageCmd.Attack(HekCannonDamage).FromMonster(this)
			.Execute(null);
	}

    private async Task HekBarrageMove(IReadOnlyList<Creature> targets)
	{
        LocString line = MonsterModel.L10NMonsterLookup("WARFRAMEMOD-VAY_HEK.banter4");
        TalkCmd.Play(line, base.Creature, VfxColor.DarkGray);
        SfxCmd.Play("vay_hek_banter4.mp3".SFXPath());
        await DamageCmd.Attack(HekBarrageDamage).WithHitCount(HekBarrageRepeat).FromMonster(this)
			.Execute(null);
	}
}
