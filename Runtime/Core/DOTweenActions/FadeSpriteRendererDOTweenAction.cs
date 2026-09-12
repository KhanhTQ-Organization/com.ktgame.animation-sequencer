#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class FadeSpriteRendererDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(SpriteRenderer);
        public override string DisplayName => "Fade SpriteRenderer";

        [SerializeField]
        private float alpha = 1f;
        public float Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        private SpriteRenderer targetSpriteRenderer;
        private float previousAlpha;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (targetSpriteRenderer == null)
            {
                targetSpriteRenderer = target.GetComponent<SpriteRenderer>();
                if (targetSpriteRenderer == null)
                {
                    Debug.LogError($"'{target}' does not have {TargetComponentType} component");
                    return null;
                }
            }

            previousAlpha = targetSpriteRenderer.color.a;
            TweenerCore<Color, Color, ColorOptions> spriteTween = targetSpriteRenderer.DOFade(alpha, duration);
            
#if UNITY_EDITOR 
            if (!Application.isPlaying)
            {
                spriteTween.OnUpdate(() =>
                {
                    targetSpriteRenderer.enabled = false;
                    targetSpriteRenderer.enabled = true;
                });
            }
#endif
                
            return spriteTween;
        }

        public override void ResetToInitialState()
        {
            if (targetSpriteRenderer == null)
                return;

            Color color = targetSpriteRenderer.color;
            color.a = previousAlpha;
            targetSpriteRenderer.color = color;
        }
    }
}
#endif
