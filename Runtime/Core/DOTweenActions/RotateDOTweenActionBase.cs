using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public abstract class RotateDOTweenActionBase : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);

		public override string DisplayName => "Punch Scale";

		[SerializeField] private bool _local;
		public bool Local
		{
			get => _local;
			set => _local = value;
		}

		[SerializeField] private RotateMode _rotationMode = RotateMode.Fast;
		public RotateMode RotationMode
		{
			get => _rotationMode;
			set => _rotationMode = value;
		}


		private Transform _previousTarget;
		private Quaternion _previousRotation;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			_previousTarget = target.transform;
			TweenerCore<Quaternion, Vector3, QuaternionOptions> localTween;
			if (_local)
			{
				_previousRotation = target.transform.localRotation;
				localTween = target.transform.DOLocalRotate(GetRotation(), duration, _rotationMode);
			}
			else
			{
				_previousRotation = target.transform.rotation;
				localTween = target.transform.DORotate(GetRotation(), duration, _rotationMode);
			}

			return localTween;
		}

        
		protected abstract Vector3 GetRotation();

		public override void ResetToInitialState()
		{
			if (_previousTarget == null)
			{
				return;
			}

			if (!_local)
			{
				_previousTarget.rotation = _previousRotation;
			}
			else
			{
				_previousTarget.localRotation = _previousRotation;
			}
		}
	}
}