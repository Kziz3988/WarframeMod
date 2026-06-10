using BaseLib.Abstracts;
using WarframeMod.Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using WarframeMod.Code.Cards.Basic;
using WarframeMod.Code.Relics.Starter;
using MegaCrit.Sts2.Core.Helpers;

namespace WarframeMod.Code.Character;

public class Excalibur : PlaceholderCharacterModel
{
    public const string CharacterId = "Excalibur";
    
    public static readonly Color Color = Color.FromHsv(0.54f, 0.35f, 0.92f);

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 70;
    public override int StartingGold => 99;

    
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

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ArcaneHusk>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<WarframeModCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<WarframeModRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<WarframeModPotionPool>();
    
    public override string CustomIconPath => "excalibur_icon.tscn".CharacterIconScenePath();
    public override string CustomIconTexturePath => "character_icon_excalibur.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_excalibur.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_excalibur_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_excalibur.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "char_select_bg_excalibur.tscn".CharacterBackgroundScenePath();
    public override string CustomEnergyCounterPath => "warframe_energy_counter.tscn".CharacterEnergyScenePath();
    public override string CustomTrailPath => SceneHelper.GetScenePath("vfx/card_trail_defect");
    public override string CustomArmPointingTexturePath => "multiplayer_hand_excalibur_point.png".CharacterUiPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_excalibur_rock.png".CharacterUiPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_excalibur_paper.png".CharacterUiPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_excalibur_scissors.png".CharacterUiPath();
    public override string CustomVisualPath => "excalibur.tscn".CharacterAnimationScenePath();
    public override string CustomRestSiteAnimPath => "excalibur_rest_site.tscn".CharacterRestAnimationScenePath();
    public override string CustomMerchantAnimPath => "excalibur_merchant.tscn".CharacterMerchantAnimationScenePath();

    public override Color MapDrawingColor => Color.FromHsv(0.54f, 0.22f, 0.45f);

}