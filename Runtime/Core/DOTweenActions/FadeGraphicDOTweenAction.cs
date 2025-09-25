using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace com.ktgame.animation_sequencer
{
    [Serializable]
    public sealed class FadeGraphicDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Graphic);
        public override string DisplayName => "Fade Graphic";

        [SerializeField] private float _alpha;

        public float Alpha
        {
            get => _alpha;
            set => _alpha = value;
        }

        private Graphic _targetGraphic;
        private float _previousAlpha;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (_targetGraphic == null)
            {
                _targetGraphic = target.GetComponent<Graphic>();
                if (_targetGraphic == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            _previousAlpha = _targetGraphic.color.a;
            TweenerCore<Color, Color, ColorOptions> graphicTween = _targetGraphic.DOFade(_alpha, duration);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Work around a Unity bug where updating the colour does not cause any visual change outside of PlayMode.
                // https://forum.unity.com/threads/editor-scripting-force-color-update.798663/
                graphicTween.OnUpdate(() =>
                {
                    _targetGraphic.enabled = false;
                    _targetGraphic.enabled = true;
                });
            }
#endif

            return graphicTween;
        }

        public override void ResetToInitialState()
        {
            if (_targetGraphic == null)
            {
                return;
            }

            Color color = _targetGraphic.color;
            color.a = _previousAlpha;
            _targetGraphic.color = color;
        }
    }
}