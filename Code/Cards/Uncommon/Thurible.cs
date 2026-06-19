using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Thurible() : WarframeModCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2),
        new EnergyVar("TotalEnergy", 0)
    ];

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
       if (player == base.Owner)
        {
            ThuriblePower? power = base.Owner.Creature.GetPower<ThuriblePower>();
            if (power == null)
            {
                base.DynamicVars["TotalEnergy"].BaseValue = 0;
                return Task.CompletedTask;
            }
            base.DynamicVars["TotalEnergy"].BaseValue = power.DisplayAmount;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != base.Owner)
        {
            return Task.CompletedTask;
        }
        if (card.GetType() != typeof(Thurible))
        {
            return Task.CompletedTask;
        }
        base.DynamicVars["TotalEnergy"].BaseValue = 0;
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ThuriblePower>(choiceContext, base.Owner.Creature, base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}