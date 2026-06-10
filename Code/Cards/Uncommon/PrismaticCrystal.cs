using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Uncommon;

public class PrismaticCrystal() : WarframeModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ColdPower>(),
        HoverTipFactory.FromPower<ElectricityPower>(),
        HoverTipFactory.FromPower<HeatPower>(),
        HoverTipFactory.FromPower<PoisonPower>()
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ColdPower>(1m),
        new PowerVar<ElectricityPower>(1m),
        new PowerVar<HeatPower>(1m),
        new PowerVar<PoisonPower>(1m),
	];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ColdPower>(cardPlay.Target, base.DynamicVars["ColdPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<ElectricityPower>(cardPlay.Target, base.DynamicVars["ElectricityPower"].BaseValue, base.Owner.Creature, this);
		await PowerCmd.Apply<HeatPower>(cardPlay.Target, base.DynamicVars["HeatPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<PoisonPower>(cardPlay.Target, base.DynamicVars["PoisonPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ColdPower"].UpgradeValueBy(1m);
        base.DynamicVars["ElectricityPower"].UpgradeValueBy(1m);
        base.DynamicVars["HeatPower"].UpgradeValueBy(1m);
        base.DynamicVars["PoisonPower"].UpgradeValueBy(1m);
    }
}
