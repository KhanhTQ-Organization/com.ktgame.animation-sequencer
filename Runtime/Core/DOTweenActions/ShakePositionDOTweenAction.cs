using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class ShakePositionDOTweenAction : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);
		public override string DisplayName => "Shake Position";

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

		[SerializeField] private bool _snapping;
		public bool Snapping
		{
			get => _snapping;
			set => _snapping = value;
		}

		[SerializeField] private bool _fadeout = true;
		public bool Fadeout
		{
			get => _fadeout;
			set => _fadeout = value;
		}

		private Transform _previousTarget;
		private Vector3 _previousPosition;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			_previousTarget = target.transform;
			_previousPosition = _previousTarget.position;
			Tweener tween = target.transform.DOShakePosition(duration, _strength, _vibrato, _randomness, _snapping, _fadeout);

			return tween;
		}

		public override void ResetToInitialState()
		{
			if (_previousTarget == null)
			{
				return;
			}

			_previousTarget.position = _previousPosition;
		}
	}
}