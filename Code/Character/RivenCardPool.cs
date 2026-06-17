using BaseLib.Abstracts;
using WarframeMod.Code.Extensions;
using Godot;

namespace WarframeMod.Code.Character;

public class RivenCardPool : CustomCardPoolModel
{
    public override string Title => "WARFRAMEMOD-RIVEN";

    public override string BigEnergyIconPath => "charui/warframe_big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/warframe_text_energy.png".ImagePath();

    public override Color ShaderColor => new(0.61f, 0.57f, 0.76f);
    
    public override Color DeckEntryCardColor => new(0.61f, 0.57f, 0.76f);
    
    public override bool IsColorless => false;
}