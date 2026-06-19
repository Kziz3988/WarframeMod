using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Basic;

public class RadialJavelin() : WarframeModCard(1, CardType.Skill, CardRarity.Basic, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SlashPower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SlashPower>(2m)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await PowerCmd.Apply<SlashPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["SlashPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["SlashPower"].UpgradeValueBy(1m);
    }
}
