using BaseLib.Abstracts;
using WarframeMod.Code.Extensions;
using Godot;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Cards.Basic;

namespace WarframeMod.Code.Character;

public class WarframeModCardPool : CustomCardPoolModel
{
    public override string Title => Excalibur.CharacterId; //This is not a display name.
    
    public override string BigEnergyIconPath => "charui/warframe_big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/warframe_text_energy.png".ImagePath();

    public override Color ShaderColor => Excalibur.Color;
    
    public override Color DeckEntryCardColor => Excalibur.Color;
    
    public override bool IsColorless => false;

    private static HashSet<string>? _warframeCardPoolTitles;

    private static Dictionary<ModelId, (ModelId, ModelId)>? _cachedCustomStrikeAndDefend;

    public static HashSet<string> WarframeCardPoolTitles
    {
        get
        {
            if (_warframeCardPoolTitles == null)
            {
                _warframeCardPoolTitles =
                [
                    Excalibur.CharacterId,
                    Mag.CharacterId,
                    Volt.CharacterId
                ];
            }
            return _warframeCardPoolTitles;
        }
    }
    
    public static Dictionary<ModelId, (ModelId strike, ModelId defend)> CustomStrikeAndDefend
    {
        get
        {
            if (_cachedCustomStrikeAndDefend == null)
            {
                _cachedCustomStrikeAndDefend = new Dictionary<ModelId, (ModelId, ModelId)>
                {
                    [ModelDb.Character<Excalibur>().Id] = (ModelDb.Card<SlashDash>().Id, ModelDb.Card<RadialBlind>().Id),
                    [ModelDb.Character<Mag>().Id] = (ModelDb.Card<Magnetize>().Id, ModelDb.Card<Pull>().Id),
                    [ModelDb.Character<Volt>().Id] = (ModelDb.Card<Shock>().Id, ModelDb.Card<ElectricShield>().Id)
                };
            }
            return _cachedCustomStrikeAndDefend;
        }
    }
}