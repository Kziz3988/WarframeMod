using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class MachRush() : WarframeModCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
	protected override bool HasEnergyCostX => true;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		int num = ResolveEnergyXValue();
        if (base.IsUpgraded)
        {
            num++;
        }
        await CardPileCmd.Draw(choiceContext, num, base.Owner);
		await PowerCmd.Apply<MachRushPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }
}
