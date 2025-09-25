using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace com.ktgame.animation_sequencer
{
    [Serializable]
    public sealed class SetTargetTransformPropertiesStep : AnimationStepBase
    {
        public override string DisplayName => "Set Target Transform Properties";
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private bool _useLocal;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _eulerAngles;
        [SerializeField] private Vector3 _scale = Vector3.one;

        private Vector3 _originalPosition;
        private Vector3 _originalEulerAngles;
        private Vector3 _originalScale;
        public SetTargetTransformPropertiesStep(Vector3 originalScale)
        {
            _originalScale = originalScale;
        }

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = DOTween.Sequence();
            behaviourSequence.SetDelay(Delay);

            behaviourSequence.AppendCallback(() =>
            {
                if (_useLocal)
                {
                    _originalPosition = _targetTransform.localPosition;
                    _originalEulerAngles = _targetTransform.localEulerAngles;
                    
                    _targetTransform.localPosition = _position;
                    _targetTransform.localEulerAngles = _eulerAngles;
                }
                else
                {
                    _originalPosition = _targetTransform.position;
                    _originalEulerAngles = _targetTransform.eulerAngles;
                    
                    _targetTransform.position = _position;
                    _targetTransform.eulerAngles = _eulerAngles;
                }

                _originalScale = _targetTransform.localScale; 
                _targetTransform.localScale = _scale;
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
            if (_useLocal)
            {
                _targetTransform.localPosition = _originalPosition;
                _targetTransform.localEulerAngles = _originalEulerAngles;
            }
            else
            {
                _targetTransform.position = _originalPosition;
                _targetTransform.eulerAngles = _originalEulerAngles;
            }
            _targetTransform.localScale = _originalScale;
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (_targetTransform != null)
            {
                display = _targetTransform.name;
            }

            return $"{index}. Set {display} Transform Properties";
        }   
    }
}