using System;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class AnchoredPositionMoveToRectTransformPositionDOTweenActionBase : AnchoredPositionMoveDOTweenActionBase
	{
		[SerializeField]
		private RectTransform target;

		public override string DisplayName => "Move to RectTransform Anchored Position";

		protected override Vector2 GetPosition()
		{
			return target.anchoredPosition;
		}
	}
}