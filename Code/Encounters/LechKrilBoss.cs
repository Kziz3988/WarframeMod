using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Monsters;

namespace WarframeMod.Code.Encounters;

public sealed class LechKrilBoss() : CustomEncounterModel(RoomType.Boss)
{
    public override  string? CustomRunHistoryIconPath => "lech_kril_boss.png".UiPath();
    public override  string? CustomRunHistoryIconOutlinePath => "lech_kril_boss_outline.png".UiPath();
    public override string BossNodePath => "lech_kril_boss_icon".UiPath();
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;

	public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<LechKril>()];

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return [
            (ModelDb.Monster<LechKril>().ToMutable(), null)
        ];
	}

    public override bool IsValidForAct(ActModel act)
    {
        return act is Hive;
    }

    public override BackgroundAssets? CustomEncounterBackground(ActModel parentAct, Rng rng)
    {
        return new BackgroundAssets(ModelDb.Encounter<TheInsatiableBoss>().Id.Entry.ToLowerInvariant(), Rng);
    }
}
