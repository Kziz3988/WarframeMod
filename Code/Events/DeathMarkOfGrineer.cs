
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Encounters;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Relics.Ancient;

namespace WarframeMod.Code.Events;

public sealed class DeathMarkOfGrineer : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "death_mark_of_grineer.png".EventBackgroundPath();

    public override bool IsShared => true;

    public override ActModel[] Acts => [ModelDb.Act<Hive>(), ModelDb.Act<Glory>()];

    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.All(p => p.Creature.CurrentHp >= 50);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = [];
		list.Add(new EventOption(this, Fight, "WARFRAMEMOD-DEATH_MARK_OF_GRINEER.pages.INITIAL.options.FIGHT"));
		list.Add(new EventOption(this, Run, "WARFRAMEMOD-DEATH_MARK_OF_GRINEER.pages.INITIAL.options.RUN", HoverTipFactory.FromRelic<GrustragBolt>()));
        return list;
    }

    private Task Fight()
    {
		List<Reward> list = new List<Reward>();
		foreach (Player player in base.Owner.RunState.Players)
		{
			list.Add(new RelicReward(RelicRarity.Rare, player));
		}
		EnterCombatWithoutExitingEvent<TheGrustragThree>(list, shouldResumeAfterCombat: false);
		return Task.CompletedTask;
    }

    private async Task Run()
    {
        await RelicCmd.Obtain<GrustragBolt>(base.Owner);
        SetEventFinished(L10NLookup("WARFRAMEMOD-DEATH_MARK_OF_GRINEER.pages.RUN_AWAY.description"));
    }
}