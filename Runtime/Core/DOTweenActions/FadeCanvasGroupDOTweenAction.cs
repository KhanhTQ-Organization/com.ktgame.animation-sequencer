using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class FadeCanvasGroupDOTweenAction : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(CanvasGroup);

		public override string DisplayName => "Fade Canvas Group";

		[SerializeField] private float _alpha;
		public float Alpha
		{
			get => _alpha;
			set => _alpha = value;
		}

		private CanvasGroup _canvasGroup;
		private float _previousFade;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			if (_canvasGroup == null)
			{
				_canvasGroup = target.GetComponent<CanvasGroup>();

				if (_canvasGroup == null)
				{
					Debug.LogError($"{target} does not have {TargetComponentType} component");
					return null;
				}
			}

			_previousFade = _canvasGroup.alpha;
			TweenerCore<float, float, FloatOptions> canvasTween = _canvasGroup.DOFade(_alpha, duration);
			return canvasTween;
		}

		public override void ResetToInitialState()
		{
			if (_canvasGroup == null)
			{
				return;
			}

			_canvasGroup.alpha = _previousFade;
		}
	}
}