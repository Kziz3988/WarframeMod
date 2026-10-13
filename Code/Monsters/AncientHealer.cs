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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace WarframeMod.Code.Monsters;

public sealed class AncientHealer : CustomMonsterModel
{
    public override string? CustomVisualPath => "ancient_healer.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 57, 52);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 60, 55);

    private int GrapplingHookDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private int GrapplingHookRepeat => 2;

    private int DebuffWeakApply => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);
    
    private int HealingPulseHeal => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 20, 15);
    
    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState grapplingHookMove = new MoveState("GRAPPLING_HOOK_MOVE", GrapplingHookMove, new MultiAttackIntent(GrapplingHookDamage, GrapplingHookRepeat), new DebuffIntent());
		MoveState healingPulseMove = new MoveState("HEALING_PULSE_MOVE", HealingPulseMove, new HealIntent());
		MoveState purifyPulseMove = new MoveState("PURIFY_PULSE_MOVE", PurifyPulseMove, new UnknownIntent());
		grapplingHookMove.FollowUpState = healingPulseMove;
        healingPulseMove.FollowUpState = purifyPulseMove;
        purifyPulseMove.FollowUpState = grapplingHookMove;
        list.Add(grapplingHookMove);
        list.Add(healingPulseMove);
        list.Add(purifyPulseMove);
		return new MonsterMoveStateMachine(list, grapplingHookMove);
	}

	private async Task GrapplingHookMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(GrapplingHookDamage).WithHitCount(GrapplingHookRepeat).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, DebuffWeakApply, this.Creature, null);
	}

	private async Task HealingPulseMove(IReadOnlyList<Creature> targets)
	{
		foreach (Creature monster in base.CombatState.HittableEnemies)
        {
            await CreatureCmd.Heal(monster, HealingPulseHeal * base.CombatState.Players.Count);
        }
	}

    private async Task PurifyPulseMove(IReadOnlyList<Creature> targets)
	{
        List<PowerModel> debuffs = [];
		foreach (Creature monster in base.CombatState.HittableEnemies)
        {
            debuffs.Clear();
            foreach (PowerModel power in monster.Powers)
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
        }
	}
}
