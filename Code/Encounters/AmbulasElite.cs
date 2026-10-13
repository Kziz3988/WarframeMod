using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Monsters;

namespace WarframeMod.Code.Encounters;

public sealed class AmbulasElite() : CustomEncounterModel(RoomType.Elite)
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [
        ModelDb.Monster<Ambulas>(),
        ModelDb.Monster<RemechOsprey>()
    ];
    
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return [
            (ModelDb.Monster<Ambulas>().ToMutable(), null)
        ];
	}

    public override bool IsValidForAct(ActModel act)
    {
        return act is Glory;
    }
}
