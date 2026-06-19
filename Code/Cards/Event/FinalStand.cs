using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using WarframeMod.Code.Cards.Token;
using WarframeMod.Code.Character;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class FinalStand() : WarframeModCard(1, CardType.Power, CardRarity.Event, TargetType.Self)
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<WarframeModCardPool>();
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<AxiosJavelin>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FinalStandPower>(1m)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FinalStandPower>(choiceContext, base.Owner.Creature, base.DynamicVars["FinalStandPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}