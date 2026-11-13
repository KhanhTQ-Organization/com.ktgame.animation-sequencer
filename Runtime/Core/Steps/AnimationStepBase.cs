#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class AnimationStepBase
    {
        [SerializeField]
        private float delay;
        public float Delay => delay;

        [SerializeField]
        private FlowType flowType;
        public FlowType FlowType => flowType;

        [SerializeField]
        [Tooltip("Muted steps are skipped when the sequence is generated (useful to test without deleting the step).")]
        private bool muted;
        public bool IsMuted
        {
            get => muted;
            set => muted = value;
        }

        public virtual bool AddsToSequence => true;

        public virtual float GetEstimatedLength()
        {
            return Mathf.Max(0f, delay);
        }

        public abstract string DisplayName { get; }
        
        public abstract void AddTweenToSequence(Sequence animationSequence);

        public abstract void ResetToInitialState();

        public virtual string GetDisplayNameForEditor(int index)
        {
            return $"{index}. {this}";
        }

        public bool IsSkippingToEnd { get; set; }
    }
}
#endif