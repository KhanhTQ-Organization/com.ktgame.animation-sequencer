using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class PunchScaleDOTweenAction : DOTweenActionBase
	{
		public override string DisplayName => "Punch Scale";
		public override Type TargetComponentType => typeof(Transform);

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
		private Vector3 _previousScale;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			_previousTarget = target.transform;
			_previousScale = _previousTarget.localScale;
            
			Tweener tween = target.transform.DOPunchScale(_punch, duration, _vibrato, _elasticity);

			return tween;
		}

		public override void ResetToInitialState()
		{
			if (_previousTarget == null)
			{
				return;
			}

			_previousTarget.localScale = _previousScale;
		}
	}
}