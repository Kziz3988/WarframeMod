using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Extensions;

public class ElementData
{
    public Dictionary<string, decimal> Elements { get; set; }

    private static readonly Dictionary<string, Type> PowerTypeMap = new()
    {
        {"ColdPower", typeof(ColdPower)},
        {"ElectricityPower", typeof(ElectricityPower)},
        {"HeatPower", typeof(HeatPower)},
        {"PoisonPower", typeof(PoisonPower)}
    };

    public ElementData()
    {
        Elements = new Dictionary<string, decimal>
        {
            {"ColdPower", 0},
            {"ElectricityPower", 0},
            {"HeatPower", 0},
            {"PoisonPower", 0}
        };
    }

    public static ElementData operator +(ElementData a, ElementData b)
    {
        ElementData result = new();
        foreach (string name in result.Elements.Keys)
        {
            result.Elements[name] = a.Elements[name] + b.Elements[name];
        }
        return result;
    }

    public async Task Apply(PlayerChoiceContext choiceContext, Creature target, Creature? applier, CardModel? cardSource, bool silent = false)
    {
        foreach (var kvp in Elements)
        {
            if (PowerTypeMap.TryGetValue(kvp.Key, out var powerType))
            {
                await WarframeModPower.Apply(powerType, choiceContext, target, kvp.Value, applier, cardSource, silent);
            }
        }
    }

    public static HoverTip GetStaticHoverTip()
	{
		LocString description = new LocString("static_hover_tips", "WARFRAMEMOD-ELEMENT.description");
		return new HoverTip(new LocString("static_hover_tips", "WARFRAMEMOD-ELEMENT.title"), description);
	}
}

public static class CardModelExtensions
{
    public static ElementData GetElements(this CardModel card)
    {
        ElementData data = new();
        foreach (string name in data.Elements.Keys)
        {
            if (card.DynamicVars.ContainsKey(name))
            {
                data.Elements[name] += card.DynamicVars[name].IntValue;
            }
        }
        return data;
    }

    public static decimal GetDamage(this CardModel card)
    {
        decimal damage = 0m;
        if (card.DynamicVars.ContainsKey("CalculatedDamage"))
        {
            damage = card.DynamicVars.CalculatedDamage.Calculate(null);
        }
        else if (card.DynamicVars.ContainsKey("Damage"))
        {
            damage = card.DynamicVars.Damage.BaseValue;
        }
        else if (card.DynamicVars.ContainsKey("OstyDamage"))
        {
            damage = card.DynamicVars.OstyDamage.BaseValue;
        }
        damage = Hook.ModifyDamage(card.Owner.RunState, card.Owner.Creature.CombatState, null, card.Owner.Creature, damage, ValueProp.Move, card, ModifyDamageHookType.All, CardPreviewMode.None, out IEnumerable<AbstractModel> _);
        return damage;
    }
}