using System;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class RotateToEulerAnglesRotateDOTweenAction : RotateDOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);

		public override string DisplayName => "Rotate to Euler Angles";

		[SerializeField] private Vector3 _eulerAngles;
		public Vector3 EulerAngles
		{
			get => _eulerAngles;
			set => _eulerAngles = value;
		}

		protected override Vector3 GetRotation()
		{
			return _eulerAngles;
		}
	}
}
