using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public class RectTransformSizeDOTweenAction : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(RectTransform);
		public override string DisplayName => "RectTransform Size";

		[SerializeField] private Vector2 _sizeDelta;
		public Vector2 SizeDelta
		{
			get => _sizeDelta;
			set => _sizeDelta = value;
		}

		[SerializeField] private AxisConstraint _axisConstraint;
		public AxisConstraint AxisConstraint
		{
			get => _axisConstraint;
			set => _axisConstraint = value;
		}


		private RectTransform _previousTarget;
		private Vector2 _previousSize;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			_previousTarget = target.transform as RectTransform;
			_previousSize = _previousTarget.sizeDelta;
			var tween = _previousTarget.DOSizeDelta(_sizeDelta, duration);
			tween.SetOptions(_axisConstraint);

			return tween;
		}

		public override void ResetToInitialState()
		{
			if (_previousTarget == null)
			{
				return;
			}

			_previousTarget.sizeDelta = _previousSize;
		}
	}
}