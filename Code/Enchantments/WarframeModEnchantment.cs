
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Enchantments;

public abstract class WarframeModEnchantment : CustomEnchantmentModel
{
    public static EnchantmentModel GetEnchantment(Type enchantment)
    {
        var method = typeof(ModelDb).GetMethod("Enchantment", System.Type.EmptyTypes);
        if (method != null)
        {
            var genericMethod = method.MakeGenericMethod(enchantment);
            return (EnchantmentModel)genericMethod.Invoke(null, null);
        }
        
        throw new InvalidOperationException($"Failed to get enchantment of type {enchantment.Name}");
    }

    public static IEnumerable<IHoverTip> GetHoverTips(Type enchantmentType, int amount = 1)
    {
        var methods = typeof(HoverTipFactory).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "FromEnchantment" && m.IsGenericMethod)
            .ToArray();

        var method = methods.FirstOrDefault(m =>
            m.GetGenericArguments().Length == 1 &&
            m.GetParameters().Length == 1 &&
            m.GetParameters()[0].ParameterType == typeof(int));

        if (method != null)
        {
            var genericMethod = method.MakeGenericMethod(enchantmentType);
            return (IEnumerable<IHoverTip>)genericMethod.Invoke(null, [amount]);
        }

        throw new InvalidOperationException($"Failed to create enchantment hover tips for type {enchantmentType.Name}");
    }
}