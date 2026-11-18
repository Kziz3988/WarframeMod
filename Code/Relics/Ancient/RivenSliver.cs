using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Cards.Event;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class RivenSliver : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Riven>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Unblockable | ValueProp.Unpowered)];

    public override async Task AfterObtained()
	{
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars.Damage, null, null, null);
		Riven riven = base.Owner.RunState.CreateCard<Riven>(base.Owner);
        riven.Cycle(base.Owner.PlayerRng.Rewards);
		CardCmd.PreviewCardPileAdd([(await CardPileCmd.Add(riven, PileType.Deck))], 2f);
	}
}
