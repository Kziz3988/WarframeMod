using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Cards.Basic;

[Pool(typeof(ExcaliburCardPool))]
public class RadialBlind() : WarframeModCard(2, CardType.Skill, CardRarity.Basic, TargetType.AllEnemies)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [StunIntent.GetStaticHoverTip()];
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
		{
			await CreatureCmd.Stun(hittableEnemy);
		}
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
