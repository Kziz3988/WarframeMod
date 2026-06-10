using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using WarframeMod.Code.Powers.Buff;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Ancient;

public class Reservoirs() : WarframeModCard(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<ElectricityPower>(),
		StunIntent.GetStaticHoverTip()
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new MaxHpVar(1m),
		new PowerVar<ReservoirsPower>(1m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await CreatureCmd.GainMaxHp(base.Owner.Creature, base.DynamicVars.MaxHp.BaseValue);
		await PowerCmd.Apply<ReservoirsPower>(base.Owner.Creature, base.DynamicVars["ReservoirsPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ReservoirsPower"].UpgradeValueBy(1m);
    }
}
