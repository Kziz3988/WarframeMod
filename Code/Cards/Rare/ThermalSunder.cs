using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Rare;

public class ThermalSunder() : WarframeModCard(0, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HeatPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<ColdPower>(),
        StunIntent.GetStaticHoverTip()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<HeatPower>(5m),
		new PowerVar<ColdPower>(1m)
	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await PowerCmd.Apply<HeatPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["HeatPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<ColdPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["ColdPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HeatPower"].UpgradeValueBy(1m);
        base.DynamicVars["ColdPower"].UpgradeValueBy(5m);
    }
}
