using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Monsters;

namespace WarframeMod.Code.Encounters;

public sealed class TheGrustragThree() : CustomEncounterModel(RoomType.Monster, false)
{
	public override IEnumerable<MonsterModel> AllPossibleMonsters =>
    [
        ModelDb.Monster<Leekter>(),
        ModelDb.Monster<ShikTal>(),
        ModelDb.Monster<VemTabook>()
    ];

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return
        [
            (ModelDb.Monster<Leekter>().ToMutable(), null),
            (ModelDb.Monster<ShikTal>().ToMutable(), null),
            (ModelDb.Monster<VemTabook>().ToMutable(), null)
        ];
	}

    public override bool IsValidForAct(ActModel act)
    {
        return act is Hive || act is Glory;
    }
}
