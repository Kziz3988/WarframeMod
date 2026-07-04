using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Cards.Uncommon;

public class Spellbind() : WarframeModCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DynamicVar("Decrement", 1)
	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<Creature> players = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
			where c != null && c.IsAlive && c.IsPlayer && c != base.Owner.Creature
			select c;
		foreach (Creature player in players)
        {
            var powersToDecrement = player.Powers
                .Where(p => p.StackType == PowerStackType.Counter && p.IsTheLessTheBetter() && ((p.Type == PowerType.Debuff && p.Amount > 0) || (p.Type == PowerType.Buff && p.Amount < 0)))
                .ToList();

            var powersToIncrement = player.Powers
                .Where(p => p.StackType == PowerStackType.Counter && p.IsTheMoreTheBetter() && ((p.Type == PowerType.Debuff && p.Amount > 0) || (p.Type == PowerType.Buff && p.Amount < 0)))
                .ToList();
            
            foreach (PowerModel power in powersToDecrement)
            {
                await PowerCmd.ModifyAmount(choiceContext, power, -base.DynamicVars["Decrement"].BaseValue, player, this);
            }
            foreach (PowerModel power in powersToIncrement)
            {
                await PowerCmd.ModifyAmount(choiceContext, power, base.DynamicVars["Decrement"].BaseValue, player, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}