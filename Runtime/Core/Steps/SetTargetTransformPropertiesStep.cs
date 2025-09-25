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
        [FormerlySerializedAs("targetGameObject")] [SerializeField] private Transform targetTransform;
        
        [SerializeField] private bool _useLocal;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _eulerAngles;
        [SerializeField] private Vector3 _scale = Vector3.one;

        private Vector3 _originalPosition;
        private Vector3 _originalEulerAngles;
        private Vector3 _originalScale;
        
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = DOTween.Sequence();
            behaviourSequence.SetDelay(Delay);

            behaviourSequence.AppendCallback(() =>
            {
                if (_useLocal)
                {
                    _originalPosition = targetTransform.localPosition;
                    _originalEulerAngles = targetTransform.localEulerAngles;
                    
                    targetTransform.localPosition = _position;
                    targetTransform.localEulerAngles = _eulerAngles;
                }
                else
                {
                    _originalPosition = targetTransform.position;
                    _originalEulerAngles = targetTransform.eulerAngles;
                    
                    targetTransform.position = _position;
                    targetTransform.eulerAngles = _eulerAngles;
                }

                _originalScale = targetTransform.localScale; 
                targetTransform.localScale = _scale;
            });
            if (FlowType == FlowType.Join)
                animationSequence.Join(behaviourSequence);
            else
                animationSequence.Append(behaviourSequence);
        }

        public override void ResetToInitialState()
        {
            if (_useLocal)
            {
                targetTransform.localPosition = _originalPosition;
                targetTransform.localEulerAngles = _originalEulerAngles;
            }
            else
            {
                targetTransform.position = _originalPosition;
                targetTransform.eulerAngles = _originalEulerAngles;
            }
            targetTransform.localScale = _originalScale;
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (targetTransform != null)
                display = targetTransform.name;
            
            return $"{index}. Set {display} Transform Properties";
        }   
    }
}