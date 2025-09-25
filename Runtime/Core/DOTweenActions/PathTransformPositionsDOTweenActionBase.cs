using System;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class PathTransformPositionsDOTweenActionBase : PathDOTweenActionBase
	{
		[SerializeField] private Transform[] _pointPositions;
		public Transform[] PointPositions
		{
			get => _pointPositions;
			set => _pointPositions = value;
		}

		public override string DisplayName => "Move to Path Transform Positions";
        
		protected override Vector3[] GetPathPositions()
		{
			Vector3[] result = new Vector3[_pointPositions.Length];

			for (int i = 0; i < _pointPositions.Length; i++)
			{
				Transform pointTransform = _pointPositions[i];

				if (_isLocal)
				{
					result[i] = pointTransform.localPosition;
				}
				else
				{
					result[i] = pointTransform.position;
				}
			}

			return result;
		}
	}
}