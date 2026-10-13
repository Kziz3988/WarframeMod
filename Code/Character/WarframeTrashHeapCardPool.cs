
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Cards.Event;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Character;

public class WarframeTrashHeapCardPool : CustomCardPoolModel
{
    public override string Title => Excalibur.CharacterId; //This is not a display name.
    
    public override string BigEnergyIconPath => "charui/warframe_big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/warframe_text_energy.png".ImagePath();

    public override Color ShaderColor => Warframe.Color;
    
    public override Color DeckEntryCardColor => Warframe.Color;
    
    public override bool IsColorless => true;

    protected override CardModel[] GenerateAllCards()
    {
        return [
            ModelDb.Card<CollectiveCurse>(),
            ModelDb.Card<Crystallize>(),
            ModelDb.Card<FinalStand>(),
            ModelDb.Card<MachRush>(),
            ModelDb.Card<Merulina>(),
            ModelDb.Card<SereneStorm>(),
            ModelDb.Card<Spores>(),
            ModelDb.Card<Thurible>(),
            ModelDb.Card<WyrdScythes>(),
        ];
    }
}