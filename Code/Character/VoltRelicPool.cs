using BaseLib.Abstracts;
using WarframeMod.Code.Extensions;
using Godot;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Character;

public class VoltRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Excalibur.Color;
    public override string BigEnergyIconPath => "charui/warframe_big__energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/warframe_text_energy.png".ImagePath();

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return ModelDb.RelicPool<WarframeModRelicPool>().AllRelics;
    }
}