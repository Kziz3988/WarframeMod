using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
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
        new DynamicVar("HeatPower", 3m),
        new DynamicVar("IsHeat", 1m)
	];

    private bool IsCurrentHeat => base.DynamicVars["IsHeat"].BaseValue == 1;

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("IsHeat", IsCurrentHeat);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsCurrentHeat)
        {
            await PowerCmd.Apply<HeatPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["HeatPower"].BaseValue, base.Owner.Creature, this);    
        }
        else
        {
            await PowerCmd.Apply<ColdPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["HeatPower"].BaseValue, base.Owner.Creature, this);
        }
		base.DynamicVars["IsHeat"].BaseValue = 1 - base.DynamicVars["IsHeat"].BaseValue;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HeatPower"].UpgradeValueBy(1m);
    }
}
