using System;
using System.Reflection;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Extensions;

public static class AncientEventModelExtensions
{
    public static EventOption RelicOption<T>(this AncientEventModel instance, string page, string descriptionOverride) where T : RelicModel, new()
    {
        var method = typeof(AncientEventModel).GetMethod(
            "RelicOption", 
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(string), typeof(string)],
            null
        );
            
        if (method == null)
        {
            throw new InvalidOperationException("Cannot find RelicOption method");
        }
        
        var genericMethod = method.MakeGenericMethod(typeof(T));
        return (EventOption)genericMethod.Invoke(instance, [page, descriptionOverride]);
    }

    public static EventOption RelicOption(this AncientEventModel instance, RelicModel relic, string pageName = "INITIAL", string? customDonePage = null)
    {
        var method = typeof(AncientEventModel)
            .GetMethod("RelicOption", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                [typeof(RelicModel), typeof(string), typeof(string)], 
                null);
        
        if (method == null)
        {
            throw new InvalidOperationException("Cannot find RelicOption method");
        }
        
        return (EventOption)method.Invoke(instance, [relic, pageName, customDonePage]);
    }
}