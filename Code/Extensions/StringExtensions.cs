using System.IO;
using Godot;

namespace WarframeMod.Code.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return Path.Join(MainFile.ResPath, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "card_portraits", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find card image path: " + path);
        return Path.Join(MainFile.ResPath, "images", "card_portraits", "card.png");
    }

    public static string BigCardImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "card_portraits", "big", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find big card image path: " + path);
        return Path.Join(MainFile.ResPath, "images", "card_portraits", "big", "card.png");
    }

    public static string PowerImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "powers", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find power image path: " + path);
        return Path.Join(MainFile.ResPath, "images", "powers", "power.png");
    }

    public static string BigPowerImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "powers", "big", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find big power image path: " + path);
        return Path.Join(MainFile.ResPath, "images", "powers", "big", "power.png");
    }

    public static string RelicImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "relics", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find relic image path: " + path);
        return Path.Join(MainFile.ResPath, "images", "relics", "relic.png");
    }

    public static string BigRelicImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "relics", "big", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find big relic image path: " + path);
        return Path.Join(MainFile.ResPath, "images", "relics", "big", "relic.png");
    }

    public static string CharacterUiPath(this string path)
    {
        return Path.Join(MainFile.ResPath, "images", "charui", path);
    }

    public static string UiPath(this string path)
    {
        return Path.Join(MainFile.ResPath, "images", "ui", path);
    }

    public static string CharacterIconScenePath(this string path)
    {
        return Path.Join(MainFile.ResPath, "scenes", "ui", "character_icons", path);
    }

    public static string CharacterBackgroundScenePath(this string path)
    {
        return Path.Join(MainFile.ResPath, "scenes", "screens", "char_select", path);
    }

    public static string CharacterEnergyScenePath(this string path)
    {
        return Path.Join(MainFile.ResPath, "scenes", "combat", "energy_counters", path);
    }

    public static string CharacterAnimationScenePath(this string path)
    {
        return Path.Join(MainFile.ResPath, "scenes", "creature_visuals", path);
    }

    public static string CharacterRestAnimationScenePath(this string path)
    {
        return Path.Join(MainFile.ResPath, "scenes", "rest_site", "characters", path);
    }

    public static string CharacterMerchantAnimationScenePath(this string path)
    {
        return Path.Join(MainFile.ResPath, "scenes", "merchant", "characters", path);
    }

    public static string EventBackgroundPath(this string path)
    {
        return Path.Join(MainFile.ResPath, "images", "events", path);
    }

    public static string CardOverlayScenePath(this string path)
    {
        return Path.Join(MainFile.ResPath, "scenes", "cards", "overlays", path);
    }

    public static string MonsterTexturePath(this string path)
    {
        return Path.Join(MainFile.ResPath, "images", "monsters", path);
    }

    public static string VfxPath(this string path)
    {
        return Path.Join(MainFile.ResPath, "scenes", "vfx", path);
    }
}