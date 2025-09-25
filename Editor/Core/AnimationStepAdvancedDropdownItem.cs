using System;
using UnityEditor.IMGUI.Controls;

namespace com.ktgame.animation_sequencer.editor
{
	public sealed class AnimationStepAdvancedDropdownItem : AdvancedDropdownItem
	{
		private readonly Type _animationStepType;
		public Type AnimationStepType => _animationStepType;

		public AnimationStepAdvancedDropdownItem(AnimationStepBase animationStepBase, string displayName) : base(displayName)
		{
			_animationStepType = animationStepBase.GetType();
		}
	}
}