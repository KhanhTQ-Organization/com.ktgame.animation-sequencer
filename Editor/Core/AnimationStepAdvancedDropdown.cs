using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace com.ktgame.animation_sequencer.editor
{
	public sealed class AnimationStepAdvancedDropdown : AdvancedDropdown
	{
		private Action<AnimationStepAdvancedDropdownItem> _callBack;

		public AnimationStepAdvancedDropdown(AdvancedDropdownState state) : base(state)
		{
			this.minimumSize = new Vector2(200, 300);
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			AdvancedDropdownItem root = new AdvancedDropdownItem("Animation Step");

			TypeCache.TypeCollection availableTypesOfAnimationStep = TypeCache.GetTypesDerivedFrom(typeof(AnimationStepBase));
			foreach (Type animatedItemType in availableTypesOfAnimationStep)
			{
				if (animatedItemType.IsAbstract)
				{
					continue;
				}

				AnimationStepBase animationStepBase = Activator.CreateInstance(animatedItemType) as AnimationStepBase;

				string displayName = animationStepBase.GetType().Name;
				if (!string.IsNullOrEmpty(animationStepBase.DisplayName))
				{
					displayName = animationStepBase.DisplayName;
				}

				root.AddChild(new AnimationStepAdvancedDropdownItem(animationStepBase, displayName));
			}

			return root;
		}

		protected override void ItemSelected(AdvancedDropdownItem item)
		{
			base.ItemSelected(item);
			_callBack?.Invoke(item as AnimationStepAdvancedDropdownItem);
		}

		public void Show(Rect rect, Action<AnimationStepAdvancedDropdownItem> onItemSelectedCallback)
		{
			_callBack = onItemSelectedCallback;
			base.Show(rect);
		}
	}
}