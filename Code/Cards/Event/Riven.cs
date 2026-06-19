using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Character;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Powers;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class Riven() : WarframeModCard(1, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
{
    private int _isUnveiled = -1;
    private int _damage;
    private int _repeat;
    private int _isTargettingAllEnemies;
    private int _draw;
    private int _add;
    private int _heal;
    private int _targetVulnerablePower;
    private int _targetWeakPower;
    private int _coldPower;
    private int _electricityPower;
    private int _heatPower;
    private int _poisonPower;
    private int _slashPower;
    private int _dazedCount;
    private int _debrisCount;
    private int _slimedCount;
    private int _woundCount;
    private int _selfFrailPower;
    private int _selfVulnerablePower;
    private int _selfWeakPower;

    [SavedProperty]
    public int IsUnveiled
    {
        get
		{
			return _isUnveiled;
		}
		set
		{
			AssertMutable();
			_isUnveiled = value;
            base.DynamicVars["IsUnveiled"].BaseValue = _isUnveiled;
		}
    }

    [SavedProperty]
    public int Damage
    {
        get
		{
			return _damage;
		}
		set
		{
			AssertMutable();
			_damage = value;
            base.DynamicVars.Damage.BaseValue = _damage;
		}
    }

    [SavedProperty]
    public int Repeat
    {
        get
		{
			return _repeat;
		}
		set
		{
			AssertMutable();
			_repeat = value;
            base.DynamicVars.Repeat.BaseValue = _repeat;
		}
    }

    [SavedProperty]
    public int IsTargettingAllEnemies
    {
        get
		{
			return _isTargettingAllEnemies;
		}
		set
		{
			AssertMutable();
			_isTargettingAllEnemies = value;
            base.DynamicVars["IsTargettingAllEnemies"].BaseValue = _isTargettingAllEnemies;
		}
    }

    [SavedProperty]
    public int Draw
    {
        get
		{
			return _draw;
		}
		set
		{
			AssertMutable();
			_draw = value;
            base.DynamicVars["Draw"].BaseValue = _draw;
		}
    }

    [SavedProperty]
    public int Add
    {
        get
		{
			return _add;
		}
		set
		{
			AssertMutable();
			_add = value;
            base.DynamicVars["Add"].BaseValue = _add;
		}
    }

    [SavedProperty]
    public int Heal
    {
        get
		{
			return _heal;
		}
		set
		{
			AssertMutable();
			_heal = value;
            base.DynamicVars["Heal"].BaseValue = _heal;
		}
    }

    [SavedProperty]
    public int TargetVulnerablePower
    {
        get
		{
			return _targetVulnerablePower;
		}
		set
		{
			AssertMutable();
			_targetVulnerablePower = value;
            base.DynamicVars["TargetVulnerablePower"].BaseValue = _targetVulnerablePower;
		}
    }

    [SavedProperty]
    public int TargetWeakPower
    {
        get
		{
			return _targetWeakPower;
		}
		set
		{
			AssertMutable();
			_targetWeakPower = value;
            base.DynamicVars["TargetWeakPower"].BaseValue = _targetWeakPower;
		}
    }

    [SavedProperty]
    public int ColdPower
    {
        get
		{
			return _coldPower;
		}
		set
		{
			AssertMutable();
			_coldPower = value;
            base.DynamicVars["ColdPower"].BaseValue = _coldPower;
		}
    }

    [SavedProperty]
    public int ElectricityPower
    {
        get
		{
			return _electricityPower;
		}
		set
		{
			AssertMutable();
			_electricityPower = value;
            base.DynamicVars["ElectricityPower"].BaseValue = _electricityPower;
		}
    }

    [SavedProperty]
    public int HeatPower
    {
        get
		{
			return _heatPower;
		}
		set
		{
			AssertMutable();
			_heatPower = value;
            base.DynamicVars["HeatPower"].BaseValue = _heatPower;
		}
    }

    [SavedProperty]
    public int PoisonPower
    {
        get
		{
			return _poisonPower;
		}
		set
		{
			AssertMutable();
			_poisonPower = value;
            base.DynamicVars["PoisonPower"].BaseValue = _poisonPower;
		}
    }

    [SavedProperty]
    public int SlashPower
    {
        get
		{
			return _slashPower;
		}
		set
		{
			AssertMutable();
			_slashPower = value;
            base.DynamicVars["SlashPower"].BaseValue = _slashPower;
		}
    }

    [SavedProperty]
    public int DazedCount
    {
        get
		{
			return _dazedCount;
		}
		set
		{
			AssertMutable();
			_dazedCount = value;
            base.DynamicVars["DazedCount"].BaseValue = _dazedCount;
		}
    }

    [SavedProperty]
    public int DebrisCount
    {
        get
		{
			return _debrisCount;
		}
		set
		{
			AssertMutable();
			_debrisCount = value;
            base.DynamicVars["DebrisCount"].BaseValue = _debrisCount;
		}
    }

    [SavedProperty]
    public int SlimedCount
    {
        get
		{
			return _slimedCount;
		}
		set
		{
			AssertMutable();
			_slimedCount = value;
            base.DynamicVars["SlimedCount"].BaseValue = _slimedCount;
		}
    }

    [SavedProperty]
    public int WoundCount
    {
        get
		{
			return _woundCount;
		}
		set
		{
			AssertMutable();
			_woundCount = value;
            base.DynamicVars["WoundCount"].BaseValue = _woundCount;
		}
    }

    [SavedProperty]
    public int SelfFrailPower
    {
        get
		{
			return _selfFrailPower;
		}
		set
		{
			AssertMutable();
			_selfFrailPower = value;
            base.DynamicVars["SelfFrailPower"].BaseValue = _selfFrailPower;
		}
    }

    [SavedProperty]
    public int SelfVulnerablePower
    {
        get
		{
			return _selfVulnerablePower;
		}
		set
		{
			AssertMutable();
			_selfVulnerablePower = value;
            base.DynamicVars["SelfVulnerablePower"].BaseValue = _selfVulnerablePower;
		}
    }

    [SavedProperty]
    public int SelfWeakPower
    {
        get
		{
			return _selfWeakPower;
		}
		set
		{
			AssertMutable();
			_selfWeakPower = value;
            base.DynamicVars["SelfWeakPower"].BaseValue = _selfWeakPower;
		}
    }

    private void SetAttribute(string propertyName, int value)
    {
        var property = GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(this, value);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("IsUnveiled", IsUnveiled),
        new DamageVar(Damage, ValueProp.Move),
        new RepeatVar(Repeat),
        new DynamicVar("IsTargettingAllEnemies", IsTargettingAllEnemies),
        new CardsVar("Draw", Draw),
        new CardsVar("Add", Add),
        new HealVar(Heal),
		new PowerVar<VulnerablePower>("TargetVulnerablePower", TargetVulnerablePower),
        new PowerVar<WeakPower>("TargetWeakPower", TargetWeakPower),
        new PowerVar<ColdPower>(ColdPower),
        new PowerVar<ElectricityPower>(ElectricityPower),
        new PowerVar<HeatPower>(HeatPower),
        new PowerVar<PoisonPower>(PoisonPower),
        new PowerVar<SlashPower>(SlashPower),

        new CardsVar("DazedCount", DazedCount),
        new CardsVar("DebrisCount", DebrisCount),
        new CardsVar("SlimedCount", SlimedCount),
        new CardsVar("WoundCount", WoundCount),
        new PowerVar<FrailPower>("SelfFrailPower", SelfFrailPower),
        new PowerVar<VulnerablePower>("SelfVulnerablePower", SelfVulnerablePower),
        new PowerVar<WeakPower>("SelfWeakPower", SelfWeakPower),
	];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> hoverTips = [];

            var bonusPower = GetExtraArg(BonusPowers);
            if (bonusPower.argName != null)
            {
                hoverTips.Add(WarframeModPower.GetHoverTip(PowerNames[bonusPower.argName])); 
            }

            var malusPower = GetExtraArg(MalusPowers);
            if (malusPower.argName != null)
            {
                hoverTips.Add(WarframeModPower.GetHoverTip(PowerNames[malusPower.argName])); 
            }

            var malusCard = GetExtraArg(MalusCards);
            if (malusCard.argName != null)
            {
                hoverTips.Add(GetHoverTip(CardNames[malusCard.argName])); 
            }

            return hoverTips;
        }
    }

    public override TargetType TargetType
	{
		get
		{
			return IsRivenTargettingAllEnemies ? TargetType.AllEnemies : TargetType.AnyEnemy;
		}
	}

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override bool HasBuiltInOverlay => true;
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<RivenCardPool>();

    private bool IsRivenUnveiled => base.DynamicVars["IsUnveiled"].BaseValue == 0;
    private bool IsRivenTargettingAllEnemies => base.DynamicVars["IsTargettingAllEnemies"].BaseValue > 0;
    private bool HasRepeat => base.DynamicVars.Repeat.BaseValue > 1;
    private bool HasDraw => base.DynamicVars["Draw"].BaseValue > 0;
    private bool HasAdd => base.DynamicVars["Add"].BaseValue > 0;
    private bool HasHeal => base.DynamicVars.Heal.BaseValue > 0;

    private (string? argName, decimal argValue) GetExtraArg(List<string> values)
    {
        foreach (string value in values)
        {
            if (base.DynamicVars.TryGetValue(value, out var arg) && arg.BaseValue > 0)
            {
                return (value, arg.BaseValue);
            }
        }
        return (null, 0);
    }

    private void AddExtraArg(string key, List<string> values, LocString description, Func<string, string> getNameFunc)
    {
        foreach (string value in values)
        {
            if (base.DynamicVars.TryGetValue(value, out var arg) && arg.BaseValue > 0)
            {
                description.Add($"Has{key}", true);
                description.Add(key, arg.BaseValue);
                description.Add($"{key}Name", getNameFunc(value));
                return;
            }
        }
        description.Add($"Has{key}", false);
        description.Add(key, 0);
        description.Add($"{key}Name", "");
    }

    private void AddExtraPowerArg(string key, List<string> values, LocString description)
    {
        AddExtraArg(key, values, description, value => WarframeModPower.GetPower(PowerNames[value]).Title.GetFormattedText());
    }

    private void AddExtraCardArg(string key, List<string> values, LocString description)
    {
        AddExtraArg(key, values, description, value => GetCard(CardNames[value]).Title);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("IsUnveiled", IsRivenUnveiled);
        description.Add("HasRepeat", HasRepeat);
        description.Add("IsTargettingAllEnemies", IsRivenTargettingAllEnemies);
        description.Add("HasDraw", HasDraw);
        description.Add("HasAdd", HasAdd);
        description.Add("HasHeal", HasHeal);
        AddExtraPowerArg("BonusPower", BonusPowers, description);
        AddExtraCardArg("MalusCard", MalusCards, description);
        AddExtraPowerArg("MalusPower", MalusPowers, description);
    }

    private static Dictionary<string, (decimal midValue, decimal minValue, decimal floatValue)> BaseValues => new()
    {
        {"Damage", (8m, 1m, 2.5m)},
        {"Repeat", (2m, 2m, 0.5m)},
        {"IsTargettingAllEnemies", (1m, 1m, 0m)},
        {"Draw", (2m, 1m, 1m)},
        {"Add", (2m, 1m, 1m)},
        {"Heal", (2m, 1m, 0.5m)},
        {"TargetVulnerablePower", (2m, 1m, 1m)},
        {"TargetWeakPower", (2m, 1m, 1m)},
        {"ColdPower", (5m, 1m, 2m)},
        {"ElectricityPower", (2m, 1m, 1m)},
        {"HeatPower", (3m, 1m, 1.5m)},
        {"PoisonPower", (3m, 1m, 1.5m)},
        {"SlashPower", (3m, 1m, 1.5m)},
        {"DazedCount", (1m, 1m, 0m)},
        {"DebrisCount", (1m, 1m, 0m)},
        {"SlimedCount", (1m, 1m, 0m)},
        {"WoundCount", (1m, 1m, 0m)},
        {"SelfFrailPower", (3m, 1m, 1.5m)},
        {"SelfVulnerablePower", (3m, 1m, 1.5m)},
        {"SelfWeakPower", (3m, 1m, 1.5m)},
    };

    private static Dictionary<string, (decimal bonus, decimal malus)> Weights => new()
    {
        {"2+", (1m, 0)},
        {"2+1", (1.25m, 0.75m)},
        {"3+", (0.5m, 0)},
        {"3+1", (0.75m, 1m)}
    };

    private static Dictionary<string, (int bonusCount, int malusCount)> StatTypes => new()
    {
        {"2+", (2, 0)},
        {"2+1", (2, 1)},
        {"3+", (3, 0)},
        {"3+1", (3, 1)}
    };

    private static List<string> AddtionalDamages => ["Repeat", "IsTargettingAllEnemies"];

    private static List<string> Hp => ["Heal"];

    private static List<string> BonusCards => ["Draw", "Add"];

    private static List<string> BonusPowers => [
        "TargetVulnerablePower",
        "TargetWeakPower",
        "ColdPower",
        "ElectricityPower",
        "HeatPower",
        "PoisonPower",
        "SlashPower"
    ];

    private static List<string> MalusCards => [
        "DazedCount",
        "DebrisCount",
        "SlimedCount",
        "WoundCount"
    ];

    private static List<string> MalusPowers => [
        "SelfFrailPower",
        "SelfVulnerablePower",
        "SelfWeakPower"
    ];

    private static List<List<string>> ExtraBonus => [AddtionalDamages, Hp, BonusCards, BonusPowers];

    private static List<List<string>> Malus => [MalusCards, MalusPowers];

    private static Dictionary<string, Type> PowerNames => new()
    {
        {"TargetVulnerablePower", typeof(VulnerablePower)},
        {"TargetWeakPower", typeof(WeakPower)},
        {"ColdPower", typeof(ColdPower)},
        {"ElectricityPower", typeof(ElectricityPower)},
        {"HeatPower", typeof(HeatPower)},
        {"PoisonPower", typeof(PoisonPower)},
        {"SlashPower", typeof(SlashPower)},
        {"SelfFrailPower", typeof(FrailPower)},
        {"SelfVulnerablePower", typeof(VulnerablePower)},
        {"SelfWeakPower", typeof(WeakPower)},
    };

    private static Dictionary<string, Type> CardNames => new()
    {
        {"DazedCount", typeof(Dazed)},
        {"DebrisCount", typeof(Debris)},
        {"SlimedCount", typeof(Slimed)},
        {"WoundCount", typeof(Wound)}
    };

    private void InitAttributes()
    {
        foreach (var attribute in base.DynamicVars)
        {
            SetAttribute(attribute.Key, 0);
        }
    }

    private void UpdateAttribute(string statType, string attribute, bool isBonus, Rng rng)
    {
        decimal baseValue = BaseValues[attribute].midValue;
        decimal weight = isBonus ? Weights[statType].bonus : Weights[statType].malus;
        decimal floatValue = BaseValues[attribute].floatValue;
        decimal adjustedValue = (baseValue + rng.NextDecimal(-floatValue, floatValue)) * weight;
        decimal finalValue = Math.Max(BaseValues[attribute].minValue, Math.Round(adjustedValue));
        SetAttribute(attribute, (int)finalValue);
    }

    public void Cycle(Rng rng)
    {
        var statType = rng.NextItem(StatTypes);
        InitAttributes();
        UpdateAttribute(statType.Key, "Damage", true, rng);

        var bonus = ExtraBonus.UnstableShuffle(rng);
        for (int i = 1; i < statType.Value.bonusCount; i++) //The first attribute must be Damage
        {
            UpdateAttribute(statType.Key, bonus[i].UnstableShuffle(rng).FirstOrDefault(), true, rng);
        }

        var malus = Malus.UnstableShuffle(rng);
        for (int i = 0; i < statType.Value.malusCount; i++)
        {
            UpdateAttribute(statType.Key, malus[i].UnstableShuffle(rng).FirstOrDefault(), false, rng);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!IsRivenTargettingAllEnemies)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            
            //Damage and Repeat
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(Math.Max(1, base.DynamicVars.Repeat.IntValue))
                .FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            //Apply bonus powers
            foreach (string power in BonusPowers)
            {
                if (base.DynamicVars[power].BaseValue > 0 && PowerNames.TryGetValue(power, out var powerType))
                {
                    await WarframeModPower.Apply(powerType, choiceContext, cardPlay.Target, base.DynamicVars[power].BaseValue, base.Owner.Creature, this);
                }
            }
        }
        else
        {
            //Damage and Repeat
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(Math.Max(1, base.DynamicVars.Repeat.IntValue))
                .FromCard(this).TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            
            //Apply bonus powers
            foreach (Creature enemy in base.CombatState.HittableEnemies)
            {
                foreach (string power in BonusPowers)
                {
                    if (base.DynamicVars[power].BaseValue > 0 && PowerNames.TryGetValue(power, out var powerType))
                    {
                        await WarframeModPower.Apply(powerType, choiceContext, enemy, base.DynamicVars[power].BaseValue, base.Owner.Creature, this);
                    }
                }          
            }

        }
        
        //Draw cards
        if (HasDraw)
        {
            await CardPileCmd.Draw(choiceContext, base.DynamicVars["Draw"].IntValue, base.Owner);
        }

        //Add bonus cards
        if (HasAdd)
        {
            List<CardModel> cards = CardFactory.GetDistinctForCombat(base.Owner, from c in base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
                where c.Rarity == CardRarity.Common || c.Rarity == CardRarity.Uncommon || c.Rarity == CardRarity.Rare
                select c, base.DynamicVars["Add"].IntValue, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
            foreach (CardModel card in cards)
            {
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner);
            }
        }

        //Heal
        if (HasHeal)
        {
            await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
        }

        //Add malus cards
        foreach (string card in MalusCards)
        {
            if (base.DynamicVars[card].BaseValue > 0 && CardNames.TryGetValue(card, out var cardType))
            {
                CardModel? c = CreateCard(cardType, base.Owner, base.CombatState);
                if (c != null)
                {
		            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(c, PileType.Discard, base.Owner));                    
                }
            }
        }

        //Apply malus powers
        foreach (string power in MalusPowers)
        {
            if (base.DynamicVars[power].BaseValue > 0 && PowerNames.TryGetValue(power, out var powerType))
            {
                await WarframeModPower.Apply(powerType, choiceContext, base.Owner.Creature, base.DynamicVars[power].BaseValue, base.Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.RemoveKeyword(CardKeyword.Exhaust);
    }
}
