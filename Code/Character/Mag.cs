using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using WarframeMod.Code.Cards.Basic;

namespace WarframeMod.Code.Character;

public class Mag : Warframe
{
    public const string CharacterId = "Mag";
    public override CharacterGender Gender => CharacterGender.Feminine;
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<Magnetize>(),
        ModelDb.Card<Magnetize>(),
        ModelDb.Card<Magnetize>(),
        ModelDb.Card<Pull>(),
        ModelDb.Card<Pull>(),
        ModelDb.Card<Pull>(),
        ModelDb.Card<Pull>(),
        ModelDb.Card<Polarize>(),
        ModelDb.Card<Crush>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<MagCardPool>();
    
    public override string CustomIconPath => "mag_icon.tscn".CharacterIconScenePath();
    public override string CustomIconTexturePath => "character_icon_mag.png".CharacterUiPath();
    public override string CustomIconOutlineTexturePath => "character_icon_mag_outline.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_mag.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_mag_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_mag.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "char_select_bg_mag.tscn".CharacterBackgroundScenePath();
    public override string CustomArmPointingTexturePath => "multiplayer_hand_mag_point.png".CharacterUiPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_mag_rock.png".CharacterUiPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_mag_paper.png".CharacterUiPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_mag_scissors.png".CharacterUiPath();
    public override string CustomVisualPath => "mag.tscn".CharacterAnimationScenePath();
    public override string CustomRestSiteAnimPath => "mag_rest_site.tscn".CharacterRestAnimationScenePath();
    public override string CustomMerchantAnimPath => "mag_merchant.tscn".CharacterMerchantAnimationScenePath();
}