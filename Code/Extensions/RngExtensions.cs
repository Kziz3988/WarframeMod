
using System;
using MegaCrit.Sts2.Core.Random;

namespace WarframeMod.Code.Extensions;

public static class RngExtensions
{
    public static decimal NextDecimal(this Rng rng)
    {
        double value = rng.NextDouble();
        return (decimal)value;
    }

    public static decimal NextDecimal(this Rng rng, decimal max)
    {
        decimal value = rng.NextDecimal();
        return value * max;
    }

    public static decimal NextDecimal(this Rng rng, decimal min, decimal max)
    {
        decimal range = max - min;
        decimal value = rng.NextDecimal();
        return min + (value * range);
    }

    public static decimal NextDecimal(this Rng rng, int precision)
    {
        double value = rng.NextDouble();
        return Math.Round((decimal)value, precision);
    }

    public static decimal NextDecimal(this Rng rng, decimal min, decimal max, int precision)
    {
        decimal value = rng.NextDecimal(min, max);
        return Math.Round(value, precision);
    }
}