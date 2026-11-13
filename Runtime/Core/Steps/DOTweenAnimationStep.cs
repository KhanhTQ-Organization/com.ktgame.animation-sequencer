#if DOTWEEN_ENABLED
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class DOTweenAnimationStep : GameObjectAnimationStep
    {
        public override string DisplayName => "Tween Target";
        [SerializeField]
        private int loopCount;
        public int LoopCount
        {
            get => loopCount;
            set => loopCount = value;
        }

        [SerializeField]
        private LoopType loopType;
        public LoopType LoopType
        {
            get => loopType;
            set => loopType = value;
        }

        [SerializeReference]
        private DOTweenActionBase[] actions;
        public DOTweenActionBase[] Actions
        {
            get => actions;
            set => actions = value;
        }

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = DOTween.Sequence();
            bool delayApplied = false;
            int actionCount = actions != null ? actions.Length : 0;
            for (int i = 0; i < actionCount; i++)
            {
                if (actions[i] == null)
                    continue;

                Tween tween = actions[i].GenerateTween(target, duration);
                if (tween == null)
                    continue;

                if (!delayApplied)
                {
                    tween.SetDelay(Delay);
                    delayApplied = true;
                }
                sequence.Join(tween);
            }

            sequence.SetLoops(loopCount, loopType);
            
            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);

        }

        public override float GetEstimatedLength()
        {
            if (target == null || actions == null)
                return 0f;

            bool hasAction = false;
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] != null)
                {
                    hasAction = true;
                    break;
                }
            }

            if (!hasAction)
                return 0f;

            if (loopCount < 0)
                return float.PositiveInfinity;

            return (Mathf.Max(0f, Delay) + Mathf.Max(0f, duration)) * Mathf.Max(1, loopCount);
        }

        public override void ResetToInitialState()
        {
            if (actions == null)
                return;

            for (int i = actions.Length - 1; i >= 0; i--)
            {
                actions[i]?.ResetToInitialState();
            }
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string targetName = "NULL";
            if (target != null)
                targetName = target.name;
            
            string actionNames = actions == null
                ? string.Empty
                : String.Join(", ", actions.Where(action => action != null).Select(action => action.DisplayName));
            return $"{index}. {targetName}: {actionNames.Truncate(45)}";
        }

        public bool TryGetActionAtIndex<T>(int index, out T result) where T: DOTweenActionBase
        {
            if (actions == null || index < 0 || index > actions.Length - 1)
            {
                result = null;
                return false;
            }

            result = actions[index] as T;
            return result != null;
        }
    }
}
#endif