using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using WarframeMod.Code.Cards.Basic;

namespace WarframeMod.Code.Character;

public class Excalibur : Warframe
{
    public const string CharacterId = "Excalibur";
    public override CharacterGender Gender => CharacterGender.Masculine;
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<SlashDash>(),
        ModelDb.Card<SlashDash>(),
        ModelDb.Card<SlashDash>(),
        ModelDb.Card<SlashDash>(),
        ModelDb.Card<RadialBlind>(),
        ModelDb.Card<RadialBlind>(),
        ModelDb.Card<RadialBlind>(),
        ModelDb.Card<RadialJavelin>(),
        ModelDb.Card<ExaltedBlade>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<ExcaliburCardPool>();
    
    public override string CustomIconPath => "excalibur_icon.tscn".CharacterIconScenePath();
    public override string CustomIconTexturePath => "character_icon_excalibur.png".CharacterUiPath();
    public override string CustomIconOutlineTexturePath => "character_icon_excalibur_outline.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_excalibur.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_excalibur_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_excalibur.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "char_select_bg_excalibur.tscn".CharacterBackgroundScenePath();
    public override string CustomArmPointingTexturePath => "multiplayer_hand_excalibur_point.png".CharacterUiPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_excalibur_rock.png".CharacterUiPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_excalibur_paper.png".CharacterUiPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_excalibur_scissors.png".CharacterUiPath();
    public override string CustomVisualPath => "excalibur.tscn".CharacterAnimationScenePath();
    public override string CustomRestSiteAnimPath => "excalibur_rest_site.tscn".CharacterRestAnimationScenePath();
    public override string CustomMerchantAnimPath => "excalibur_merchant.tscn".CharacterMerchantAnimationScenePath();
}