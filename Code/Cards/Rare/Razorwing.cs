using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using WarframeMod.Code.Cards.Token;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Rare;

public class Razorwing() : WarframeModCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{	
	protected override bool HasEnergyCostX => true;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<NewSoarPower>(),
		HoverTipFactory.FromCard<DexPixia>(base.IsUpgraded),
		HoverTipFactory.FromCard<Diwata>(base.IsUpgraded)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num = ResolveEnergyXValue();
		if (base.IsUpgraded)
		{
			num++;
		}
		if (num > 0) {
			await PowerCmd.Apply<NewSoarPower>(choiceContext, base.Owner.Creature, num, base.Owner.Creature, this);
			await Cmd.Wait(0.1f);
		}
		DexPixia dexPixia = await CreateInHand<DexPixia>(base.Owner, base.CombatState);
		if (base.IsUpgraded)
		{
			CardCmd.Upgrade(dexPixia);
		}
		await Cmd.Wait(0.1f);
		Diwata diwata = await CreateInHand<Diwata>(base.Owner, base.CombatState);
		if (base.IsUpgraded)
		{
			CardCmd.Upgrade(diwata);
		}
    }
}
