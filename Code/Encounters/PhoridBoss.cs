using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Monsters;

namespace WarframeMod.Code.Encounters;

public sealed class PhoridBoss() : CustomEncounterModel(RoomType.Boss)
{
    public override  string? CustomRunHistoryIconPath => "phorid_boss.png".UiPath();
    public override  string? CustomRunHistoryIconOutlinePath => "phorid_boss_outline.png".UiPath();
    public override string BossNodePath => "phorid_boss_icon".UiPath();
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;

	public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Phorid>()];

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return [
            (ModelDb.Monster<Phorid>().ToMutable(), null)
        ];
	}

    public override bool IsValidForAct(ActModel act)
    {
        return act is Overgrowth || act is Underdocks;
    }
}
