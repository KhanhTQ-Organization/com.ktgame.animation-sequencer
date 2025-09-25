#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public abstract class AnimationStepBase
	{
		[SerializeField] private float _delay;
		public float Delay => _delay;

		[SerializeField] private FlowType _flowType;
		public FlowType FlowType => _flowType;

		public abstract string DisplayName { get; }
        
		public abstract void AddTweenToSequence(Sequence animationSequence);

		public abstract void ResetToInitialState();

		public virtual string GetDisplayNameForEditor(int index)
		{
			return $"{index}. {this}";
		}
	}
}
#endif