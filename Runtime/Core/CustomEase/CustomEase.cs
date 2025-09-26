using System;
using DG.Tweening;
using com.ktgame.animation_sequencer;
using DG.Tweening.Core.Easing;
using JetBrains.Annotations;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
    [Serializable]
    public partial class CustomEase : IEquatable<CustomEase>
    {
        [SerializeField]
        private Ease _ease;
        public Ease Ease => _ease;
        [SerializeField]
        private AnimationCurve _curve;

        private EaseFunction _easeFunction;


        public bool UseCustomCurve => _ease == Ease.INTERNAL_Custom;

        public CustomEase(AnimationCurve curve)
        {
            this._curve = curve;
            _ease = Ease.INTERNAL_Custom;
            _easeFunction = new EaseCurve(curve).Evaluate;
        }

        public CustomEase(Ease ease)
        {
            this._ease = ease;
            _easeFunction = null;
            _curve = null;
        }

        public CustomEase()
        {
            _ease = Ease.InOutCirc;
        }
        
        public float Lerp(float from, float to, float fraction)
        {
            return Mathf.Lerp(from, to, Evaluate(fraction));
        }

        public float LerpUnclamped(float from, float to, float fraction)
        {
            return Mathf.LerpUnclamped(from, to, Evaluate(fraction));
        }

        [Pure]
        public float Evaluate(float time, float duration = 1f,
            float overshootOrAmplitude = 1.70158f)
        {
            if (UseCustomCurve)
            {
                if (_easeFunction == null)
                    _easeFunction = new EaseCurve(_curve).Evaluate;

                return EaseManager.Evaluate(Ease.INTERNAL_Custom, _easeFunction, time, duration,
                    overshootOrAmplitude, DOTween.defaultEasePeriod);
            }
            else
            {
                return EaseManager.Evaluate(_ease, null, time, duration,
                    overshootOrAmplitude, DOTween.defaultEasePeriod);
            }
        }

        public void ApplyTo(TweenParams tweenParams)
        {
            if (UseCustomCurve)
            {
                tweenParams.SetEase(_curve);
            }
            else
            {
                tweenParams.SetEase(_ease);
            }
        }

        public void ApplyTo<T>(T tween) where T : Tween
        {
            if (UseCustomCurve)
            {
                tween.SetEase(_curve);
            }
            else
            {
                tween.SetEase(_ease);
            }
        }

        public bool Equals(CustomEase other)
        {
            return _ease == other._ease && (_ease != Ease.INTERNAL_Custom || Equals(_curve, other._curve));
        }

        public override bool Equals(object obj)
        {
            return obj is CustomEase other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)_ease * 397) ^ ((_ease == Ease.INTERNAL_Custom && _curve != null) ? _curve.GetHashCode() : 0);
            }
        }
    }
}

namespace DG.Tweening
{
    public static partial class CustomEaseExtensions
    {
        /// <summary>Sets the _ease of the tween using a custom _ease function.
        /// <para>If applied to Sequences eases the whole sequence animation</para></summary>
        public static TweenParams SetEase(this TweenParams tweenParams, CustomEase customEase)
        {
            customEase.ApplyTo(tweenParams);
            return tweenParams;
        }

        /// <summary>Sets the _ease of the tween.
        /// <para>If applied to Sequences eases the whole sequence animation</para></summary>
        public static T SetEase<T>(this T t, CustomEase customEase) where T : Tween
        {
            customEase.ApplyTo(t);
            return t;
        }
    }
}