using System;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public abstract class GameObjectAnimationStep : AnimationStepBase
	{
		[SerializeField] protected GameObject _target;
		public GameObject Target
		{
			get => _target;
			set => _target = value;
		}

		[SerializeField] protected float _duration = 1;
		public float Duration
		{
			get => _duration;
			set => _duration = value;
		}

		public void SetTarget(GameObject newTarget)
		{
			_target = newTarget;
		}
	}
}