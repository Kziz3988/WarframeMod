using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Powers.Buff;

public sealed class GrustragLeaderPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier == base.Owner && power.GetType() == typeof(StrengthPower) && power.Owner == base.Owner)
        {
            foreach (Creature ally in base.CombatState.GetCreaturesOnSide(base.Owner.Side))
            {
                if (ally != base.Owner)
                {
                    await PowerCmd.Apply<StrengthPower>(choiceContext, ally, amount, base.Owner, null);
                }
            }
        }
    }
}
