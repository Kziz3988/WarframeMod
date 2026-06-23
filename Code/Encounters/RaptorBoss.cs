using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Monsters;

namespace WarframeMod.Code.Encounters;

public sealed class RaptorBoss() : CustomEncounterModel(RoomType.Boss)
{
    public override  string? CustomRunHistoryIconPath => "raptor_boss.png".UiPath();
    public override  string? CustomRunHistoryIconOutlinePath => "raptor_boss_outline.png".UiPath();
    public override string BossNodePath => "raptor_boss_icon".UiPath();
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;

	public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Raptor>()];

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return [
            (ModelDb.Monster<Raptor>().ToMutable(), null)
        ];
	}

    public override bool IsValidForAct(ActModel act)
    {
        return act is Glory;
    }
}
