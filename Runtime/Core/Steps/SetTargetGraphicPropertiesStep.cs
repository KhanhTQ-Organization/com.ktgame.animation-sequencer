using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class SetTargetGraphicPropertiesStep : AnimationStepBase
	{
		[SerializeField] private Graphic _targetGraphic;
		[SerializeField] private Color _targetColor = Color.white;

		private Color _originalColor;
        
		public override string DisplayName => "Set Target Graphic Properties";
		public override void AddTweenToSequence(Sequence animationSequence)
		{
			Sequence behaviourSequence = DOTween.Sequence();
			behaviourSequence.SetDelay(Delay);

			behaviourSequence.AppendCallback(() =>
			{
				_originalColor = _targetGraphic.color; 
				_targetGraphic.color = _targetColor;
			});
			if (FlowType == FlowType.Join)
			{
				animationSequence.Join(behaviourSequence);
			}
			else
			{
				animationSequence.Append(behaviourSequence);
			}
		}

		public override void ResetToInitialState()
		{
			_targetGraphic.color = _originalColor;
		}
        
        
		public override string GetDisplayNameForEditor(int index)
		{
			string display = "NULL";
			if (_targetGraphic != null)
			{
				display = _targetGraphic.name;
			}

			return $"{index}. Set {display} Properties";
		} 
	}
}