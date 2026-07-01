

using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Extensions;

public static class PowerModelExtensions
{
    private static HashSet<Type>? _buffTheLessTheBetter;
    private static HashSet<Type>? _debuffTheMoreTheBetter;
    private static HashSet<Type>? _isNotBetterOrWorse;

    private static HashSet<Type> BuffTheLessTheBetter
    {
        get
        {
            if (_buffTheLessTheBetter == null)
            {
                _buffTheLessTheBetter = [
                    typeof(AsleepPower),
                    typeof(BattlewornDummyTimeLimitPower),
                    typeof(EscapeArtistPower),
                    typeof(HardenedShellPower),
                    typeof(HardToKillPower),
                    typeof(HatchPower),
                    typeof(SandpitPower),
                    typeof(WitheringPresencePower),
                ];
            }
            return _buffTheLessTheBetter;
        }
    }

    private static HashSet<Type> DebuffTheMoreTheBetter
    {
        get
        {
            if (_debuffTheMoreTheBetter == null)
            {
                _debuffTheMoreTheBetter = [
                    typeof(SlothPower),
                ];
            }
            return _debuffTheMoreTheBetter;
        }
    }

    private static HashSet<Type> IsNotBetterOrWorse
    {
        get
        {
            if (_isNotBetterOrWorse == null)
            {
                _isNotBetterOrWorse = [
                    typeof(ArmorPlatePower),
                    typeof(PlowPower),
                    typeof(ShriekPower),
                ];
            }
            return _isNotBetterOrWorse;
        }
    }

    public static bool IsTheMoreTheBetter(this PowerModel power)
    {
        Type powerType = power.GetType();
        return !IsNotBetterOrWorse.Contains(powerType) && ((power.Type == PowerType.Buff && !BuffTheLessTheBetter.Contains(powerType)) || (power.Type == PowerType.Debuff && DebuffTheMoreTheBetter.Contains(powerType)));
    }

    public static bool IsTheLessTheBetter(this PowerModel power)
    {
        Type powerType = power.GetType();
        return !IsNotBetterOrWorse.Contains(powerType) && ((power.Type == PowerType.Buff && BuffTheLessTheBetter.Contains(powerType)) || (power.Type == PowerType.Debuff && !DebuffTheMoreTheBetter.Contains(powerType)));
    }
}