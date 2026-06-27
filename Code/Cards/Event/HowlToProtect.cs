using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class HowlToProtect() : WarframeModCard(0, CardType.Skill, CardRarity.Event, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<ShieldPower>()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<WeakPower>(1m),
        new DynamicVar("TotalShield", 5m),
        new DynamicVar("ShieldCapacity", 5m),
        new DynamicVar("ShieldRecharge", 1m)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WeakPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
        await ShieldPower.ApplyShield(base.Owner.Creature, base.DynamicVars["TotalShield"].IntValue, base.DynamicVars["ShieldCapacity"].IntValue, base.DynamicVars["ShieldRecharge"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Weak.UpgradeValueBy(1m);
        base.DynamicVars["TotalShield"].UpgradeValueBy(1m);
        base.DynamicVars["ShieldCapacity"].UpgradeValueBy(1m);
    }
}