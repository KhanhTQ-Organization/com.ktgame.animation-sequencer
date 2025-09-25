using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class PlaySequenceAnimationStep : AnimationStepBase
	{
		public override string DisplayName => "Play Sequence";

		[SerializeField] private AnimationSequencerController _sequencer;
		public AnimationSequencerController Sequencer
		{
			get => _sequencer;
			set => _sequencer = value;
		}

		public override void AddTweenToSequence(Sequence animationSequence)
		{
			Sequence sequence = _sequencer.GenerateSequence();
			sequence.SetDelay(Delay);
			if (FlowType == FlowType.Join)
			{
				animationSequence.Join(sequence);
			}
			else
			{
				animationSequence.Append(sequence);
			}
		}

		public override void ResetToInitialState()
		{
			_sequencer.ResetToInitialState();
		}

		public override string GetDisplayNameForEditor(int index)
		{
			string display = "NULL";
			if (_sequencer != null)
			{
				display = _sequencer.name;
			}

			return $"{index}. Play {display} Sequence";
		}

		public void SetTarget(AnimationSequencerController newTarget)
		{
			_sequencer = newTarget;
		}
	}
}