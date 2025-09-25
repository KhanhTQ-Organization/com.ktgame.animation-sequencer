using System;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class RotateToEulerAnglesFromTransformRotateDOTweenAction : RotateDOTweenActionBase
	{
		public override string DisplayName => "Rotate To Transform Euler Angles";

		[SerializeField] private Transform _target;
		public Transform Target
		{
			get => _target;
			set => _target = value;
		}

		[SerializeField] private bool _useLocalEulerAngles;
		public bool UseLocalEulerAngles
		{
			get => _useLocalEulerAngles;
			set => _useLocalEulerAngles = value;
		}
        
		protected override Vector3 GetRotation()
		{
			if (!_useLocalEulerAngles)
			{
				return _target.eulerAngles;
			}

			return _target.localEulerAngles;
		}
	}
}