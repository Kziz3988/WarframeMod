using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Powers.Buff;

public sealed class RequiemMurmurPower : WarframeModPower
{
	private static IReadOnlyList<CardType>? _validCardTypes;

    private List<CardType>? _correctOrder;

    private int _currentSequenceIndex = 0;

    public static IReadOnlyList<CardType> ValidCardTypes
    {
        get
        {
            if (_validCardTypes == null)
            {
                _validCardTypes = [
                    CardType.Attack,
                    CardType.Skill
                ];
            }
            return _validCardTypes;
        }
    }

    public List<CardType> CorrectOrder
    {
        get
        {
            if (_correctOrder == null)
            {
                _correctOrder = [
                    CardType.Attack,
                    CardType.Skill
                ];
            }
            return _correctOrder;
        }
        set
        {
            _correctOrder = value;
        }
    }

    private List<CardType> GenerateRequiemSequence(int length = 3)
    {
        List<CardType> sequence = [];
        for (int i = 0; i < length; i++)
        {
            sequence.Add(base.Owner.Monster?.Rng.NextItem(ValidCardTypes) ?? ValidCardTypes[0]);
        }
        return sequence;
    }

    private async Task RemoveAllRequiemCorrectPowers()
    {
        List<PowerModel> powers = [];
        foreach (PowerModel power in base.Owner.Powers)
        {
            if (power.GetType() == typeof(RequiemCorrectPower))
            {
                powers.Add(power);
            }
        }
        foreach (PowerModel power in powers)
        {
            await PowerCmd.Remove(power);
        }
    }
    
    public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.None;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<IntangiblePower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
	];

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        CorrectOrder = GenerateRequiemSequence();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack && cardPlay.Card.Type != CardType.Skill)
        {
            return;
        }
        Flash();
        if (cardPlay.Card.Type == CorrectOrder[_currentSequenceIndex])
        {
            if (_currentSequenceIndex == CorrectOrder.Count - 1)
            {
                await RemoveAllRequiemCorrectPowers();
                await PowerCmd.Remove(this);
            }
            else
            {
                RequiemCorrectPower power = await PowerCmd.Apply<RequiemCorrectPower>(choiceContext, base.Owner, _currentSequenceIndex + 1, base.Owner, null);
                power.RequiemCardType = CorrectOrder[_currentSequenceIndex].ToLocString();
                _currentSequenceIndex++;
            }
        }
        else
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, 1m, base.Owner, null);
            _currentSequenceIndex = 0;
            await RemoveAllRequiemCorrectPowers();
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Apply<IntangiblePower>(choiceContext, base.Owner, 1m, base.Owner, null);
            _currentSequenceIndex = 0;
            await RemoveAllRequiemCorrectPowers();
        }
    }
}
