using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class Crystallize() : WarframeModCard(1, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<WarframeModCardPool>();
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
		new DynamicVar("Decrement", 1m)
	];

    private async Task DealDamage(Queue<(int, decimal, decimal)> queue, PlayerChoiceContext choiceContext)
    {
        (int target, decimal amount, decimal calculated) = queue.Dequeue();
        if (calculated <= 0 || target < 0 || target >= base.CombatState.HittableEnemies.Count)
        {
            return;
        }
        await DamageCmd.Attack(amount).FromCard(this).Targeting(base.CombatState.HittableEnemies[target])
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        //BFS
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        Queue<(int target, decimal amount, decimal calculated)> queue = new();
        int index = base.CombatState.HittableEnemies.IndexOf(cardPlay.Target);
        queue.Enqueue((index, base.DynamicVars.Damage.BaseValue, base.DynamicVars.Damage.PreviewValue));
        while (queue.Count > 0)
        {
            (int target, decimal amount, decimal calculated) = queue.Peek();
            await DealDamage(queue, choiceContext);
            if (calculated <= 0 || target < 0 || target >= base.CombatState.HittableEnemies.Count)
            {
                continue;
            }
            queue.Enqueue((target - 1, amount - 1, calculated - 1));
            queue.Enqueue((target + 1, amount - 1, calculated - 1));
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
