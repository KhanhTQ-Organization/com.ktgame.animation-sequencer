using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
    [Serializable]
    public sealed class TMP_TextDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(TMP_Text);
        public override string DisplayName => "TMP Text";

        [SerializeField] private string _text;
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        [SerializeField] private bool _richText;
        public bool RichText
        {
            get => _richText;
            set => _richText = value;
        }

        [SerializeField] private ScrambleMode _scrambleMode = ScrambleMode.None;
        public ScrambleMode ScrambleMode
        {
            get => _scrambleMode;
            set => _scrambleMode = value;
        }
        
        private TMP_Text _tmpTextComponent;
        private TMP_Text _previousTarget;
        private string _previousText;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (_tmpTextComponent == null)
            {
                _tmpTextComponent = target.GetComponent<TMP_Text>();
                if (_tmpTextComponent == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            _previousText = _tmpTextComponent.text;
            _previousTarget = _tmpTextComponent;
            TweenerCore<string, string, StringOptions> tween = _tmpTextComponent.DOText(_text, duration, _richText, _scrambleMode);
            return tween;
        }

        public override void ResetToInitialState()
        {
            if (_previousTarget == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_previousText))
            {
                return;
            }

            _previousTarget.text = _previousText;
        }
    }
}