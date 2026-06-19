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
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Cards.Uncommon;

public class Molt() : WarframeModCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DynamicVar("Decrement", 1),
		new CardsVar(3)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		Creature self = base.Owner.Creature;
		var powersToDecrement = self.Powers
			.Where(p => p.GetType() == typeof(HardenedShellPower) || p.StackType == PowerStackType.Counter && p.Type == PowerType.Debuff)
			.ToList();

		var powersToIncrement = self.Powers
			.Where(p => p.StackType == PowerStackType.Counter && p.Type == PowerType.Buff && p.Amount < 0)
			.ToList();
		
		foreach (PowerModel power in powersToDecrement)
		{
			await PowerCmd.ModifyAmount(choiceContext, power, -base.DynamicVars["Decrement"].BaseValue, self, this);
		}
		foreach (PowerModel power in powersToIncrement)
		{
			await PowerCmd.ModifyAmount(choiceContext, power, base.DynamicVars["Decrement"].BaseValue, self, this);
		}

        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}