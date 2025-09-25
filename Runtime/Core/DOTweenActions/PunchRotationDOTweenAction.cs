using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class PunchRotationDOTweenAction : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);
		public override string DisplayName => "Punch Rotation";

		[SerializeField] private Vector3 _punch;
		public Vector3 Punch
		{
			get => _punch;
			set => _punch = value;
		}

		[SerializeField] private int _vibrato = 10;
		public int Vibrato
		{
			get => _vibrato;
			set => _vibrato = value;
		}

		[SerializeField] private float _elasticity = 1f;
		public float Elasticity
		{
			get => _elasticity;
			set => _elasticity = value;
		}

		private Transform _previousTarget;
		private Quaternion _previousRotation;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			_previousTarget = target.transform;
			_previousRotation = target.transform.rotation;
			Tweener tween = target.transform.DOPunchRotation(_punch, duration, _vibrato, _elasticity);

			return tween;
		}

		public override void ResetToInitialState()
		{
			if (_previousTarget == null)
			{
				return;
			}

			_previousTarget.rotation = _previousRotation;
		}
	}
}