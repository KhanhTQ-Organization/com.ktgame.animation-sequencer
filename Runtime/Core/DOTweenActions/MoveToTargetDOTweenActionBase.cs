using System;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class MoveToTargetDOTweenActionBase : MoveDOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Transform);

		[SerializeField]
		private Transform _target;
		public Transform Target
		{
			get => _target;
			set => _target = value;
		}

		[SerializeField]
		private bool useLocalPosition;
		public bool UseLocalPosition
		{
			get => useLocalPosition;
			set => useLocalPosition = value;
		}

		public override string DisplayName => "Move To Transform Position";

		protected override Vector3 GetPosition()
		{
			if (useLocalPosition)
			{
				return _target.localPosition;
			}

			return _target.position;
		}
	}
}