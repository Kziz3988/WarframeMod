
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Relics;
using WarframeMod.Code.Relics.Event;

namespace WarframeMod.Code.Events;

public sealed class MaroosBazaar : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "maroos_bazaar.png".EventBackgroundPath();

    public override ActModel[] Acts => [ModelDb.Act<Overgrowth>(), ModelDb.Act<Underdocks>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("PriceOfRelic", 25m),
        new DynamicVar("PriceOfAya", 5m),
    ];

    public override bool IsAllowed(IRunState runState)
	{
		return runState.Players.All(p => p.Gold >= base.DynamicVars["PriceOfRelic"].BaseValue);
	}

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = [];
		list.Add(new EventOption(this, RandomRelic, "WARFRAMEMOD-MAROOS_BAZAAR.pages.INITIAL.options.RANDOM_RELIC", VoidRelic.GetStaticHovertip()));
		list.Add(new EventOption(this, Aya, "WARFRAMEMOD-MAROOS_BAZAAR.pages.INITIAL.options.AYA"));
        return list;
    }

    private async Task RandomRelic()
    {
        await PlayerCmd.LoseGold(base.DynamicVars["PriceOfRelic"].BaseValue, base.Owner, GoldLossType.Spent);
        List<VoidRelic> relics = 
        [
            ModelDb.Relic<Axi>(),
            ModelDb.Relic<Lith>(),
            ModelDb.Relic<Meso>(),
            ModelDb.Relic<Neo>(),
            ModelDb.Relic<Requiem>()
        ];

        await RelicCmd.Obtain(base.Rng.NextItem(relics).ToMutable(), base.Owner);
        SetEventFinished(L10NLookup("WARFRAMEMOD-MAROOS_BAZAAR.pages.TOOK_A_RELIC.description"));
    }

    private async Task Aya()
    {
        await PlayerCmd.LoseGold(base.DynamicVars["PriceOfAya"].BaseValue, base.Owner, GoldLossType.Spent);
        await RelicCmd.Obtain<Drippy>(base.Owner);
        SetEventFinished(L10NLookup("WARFRAMEMOD-MAROOS_BAZAAR.pages.YOU_ARE_FOOLED.description"));
    }
}