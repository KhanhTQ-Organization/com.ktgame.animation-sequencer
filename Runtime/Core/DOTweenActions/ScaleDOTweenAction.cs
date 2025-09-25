using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class ScaleDOTweenAction : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);
		public override string DisplayName => "Scale to Size";

		[SerializeField] private Vector3 _scale;
		public Vector3 Scale
		{
			get => _scale;
			set => _scale = value;
		}

		[SerializeField] private AxisConstraint _axisConstraint;
		public AxisConstraint AxisConstraint
		{
			get => _axisConstraint;
			set => _axisConstraint = value;
		}

		private Vector3? _previousState;
		private GameObject _previousTarget;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			_previousState = target.transform.localScale;
			_previousTarget = target;
            
			TweenerCore<Vector3, Vector3, VectorOptions> scaleTween = target.transform.DOScale(_scale, duration).SetEase(_ease);
			scaleTween.SetOptions(_axisConstraint);

			return scaleTween;
		}

		public override void ResetToInitialState()
		{
			if (!_previousState.HasValue)
			{
				return;
			}

			_previousTarget.transform.localScale = _previousState.Value;
		}
	}
}