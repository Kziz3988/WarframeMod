using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Cards.Rare;

public class Virulence() : WarframeModCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int InitialDamage = 4;
    private int _currentDamage = InitialDamage;
    private int _doubling = 1;
    private int _counter = 5;

    [SavedProperty]
    public int CurrenctDamage
	{
		get
		{
			return _currentDamage;
		}
		set
		{
			AssertMutable();
			_currentDamage = value;
            base.DynamicVars.Damage.BaseValue = _currentDamage;
		}
	}

    [SavedProperty]
	public int Doubling
	{
		get
		{
			return _doubling;
		}
		set
		{
			AssertMutable();
			_doubling = value;
		}
	}

    [SavedProperty]
	public int Counter
	{
		get
		{
			return _counter;
		}
		set
		{
			AssertMutable();
			_counter = value;
            base.DynamicVars["Countdown"].BaseValue = _counter;
		}
	}

    private void Countdown()
    {
        UpdateCounter();
        Counter--;
        if (Counter == 0)
        {
            Counter = base.DynamicVars["Requirement"].IntValue;
            Doubling *= 2;
            UpdateDamage();
        }
    }

    private void UpdateCounter()
    {
        Counter = Math.Min(Counter, base.DynamicVars["Requirement"].IntValue);
    }

    private void UpdateDamage()
    {
        CurrenctDamage = InitialDamage * Doubling;
        base.DynamicVars.Damage.BaseValue = CurrenctDamage;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrenctDamage, ValueProp.Move),
        new EnergyVar(1),
		new DynamicVar("Requirement", 5m),
        new DynamicVar("Countdown", Counter)

	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        Countdown();
        (base.DeckVersion as Virulence)?.Countdown();
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Requirement"].UpgradeValueBy(-1m);
        UpdateCounter();
    }
}
