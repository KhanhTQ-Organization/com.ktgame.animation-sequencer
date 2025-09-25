using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class PunchPositionDOTweenAction : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);
		public override string DisplayName => "Punch Position";

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

		[SerializeField] private bool _snapping;
		public bool Snapping
		{
			get => _snapping;
			set => _snapping = value;
		}

		private Transform previousTarget;
		private Vector3 previousPosition;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			previousTarget = target.transform;
			previousPosition = target.transform.position;
			Tweener tween = target.transform.DOPunchPosition(_punch, duration, _vibrato, _elasticity, _snapping);

			return tween;
		}

		public override void ResetToInitialState()
		{
			if (previousTarget == null)
			{
				return;
			}

			previousTarget.position = previousPosition;
		}
	}
}