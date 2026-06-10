using BaseLib.Abstracts;
using WarframeMod.Code.Extensions;
using Godot;

namespace WarframeMod.Code.Character;

public class WarframeModRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Excalibur.Color;

    public override string BigEnergyIconPath => "charui/warframe_big__energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/warframe_text_energy.png".ImagePath();
}