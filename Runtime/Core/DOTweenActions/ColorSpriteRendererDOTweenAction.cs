#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ColorSpriteRendererDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(SpriteRenderer);
        public override string DisplayName => "Color SpriteRenderer";

        [SerializeField]
        private Color color = Color.white;

        private SpriteRenderer targetSpriteRenderer;
        private Color previousColor;

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

            previousColor = targetSpriteRenderer.color;
            TweenerCore<Color, Color, ColorOptions> spriteTween = targetSpriteRenderer.DOColor(color, duration);

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

            targetSpriteRenderer.color = previousColor;
        }
    }
}
#endif
