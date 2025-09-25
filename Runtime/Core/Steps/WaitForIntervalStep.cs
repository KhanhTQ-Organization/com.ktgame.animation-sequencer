using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class WaitForIntervalStep : AnimationStepBase
	{
		public override string DisplayName => "Wait for Interval";

		[SerializeField] private float _interval;
		public float Interval
		{
			get => _interval;
			set => _interval = value;
		}

		public override void AddTweenToSequence(Sequence animationSequence)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetDelay(Delay);

			sequence.AppendInterval(_interval);
            
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
		}

		public override string GetDisplayNameForEditor(int index)
		{
			return $"{index}. Wait {_interval} seconds";
		}
	}
}