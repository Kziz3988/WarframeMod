using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Extensions;

public static class ModelIdExtensions
{
    public static bool IsWarframe(this ModelId? characterId)
    {
        return characterId != null && WarframeModCardPool.CustomStrikeAndDefend.ContainsKey(characterId);
    }
}