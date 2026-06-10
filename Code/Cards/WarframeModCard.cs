using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using WarframeMod.Code.Character;
using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using System.Threading.Tasks;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Cards;

[Pool(typeof(WarframeModCardPool))]
public abstract class WarframeModCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    
    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190
    
    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public static async Task<T?> CreateInHand<T>(Player owner, CombatState combatState) 
        where T : WarframeModCard
    {
        return (await CreateInHand<T>(owner, 1, combatState)).FirstOrDefault();
    }

    public static async Task<IEnumerable<T>> CreateInHand<T>(Player owner, int count, CombatState combatState) 
        where T : WarframeModCard
    {
        if (count == 0 || CombatManager.Instance.IsOverOrEnding)
        {
            return [];
        }
        
        List<T> cards = new List<T>();
        for (int i = 0; i < count; i++)
        {
            cards.Add(combatState.CreateCard<T>(owner));
        }
        
        await CardPileCmd.AddGeneratedCardsToCombat(cards.Cast<CardModel>().ToList(), 
            PileType.Hand, addedByPlayer: true);
        return cards;
    }
}