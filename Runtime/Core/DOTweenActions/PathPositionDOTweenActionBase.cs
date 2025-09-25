using System;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class PathPositionDOTweenActionBase : PathDOTweenActionBase
	{
		[SerializeField] private Vector3[] _positions;
		public Vector3[] Positions => _positions;

		public override string DisplayName => "Move to Path Positions" ;

		protected override Vector3[] GetPathPositions()
		{
			return _positions;
		}
	}
}