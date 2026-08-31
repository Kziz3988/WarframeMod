using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using WarframeMod.Code.Relics.Starter;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.Helpers;

namespace WarframeMod.Code.Character;

public abstract class Warframe : PlaceholderCharacterModel
{
    public static readonly Color Color = Color.FromHsv(0.54f, 0.35f, 0.92f);

    public override Color NameColor => Color;
    public override int StartingHp => 50;
    public override int StartingGold => 99;

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ArcaneHusk>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<WarframeModCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<WarframeModRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<WarframeModPotionPool>();

    public override string CustomEnergyCounterPath => "warframe_energy_counter.tscn".CharacterEnergyScenePath();
    public override string CustomTrailPath => SceneHelper.GetScenePath("vfx/card_trail_defect");

    public override Color MapDrawingColor => Color.FromHsv(0.54f, 0.22f, 0.45f);

    public static HoverTip GetStaticHoverTip()
	{
		LocString description = new LocString("static_hover_tips", "WARFRAMEMOD-WARFRAME.description");
		return new HoverTip(new LocString("static_hover_tips", "WARFRAMEMOD-WARFRAME.title"), description);
	}
}