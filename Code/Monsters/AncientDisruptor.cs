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
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Afflictions;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Monsters;

public sealed class AncientDisruptor : CustomMonsterModel
{
    public override string? CustomVisualPath => "ancient_disruptor.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 52, 50);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 55, 53);

    private int GrapplingHookDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private int GrapplingHookRepeat => 2;

    private int DebuffWeakApply => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);
    
    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState grapplingHookMove = new MoveState("GRAPPLING_HOOK_MOVE", GrapplingHookMove, new MultiAttackIntent(GrapplingHookDamage, GrapplingHookRepeat), new DebuffIntent());
		MoveState disruptionPulseMove = new MoveState("DISRUPTION_PULSE_MOVE", DisruptionPulseMove, new CardDebuffIntent());
		grapplingHookMove.FollowUpState = disruptionPulseMove;
        disruptionPulseMove.FollowUpState = grapplingHookMove;
        list.Add(grapplingHookMove);
        list.Add(disruptionPulseMove);
		return new MonsterMoveStateMachine(list, disruptionPulseMove);
	}

	private async Task GrapplingHookMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(GrapplingHookDamage).WithHitCount(GrapplingHookRepeat).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, DebuffWeakApply, this.Creature, null);
	}

	private async Task DisruptionPulseMove(IReadOnlyList<Creature> targets)
	{
		await PowerCmd.Apply<DisruptedPower>(new ThrowingPlayerChoiceContext(), targets, 1m, base.Creature, null);
	}

	public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
	{
		if (!(card.Affliction is Entangled))
		{
			modifiedCost = originalCost;
			return false;
		}
		modifiedCost = originalCost + 1m;
		return true;
	}

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature == base.Creature)
        {
            foreach (Creature target in base.CombatState.PlayerCreatures)
            {
                if (target.Player == null)
                {
                    continue;
                }
                foreach (CardModel card in PileType.Draw.GetPile(target.Player).Cards)
                {
                    if (card.Affliction is Entangled)
                    {
                        CardCmd.ClearAffliction(card);
                    }
                }
				foreach (CardModel card in PileType.Hand.GetPile(target.Player).Cards)
                {
                    if (card.Affliction is Entangled)
                    {
                        CardCmd.ClearAffliction(card);
                    }
                }
				foreach (CardModel card in PileType.Discard.GetPile(target.Player).Cards)
                {
                    if (card.Affliction is Entangled)
                    {
                        CardCmd.ClearAffliction(card);
                    }
                }
				foreach (CardModel card in PileType.Exhaust.GetPile(target.Player).Cards)
                {
                    if (card.Affliction is Entangled)
                    {
                        CardCmd.ClearAffliction(card);
                    }
                }
            }
        }
        return Task.CompletedTask;
    }
}
