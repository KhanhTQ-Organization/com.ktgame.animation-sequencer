#if DOTWEEN_ENABLED
#if TMP_ENABLED

using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{

    [Serializable]
    public sealed class TMP_FadeDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(TMP_Text);
        public override string DisplayName => "TMP Fade Text";

        [SerializeField]
        private float alpha;
        public float Alpha
        {
            get => alpha;
            set => alpha = value;
        }
        
        private TMP_Text tmpTextComponent;
        private float previousAlpha;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (tmpTextComponent == null || tmpTextComponent.gameObject != target)
            {
                tmpTextComponent = target.GetComponent<TMP_Text>();
                if (tmpTextComponent == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            previousAlpha = tmpTextComponent.alpha;
            TMP_Text text = tmpTextComponent;
            var tween = DOTween.ToAlpha(() => text.color, value => text.color = value, alpha, duration);
            tween.SetTarget(text);

#if UNITY_EDITOR 
            if (!Application.isPlaying)
            {
                // Work around a Unity bug where updating the colour does not cause any visual change outside of PlayMode.
                // https://forum.unity.com/threads/editor-scripting-force-color-update.798663/
                tween.OnUpdate(() =>
                {
                    text.transform.localScale = new Vector3(1.001f, 1.001f, 1.001f);
                    text.transform.localScale = new Vector3(1, 1, 1);
                });
            }
#endif
            
            return tween;
        }

        public override void ResetToInitialState()
        {
            if (tmpTextComponent == null)
                return;

            tmpTextComponent.alpha = previousAlpha;
        }
    }
}
#endif
#endif