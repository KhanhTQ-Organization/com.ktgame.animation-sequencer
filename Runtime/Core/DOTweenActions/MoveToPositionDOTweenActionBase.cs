using System;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class MoveToPositionDOTweenActionBase : MoveDOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);

		[SerializeField] private Vector3 _position;
		public Vector3 Position
		{
			get => _position;
			set => _position = value;
		}

		public override string DisplayName => "Move To Position";

		protected override Vector3 GetPosition()
		{
			return _position;
		}
	}
}