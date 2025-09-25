using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public abstract class MoveDOTweenActionBase : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);

		[SerializeField] private bool _localMove;
		public bool LocalMove
		{
			get => _localMove;
			set => _localMove = value;
		}

		[SerializeField] private AxisConstraint _axisConstraint;
		public AxisConstraint AxisConstraint
		{
			get => _axisConstraint;
			set => _axisConstraint = value;
		}

		private Vector3 _previousPosition;
		private GameObject _previousTarget;

		public override string DisplayName => "Move to Position";

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> moveTween;
			_previousTarget = target;
			if (_localMove)
			{
				_previousPosition = target.transform.localPosition;
				moveTween = target.transform.DOLocalMove(GetPosition(), duration);
                
			}
			else
			{
				_previousPosition = target.transform.position;
				moveTween = target.transform.DOMove(GetPosition(), duration);
			}

			moveTween.SetOptions(_axisConstraint);
			return moveTween;
		}

		protected abstract Vector3 GetPosition();

		public override void ResetToInitialState()
		{
			if (_previousTarget == null)
			{
				return;
			}

			if (_localMove)
			{
				_previousTarget.transform.localPosition = _previousPosition;
			}
			else
			{
				_previousTarget.transform.position = _previousPosition;
			}
		}
	}
}