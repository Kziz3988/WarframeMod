using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class EnergizedMunitions() : WarframeModCard(1, CardType.Skill, CardRarity.Event, TargetType.Self)
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await PowerCmd.Apply<EnergizedMunitionsPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.AddKeyword(CardKeyword.Retain);
    }
}
