using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class FillImageDOTweenAction : DOTweenActionBase
	{
		public override Type TargetComponentType => typeof(Image);
		public override string DisplayName => "Fill Amount";

		[SerializeField, Range(0, 1)] private float _fillAmount;
		public float FillAmount
		{
			get => _fillAmount;
			set => _fillAmount = Mathf.Clamp01(value);
		}

		private Image _image;
		private float _previousFillAmount;

		protected override Tweener GenerateTween_Internal(GameObject target, float duration)
		{
			if (_image == null)
			{
				_image = target.GetComponent<Image>();
				if (_image == null)
				{
					Debug.LogError($"{target} does not have {TargetComponentType} component");
					return null;
				}
			}

			_previousFillAmount = _image.fillAmount;
			TweenerCore<float, float, FloatOptions> tween = _image.DOFillAmount(_fillAmount, duration);
			return tween;
		}

		public override void ResetToInitialState()
		{
			if (_image == null)
			{
				return;
			}

			_image.fillAmount = _previousFillAmount;
		}
	}
}