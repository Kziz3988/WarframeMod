using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using WarframeMod.Code.Cards.Basic;

namespace WarframeMod.Code.Character;

public class Volt : Warframe
{
    public const string CharacterId = "Volt";
    public override CharacterGender Gender => CharacterGender.Masculine;

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<Shock>(),
        ModelDb.Card<Shock>(),
        ModelDb.Card<Shock>(),
        ModelDb.Card<Shock>(),
        ModelDb.Card<ElectricShield>(),
        ModelDb.Card<ElectricShield>(),
        ModelDb.Card<ElectricShield>(),
        ModelDb.Card<ElectricShield>(),
        ModelDb.Card<ElectricShield>(),
        ModelDb.Card<Speed>(),
        ModelDb.Card<Discharge>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<VoltCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<VoltRelicPool>();
    
    public override string CustomIconPath => "volt_icon.tscn".CharacterIconScenePath();
    public override string CustomIconTexturePath => "character_icon_volt.png".CharacterUiPath();
    public override string CustomIconOutlineTexturePath => "character_icon_volt_outline.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_volt.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_volt_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_volt.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "char_select_bg_volt.tscn".CharacterBackgroundScenePath();
    public override string CustomArmPointingTexturePath => "multiplayer_hand_volt_point.png".CharacterUiPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_volt_rock.png".CharacterUiPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_volt_paper.png".CharacterUiPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_volt_scissors.png".CharacterUiPath();
    public override string CustomVisualPath => "volt.tscn".CharacterAnimationScenePath();
    public override string CustomRestSiteAnimPath => "volt_rest_site.tscn".CharacterRestAnimationScenePath();
    public override string CustomMerchantAnimPath => "volt_merchant.tscn".CharacterMerchantAnimationScenePath();
}