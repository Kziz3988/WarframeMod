using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves.Runs;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Cards.Event;
using WarframeMod.Code.Cards.Quest;
using WarframeMod.Code.Monsters;

namespace WarframeMod.Code.Relics.Event;

public class KubrowRelic : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    private int[] _hatchedKubrows = [];

    [SavedProperty]
    public int[] HatchedKubrows
    {
        get
        {
            return _hatchedKubrows;
        }
        set
        {
            AssertMutable();
            _hatchedKubrows = value;
        }
    }

    private static List<(MonsterModel pet, CardModel card)>? _cachedKubrows;

    private static List<(MonsterModel pet, CardModel card)> Kubrows
    {
        get
        {
            _cachedKubrows ??=
            [
                (ModelDb.Monster<ChesaKubrow>(), ModelDb.Card<GnawToNeutralize>()),
                (ModelDb.Monster<HurasKubrow>(), ModelDb.Card<StalkToHunt>()),
                (ModelDb.Monster<RaksaKubrow>(), ModelDb.Card<HowlToProtect>()),
                (ModelDb.Monster<SahasaKubrow>(), ModelDb.Card<Dig>()),
                (ModelDb.Monster<SunikaKubrow>(), ModelDb.Card<UnleashedSavagery>())
            ];
            return _cachedKubrows;
        }
    }

    public override async Task AfterObtained()
	{
        List<CardModel> list = PileType.Deck.GetPile(base.Owner).Cards.Where(c => c is KubrowEgg).ToList();
		if (CombatManager.Instance.IsInProgress)
		{
			list.AddRange(base.Owner.PlayerCombatState.AllCards.Where(c => c is KubrowEgg));
		}
        Rng rng = new(base.Owner, base.Id);
		foreach (CardModel item in list)
		{
            int kubrowIdx = rng.NextInt(0, Kubrows.Count);
            var kubrow = Kubrows[kubrowIdx];
            CardModel card = base.Owner.RunState.CreateCard(kubrow.card, base.Owner);
			await WarframeModCard.TransformTo(item, card);
            SavePet(kubrowIdx);
		}
        if (CombatManager.Instance.IsInProgress)
        {
            await SummonPet();
        }
	}

    public override async Task BeforeCombatStart()
	{
		await SummonPet();
	}

    private void SavePet(int index)
    {
        List<int> kubrows = HatchedKubrows.ToList();
        kubrows.Add(index);
        HatchedKubrows = kubrows.ToArray();
    }

    private async Task SummonPet()
	{
        foreach (int idx in HatchedKubrows)
        {
            Creature pet = base.Owner.Creature.CombatState.CreateCreature(Kubrows[idx].pet.ToMutable(), base.Owner.Creature.Side, null);
            await PlayerCmd.AddPet(pet, base.Owner);
        }
	}
}