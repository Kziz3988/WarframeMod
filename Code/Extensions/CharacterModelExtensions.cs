using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Extensions;

public static class CharacterModelExtensions
{
    public static bool IsWarframe(this CharacterModel? character)
    {
        return character != null && WarframeModCardPool.CustomStrikeAndDefend.ContainsKey(character.Id);
    }
}