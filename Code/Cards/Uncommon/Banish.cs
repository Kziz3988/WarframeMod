using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Banish() : WarframeModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<BanishmentPower>(),
		StunIntent.GetStaticHoverTip(),
		HoverTipFactory.FromPower<IntangiblePower>()
	];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3m, ValueProp.Move),
		new PowerVar<BanishmentPower>(1)
	];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<BanishmentPower>(cardPlay.Target, base.DynamicVars["BanishmentPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<IntangiblePower>(cardPlay.Target, base.DynamicVars["BanishmentPower"].BaseValue, base.Owner.Creature, this);
	}

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}