using BaseLib.Abstracts;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Monsters;

public abstract class Kubrow(string visualPath) : CustomPetModel(false)
{
    public override int MinInitialHp => 9999;

	public override int MaxInitialHp => 9999;

    public override string? CustomVisualPath => visualPath.CharacterAnimationScenePath();
}
