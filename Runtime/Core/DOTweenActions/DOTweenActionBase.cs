using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public abstract class DOTweenActionBase
	{
		public enum AnimationDirection
		{
			To, 
			From
		}
        
        
		[SerializeField] protected AnimationDirection _direction;
		public AnimationDirection Direction
		{
			get => _direction;
			set => _direction = value;
		}

		[SerializeField] protected CustomEase _ease = CustomEase.InOutCirc;
		public CustomEase Ease
		{
			get => _ease;
			set => _ease = value;
		}

		[SerializeField] protected bool _isRelative;
		public bool IsRelative
		{
			get => _isRelative;
			set => _isRelative = value;
		}

		public virtual Type TargetComponentType { get; }
		public abstract string DisplayName { get; }

		protected abstract Tweener GenerateTween_Internal(GameObject target, float duration);

		public Tween GenerateTween(GameObject target, float duration)
		{
			Tweener tween = GenerateTween_Internal(target, duration);
			if (_direction == AnimationDirection.From)
				// tween.SetRelative() does not work for From variant of "Move To Anchored Position", it must be set
				// here instead. Not sure if this is a bug in DOTween or expected behaviour...
				tween.From(isRelative: _isRelative);

			tween.SetEase(_ease);
			tween.SetRelative(_isRelative);
			return tween;
		}

		public abstract void ResetToInitialState();
	}
}