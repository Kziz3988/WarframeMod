
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Relics.Event;

namespace WarframeMod.Code.RestSite;

public sealed class IncubateRestSiteOption(Player owner) : CustomRestSiteOption(owner)
{
    public override string OptionId => "WARFRAMEMOD-INCUBATE";

    public override string? CustomIconPath => "option_incubate.png".RestSiteUiPath();

    public override async Task<bool> OnSelect()
    {
        await RelicCmd.Obtain<KubrowRelic>(base.Owner);
		return true;
    }
}