using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace com.ktgame.animation_sequencer
{
    [Serializable]
    public sealed class SetTargetImagePropertiesStep : AnimationStepBase
    {
        [SerializeField] private Image _targetGraphic;
        [SerializeField] private Color _targetColor = Color.white;
        [SerializeField] private Sprite _targetSprite;
        [SerializeField] private Material _targetMaterial;

        private Color _originalColor = Color.white;
        private Material _originalMaterial;
        private Sprite _originalSprite;

        public override string DisplayName => "Set Target Image Properties";
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = DOTween.Sequence();
            behaviourSequence.SetDelay(Delay);

            behaviourSequence.AppendCallback(() =>
            {

                if (_targetColor != _targetGraphic.color)
                {
                    _originalColor = _targetGraphic.color;
                    _targetGraphic.color = _targetColor;
                }

                if (_targetSprite != null)
                {
                    _originalSprite = _targetGraphic.sprite; 
                    _targetGraphic.sprite = _targetSprite;
                }

                if (_targetMaterial != null)
                {
                    _originalMaterial = _targetGraphic.material;
                    _targetGraphic.material = _targetMaterial;
                }


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
            
            if(_originalSprite != null)
            {
                _targetGraphic.sprite = _originalSprite;
            }

            if (_originalMaterial != null)
            {
                _targetGraphic.material = _targetMaterial;
            }
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (_targetGraphic != null)
            {
                display = _targetGraphic.name;
            }

            return $"{index}. Set {display}(Image) Properties";
        } 
    }
}