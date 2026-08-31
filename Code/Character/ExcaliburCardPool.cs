
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Character;

public class ExcaliburCardPool : CustomCardPoolModel
{
    public override string Title => Excalibur.CharacterId; //This is not a display name.
    
    public override string BigEnergyIconPath => "charui/warframe_big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/warframe_text_energy.png".ImagePath();

    public override Color ShaderColor => Excalibur.Color;
    
    public override Color DeckEntryCardColor => Excalibur.Color;
    
    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return (CardModel[])ModelDb.CardPool<WarframeModCardPool>().AllCards;
    }
}