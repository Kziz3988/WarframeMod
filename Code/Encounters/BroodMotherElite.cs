using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Monsters;

namespace WarframeMod.Code.Encounters;

public sealed class BroodMotherElite() : CustomEncounterModel(RoomType.Elite)
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [
        ModelDb.Monster<BroodMother>(),
        ModelDb.Monster<Maggot>()
    ];
    
    public override bool HasScene => true;
    public override IReadOnlyList<string> Slots => ["brood_mother", "maggot0", "maggot1", "maggot2", "maggot3", "maggot4", "maggot5", "maggot6", "maggot7"];
    public override string? CustomScenePath => "brood_mother_elite.tscn".EncounterScenePath();

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return [
            (ModelDb.Monster<BroodMother>().ToMutable(), "brood_mother")
        ];
	}

    public override bool IsValidForAct(ActModel act)
    {
        return act is Hive;
    }
}
