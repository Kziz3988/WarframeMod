using BaseLib.Abstracts;
using BaseLib.Utils;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Potions;

[Pool(typeof(WarframeModPotionPool))]
public abstract class WarframeModPotion : CustomPotionModel;