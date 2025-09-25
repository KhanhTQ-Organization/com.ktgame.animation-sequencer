using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
    [Serializable]
    public sealed class SetTargetRectTransformPropertiesStep : AnimationStepBase
    {
        public override string DisplayName => "Set Target RectTransform Properties";
        
        [SerializeField] private RectTransform _targetRectTransform;
        
        [SerializeField] private bool _useLocal;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _eulerAngles;
        [SerializeField] private Vector3 _scale = Vector3.one;

        [SerializeField] private Vector2 _anchorMin =  new Vector2(0.5f, 0.5f);
        [SerializeField] private Vector2 _anchorMax =  new Vector2(0.5f, 0.5f);
        [SerializeField] private Vector2 _anchoredPosition;
        [SerializeField] private Vector2 _sizeDelta;
        [SerializeField] private Vector2 _pivot = new Vector2(0.5f, 0.5f);

        private Vector3 _originalPosition;
        private Vector3 _originalEulerAngles;
        private Vector3 _originalScale;
        private Vector2 _originalAnchorMin;
        private Vector2 _originalAnchorMax;
        private Vector2 _originalAnchoredPosition;
        private Vector2 _originalSizeDelta;
        private Vector2 _originalPivot;
        
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = DOTween.Sequence();
            behaviourSequence.SetDelay(Delay);

            behaviourSequence.AppendCallback(() =>
            {
                if (_useLocal)
                {
                    _originalPosition = _targetRectTransform.localPosition;
                    _originalEulerAngles = _targetRectTransform.localEulerAngles;
                    
                    _targetRectTransform.localPosition = _position;
                    _targetRectTransform.localEulerAngles = _eulerAngles;
                }
                else
                {
                    _originalPosition = _targetRectTransform.position;
                    _originalEulerAngles = _targetRectTransform.eulerAngles;
                    
                    _targetRectTransform.position = _position;
                    _targetRectTransform.eulerAngles = _eulerAngles;
                }


                _targetRectTransform.anchorMin = _anchorMin;
                _targetRectTransform.anchorMax = _anchorMax;
                _targetRectTransform.anchoredPosition = _anchoredPosition;
                _targetRectTransform.sizeDelta = _sizeDelta;
                _targetRectTransform.pivot = _pivot;
                
                
                _originalAnchorMin = _targetRectTransform.anchorMin;
                _originalAnchorMax = _targetRectTransform.anchorMax;
                _originalAnchoredPosition = _targetRectTransform.anchoredPosition;
                _originalSizeDelta = _targetRectTransform.sizeDelta;
                _originalPivot = _targetRectTransform.pivot;
                
                _originalScale = _targetRectTransform.localScale; 
                _targetRectTransform.localScale = _scale;
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
                _targetRectTransform.localPosition = _originalPosition;
                _targetRectTransform.localEulerAngles = _originalEulerAngles;
            }
            else
            {
                _targetRectTransform.position = _originalPosition;
                _targetRectTransform.eulerAngles = _originalEulerAngles;
            }
            _targetRectTransform.localScale = _originalScale;
            
            _targetRectTransform.anchorMin = _originalAnchorMin;
            _targetRectTransform.anchorMax = _originalAnchorMax;
            _targetRectTransform.anchoredPosition = _originalAnchoredPosition;
            _targetRectTransform.sizeDelta = _originalSizeDelta;
            _targetRectTransform.pivot = _originalPivot;
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (_targetRectTransform != null)
            {
                display = _targetRectTransform.name;
            }

            return $"{index}. Set {display}(RectTransform) Properties";
        }  

    }
}