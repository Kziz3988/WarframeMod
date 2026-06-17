using BaseLib.Abstracts;
using BaseLib.Extensions;
using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.HoverTips;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using WarframeMod.Code.Powers.Debuff;
using System;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Powers;

/// <summary>
/// This is the base class for your mod's powers, which is set up to load the power's images from your mod's resources.
/// When creating a power, right click the Powers folder and create a new file with the Custom Power template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
public abstract class WarframeModPower : CustomPowerModel
{
    //Loads from WarframeMod/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    /// <summary>
    /// Whether this power is a buff or debuff.
    /// </summary>
    public abstract override PowerType Type { get; }
    
    /// <summary>
    /// How this power stacks if reapplied. Counter is the most common type, where applying the power again just
    /// adds to the amount. Single means the power does not stack, like Barricade. None functions identically to
    /// Single, but you're suggested to use Single as it is more explicit about how it will work.
    /// </summary>
    public abstract override PowerStackType StackType { get; }

    protected virtual string ExtraDescriptionLocKey => base.Id.Entry + ".extraDescription";
    public bool HasExtraDescription => LocString.Exists("powers", ExtraDescriptionLocKey);

    public LocString ExtraDescription
	{
		get
		{
			if (!HasExtraDescription)
			{
				return Description;
			}
			return new LocString("powers", ExtraDescriptionLocKey);
		}
    }

    protected virtual HoverTip GetExtraHoverTip()
    {
        return new HoverTip();
    }

    public static async Task Stun(Creature target)
    {
        if (target.Player != null && target.GetPower<PlayerStunnedPower>() == null)
        {
            await PowerCmd.Apply<PlayerStunnedPower>(target, 1, null, null);
        }
        if (target.Monster != null)
        {
            await CreatureCmd.Stun(target);
        }
    }

    public static async Task Apply(Type powerType, Creature target, decimal amount, Creature? applier, CardModel? cardSource, bool silent = false)
    {
        var method = typeof(PowerCmd).GetMethod("Apply",
        [
            typeof(Creature),   // target
            typeof(decimal),    // value
            typeof(Creature),   // applier
            typeof(CardModel),  // cardSource
            typeof(bool)        // silent
        ]);
        
        if (method != null)
        {
            var genericMethod = method.MakeGenericMethod(powerType);
            await (Task)genericMethod.Invoke(null, [target, amount, applier, cardSource, silent]);
        }
    }

    public static IHoverTip GetHoverTip(Type powerType)
    {
        var method = typeof(HoverTipFactory).GetMethod("FromPower", System.Type.EmptyTypes);
        if (method != null)
        {
            var genericMethod = method.MakeGenericMethod(powerType);
            return (IHoverTip)genericMethod.Invoke(null, null);
        }
        
        throw new InvalidOperationException($"Failed to create hover tip for power type {powerType.Name}");
    }

    public static PowerModel GetPower(Type powerType)
    {
        var method = typeof(ModelDb).GetMethod("Power", System.Type.EmptyTypes);
        if (method != null)
        {
            var genericMethod = method.MakeGenericMethod(powerType);
            return (PowerModel)genericMethod.Invoke(null, null);
        }
        
        throw new InvalidOperationException($"Failed to get power of type {powerType.Name}");
    }
}