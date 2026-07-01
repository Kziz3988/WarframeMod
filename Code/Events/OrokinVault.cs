
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Enchantments;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Events;

public sealed class OrokinVault : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "orokin_vault.png".EventBackgroundPath();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(20m)];
    
    public override ActModel[] Acts => [ModelDb.Act<Glory>()];

	private static EnchantmentModel[] AttackEnchantments =>
	[
		ModelDb.Enchantment<BlindRage>(),
        ModelDb.Enchantment<HeavyCaliber>(),
        ModelDb.Enchantment<Overextended>()
    ];

    private static EnchantmentModel[] SkillEnchantments =>
	[
		ModelDb.Enchantment<DepletedReload>(),
        ModelDb.Enchantment<NarrowMinded>(),
        ModelDb.Enchantment<TaintedMag>()
    ];

    private EnchantmentModel[] GetValidEnchantments(EnchantmentModel[] pool, Player player)
    {
        return pool.Where(e => player.Deck.Cards.Any(c => e.CanEnchant(c))).ToArray();
    }

    private bool HasCard(Player player)
    {
        return GetValidEnchantments(AttackEnchantments, player).Length > 0 || GetValidEnchantments(SkillEnchantments, player).Length > 0;
    }

    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.All(p => HasCard(p));
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = [];

        var attackEnchantments = GetValidEnchantments(AttackEnchantments, base.Owner);
        if (attackEnchantments.Length == 0)
        {
            list.Add(new EventOption(this, null, "WARFRAMEMOD-OROKIN_VAULT.pages.INITIAL.options.CORRUPT_ATTACK_LOCKED"));
        }
        else
        {
            EnchantmentModel enchantment = base.Rng.NextItem(attackEnchantments);
            list.Add(new EventOption(this, () => Corrupt(enchantment), $"WARFRAMEMOD-OROKIN_VAULT.pages.INITIAL.options.{enchantment.Id}", WarframeModEnchantment.GetHoverTips(enchantment.GetType())));
        }

        var skillEnchantments = GetValidEnchantments(SkillEnchantments, base.Owner);
        if (skillEnchantments.Length == 0)
        {
            list.Add(new EventOption(this, null, "WARFRAMEMOD-OROKIN_VAULT.pages.INITIAL.options.CORRUPT_SKILL_LOCKED"));
        }
        else
        {
            EnchantmentModel enchantment = base.Rng.NextItem(skillEnchantments);
            list.Add(new EventOption(this, () => Corrupt(enchantment), $"WARFRAMEMOD-OROKIN_VAULT.pages.INITIAL.options.{enchantment.Id}", WarframeModEnchantment.GetHoverTips(enchantment.GetType())));
        }

        list.Add(new EventOption(this, Leave, "WARFRAMEMOD-OROKIN_VAULT.pages.INITIAL.options.LEAVE"));

        return list;
    }

    private async Task Corrupt(EnchantmentModel enchantment)
    {
        CardModel card = (await CardSelectCmd.FromDeckForEnchantment(base.Owner, enchantment, 1, enchantment.CanEnchant, new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1)))
            .ToList().First();
        CardCmd.Enchant(enchantment.ToMutable(), card, 1m);
        SetEventFinished(L10NLookup("WARFRAMEMOD-OROKIN_VAULT.pages.CORRUPTED.description"));
    }

    private async Task Leave()
    {
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
        SetEventFinished(L10NLookup("WARFRAMEMOD-OROKIN_VAULT.pages.LEAVE.description"));
    }
}