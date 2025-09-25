using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class ShakeRotationDOTweenAction : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);
		public override string DisplayName => "Shake Rotation";

		[SerializeField] private Vector3 _strength;
		public Vector3 Strength
		{
			get => _strength;
			set => _strength = value;
		}

		[SerializeField] private int _vibrato = 10;
		public int Vibrato
		{
			get => _vibrato;
			set => _vibrato = value;
		}

		[SerializeField] private float _randomness = 90;
		public float Randomness
		{
			get => _randomness;
			set => _randomness = value;
		}

		[SerializeField] private bool _fadeout = true;
		public bool Fadeout
		{
			get => _fadeout;
			set => _fadeout = value;
		}


		private Transform _previousTarget;
		private Quaternion _previousRotation;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			_previousTarget = target.transform;
			_previousRotation = _previousTarget.rotation;
            
			Tweener tween = _previousTarget.DOShakeRotation(duration, _strength, _vibrato, _randomness, _fadeout);

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