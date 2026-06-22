
using System.Linq;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace WarframeMod.Code.Extensions;

public static class MonsterModelExtensions
{
    public static bool IntendsTo(this MonsterModel monster, IntentType intent)
    {
        return monster.NextMove.Intents.Any(delegate(AbstractIntent i)
		{
			IntentType intentType = i.IntentType;
			return intentType == intent;
		});
    }
}