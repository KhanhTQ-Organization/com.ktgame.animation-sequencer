using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
    [Serializable]
    public sealed class DOTweenAnimationStep : GameObjectAnimationStep
    {
        public override string DisplayName => "Tween Target";
        [SerializeField] private int _loopCount;
        public int LoopCount
        {
            get => _loopCount;
            set => _loopCount = value;
        }

        [SerializeField] private LoopType _loopType;
        public LoopType LoopType
        {
            get => _loopType;
            set => _loopType = value;
        }

        [SerializeReference] private DOTweenActionBase[] _actions;
        public DOTweenActionBase[] Actions
        {
            get => _actions;
            set => _actions = value;
        }

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < _actions.Length; i++)
            {
                Tween tween = _actions[i].GenerateTween(_target, _duration);
                if (i == 0)
                {
                    tween.SetDelay(Delay);
                }
                sequence.Join(tween);
            }

            sequence.SetLoops(_loopCount, _loopType);
            
            if (FlowType == FlowType.Join)
            {
                animationSequence.Join(sequence);
            }
            else
            {
                animationSequence.Append(sequence);
            }
        }

        public override void ResetToInitialState()
        {
            for (int i = _actions.Length - 1; i >= 0; i--)
            {
                _actions[i].ResetToInitialState();
            }
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string targetName = "NULL";
            if (_target != null)
            {
                targetName = _target.name;
            }

            return $"{index}. {targetName}: {String.Join(", ", _actions.Select(action => action.DisplayName)).Truncate(45)}";
        }

        public bool TryGetActionAtIndex<T>(int index, out T result) where T: DOTweenActionBase
        {
            if (index < 0 || index > _actions.Length - 1)
            {
                result = null;
                return false;
            }

            result = _actions[index] as T;
            return result != null;
        }
    }
}