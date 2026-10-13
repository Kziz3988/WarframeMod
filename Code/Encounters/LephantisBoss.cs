using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Monsters;

namespace WarframeMod.Code.Encounters;

public sealed class LephantisBoss() : CustomEncounterModel(RoomType.Boss)
{
    public override  string? CustomRunHistoryIconPath => "lephantis_boss.png".UiPath();
    public override  string? CustomRunHistoryIconOutlinePath => "lephantis_boss_outline.png".UiPath();
    public override string BossNodePath => "lephantis_boss_icon".UiPath();
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;

    public override bool HasScene => true;
    public override IReadOnlyList<string> Slots => ["infested0", "infested1", "infested2", "infested_corpus", "ancient_infested", "infested_grineer"];
    public override string? CustomScenePath => "lephantis_boss.tscn".EncounterScenePath();

	public override IEnumerable<MonsterModel> AllPossibleMonsters => [
        ModelDb.Monster<InfestedCorpus>(),
        ModelDb.Monster<AncientInfested>(),
        ModelDb.Monster<InfestedGrineer>(),
        ModelDb.Monster<Charger>(),
        ModelDb.Monster<Leaper>(),
        ModelDb.Monster<VolatileRunner>()
    ];

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return [
            (ModelDb.Monster<InfestedCorpus>().ToMutable(), "infested_corpus"),
            (ModelDb.Monster<AncientInfested>().ToMutable(), "ancient_infested"),
            (ModelDb.Monster<InfestedGrineer>().ToMutable(), "infested_grineer")
        ];
	}

    public override bool IsValidForAct(ActModel act)
    {
        return act is Glory;
    }
}
