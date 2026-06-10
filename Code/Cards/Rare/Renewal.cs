using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Cards.Rare;

public class Renewal() : WarframeModCard(0, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override bool HasEnergyCostX => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new HealVar(1),
        new BlockVar(5, ValueProp.Move)
	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num = ResolveEnergyXValue();
        if (base.IsUpgraded)
		{
			num++;
		}
        IEnumerable<Creature> players = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
			where c != null && c.IsAlive && c.IsPlayer
			select c;
        for (int i = 0; i < num; i++)
        {
            foreach (Creature player in players)
            {
                await CreatureCmd.Heal(player, base.DynamicVars.Heal.BaseValue);
                await CreatureCmd.GainBlock(player, base.DynamicVars.Block, cardPlay);
            }
        }
    }
}