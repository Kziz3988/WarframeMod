using System;
using System.Linq;
using System.Reflection;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Extensions;

public static class AncientEventModelExtensions
{
    public static EventOption RelicOption<T>(this AncientEventModel instance, string pageName = "INITIAL") where T : RelicModel
    {
        var method = typeof(AncientEventModel)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(m =>
                m.Name == "RelicOption" &&
                m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 1);

        if (method == null)
        {
            throw new InvalidOperationException("Cannot find generic RelicOption<T> method");
        }

        var genericMethod = method.MakeGenericMethod(typeof(T));
        return (EventOption)genericMethod.Invoke(instance, new object?[] { pageName });
    }

    public static EventOption RelicOption(this AncientEventModel instance, RelicModel relic, string pageName = "INITIAL")
    {
        var method = typeof(AncientEventModel)
            .GetMethod(
                "RelicOption",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(RelicModel), typeof(string) },
                null);

        if (method == null)
        {
            throw new InvalidOperationException("Cannot find RelicOption(RelicModel, string) method");
        }

        return (EventOption)method.Invoke(instance, new object?[] { relic, pageName });
    }
}