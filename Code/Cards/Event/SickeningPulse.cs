using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class SickeningPulse() : WarframeModCard(2, CardType.Skill, CardRarity.Event, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Increment", 10)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		Creature target = cardPlay.Target;

		var powersToDecrement = target.Powers
			.Where(p => p.StackType == PowerStackType.Counter && p.Type == PowerType.Buff && p.Amount < 0)
			.ToList();

		var powersToIncrement = target.Powers
			.Where(p => p.GetType() == typeof(HardenedShellPower) || p.StackType == PowerStackType.Counter && p.Type == PowerType.Debuff)
			.ToList();
		
		foreach (PowerModel power in powersToDecrement)
		{
			await PowerCmd.ModifyAmount(choiceContext, power, -base.DynamicVars["Increment"].BaseValue, target, this);
		}
		foreach (PowerModel power in powersToIncrement)
		{
			await PowerCmd.ModifyAmount(choiceContext, power, base.DynamicVars["Increment"].BaseValue, target, this);
		}
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}