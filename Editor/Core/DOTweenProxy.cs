#if DOTWEEN_ENABLED
using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    internal static class DOTweenProxy
    {
        private static FieldInfo sequencedObjects;
        private static FieldInfo sequencedPosition;
        private static FieldInfo sequencedEndPosition;

        [InitializeOnLoadMethod]
        private static void Setup()
        {
            try
            {
                sequencedObjects = typeof(Sequence)
                    .GetField("_sequencedObjs", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                sequencedPosition = typeof(ABSSequentiable)
                    .GetField("sequencedPosition",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                sequencedEndPosition = typeof(ABSSequentiable)
                    .GetField("sequencedEndPosition",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static (float start, float end)[] GetTimings(Sequence sequence, AnimationStepBase[] steps)
        {
            try
            {
                if (sequence == null || steps == null || sequencedObjects == null || sequencedPosition == null || sequencedEndPosition == null)
                    return null;

                var objs = GetSequencedObjects(sequence);
                if (objs == null)
                    return null;

                var duration = sequence.Duration();
                if (duration <= 0f || float.IsInfinity(duration))
                    return null;

                var stepObjects = new List<ABSSequentiable>(objs.Count);
                foreach (var obj in objs)
                {
                    if (obj is Tween)
                        stepObjects.Add(obj);
                }

                var timings = new (float start, float end)[steps.Length];
                int objectIndex = 0;
                for (int i = 0; i < steps.Length; i++)
                {
                    var step = steps[i];
                    if (step == null || step.IsMuted || !step.AddsToSequence)
                    {
                        timings[i] = (-1f, -1f);
                        continue;
                    }

                    if (objectIndex >= stepObjects.Count)
                        return null;

                    var obj = stepObjects[objectIndex++];
                    var start = GetSequencedStartPosition(obj) + step.Delay;
                    var end = GetSequencedEndPosition(obj);

                    timings[i] = (start / duration, end / duration);
                }

                return objectIndex == stepObjects.Count ? timings : null;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        private static List<ABSSequentiable> GetSequencedObjects(Sequence sequence)
        {
            return sequencedObjects?.GetValue(sequence) as List<ABSSequentiable>;
        }

        private static float GetSequencedStartPosition(ABSSequentiable sequenced)
        {
            return (float) sequencedPosition.GetValue(sequenced);
        }

        private static float GetSequencedEndPosition(ABSSequentiable sequenced)
        {
            return (float) sequencedEndPosition.GetValue(sequenced);
        }
    }
}
#endif
