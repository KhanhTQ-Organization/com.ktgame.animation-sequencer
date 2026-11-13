#if DOTWEEN_ENABLED
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BrunoMikoski.AnimationSequencer
{
    public static class AnimationStepValidator
    {
        private const BindingFlags FieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static List<string> GetIssues(AnimationStepBase step, AnimationSequencerController owner)
        {
            var issues = new List<string>();

            if (step == null)
            {
                issues.Add("Missing step type (the class was renamed or removed).");
                return issues;
            }

            AddUnassignedReferences(step, string.Empty, issues);

            switch (step)
            {
                case DOTweenAnimationStep tweenStep:
                    ValidateActions(tweenStep, issues);
                    break;

                case PlaySequenceAnimationStep playSequenceStep:
                    if (owner != null && playSequenceStep.Sequencer == owner)
                        issues.Add("Plays its own controller: skipped at runtime.");
                    break;
            }

            return issues;
        }

        public static int CountIssues(AnimationSequencerController controller)
        {
            if (controller == null || controller.AnimationSteps == null)
                return 0;

            int count = 0;
            foreach (AnimationStepBase step in controller.AnimationSteps)
            {
                if (step != null && step.IsMuted)
                    continue;

                count += GetIssues(step, controller).Count;
            }

            return count;
        }

        private static void ValidateActions(DOTweenAnimationStep tweenStep, List<string> issues)
        {
            DOTweenActionBase[] actions = tweenStep.Actions;
            if (actions == null || actions.Length == 0)
            {
                issues.Add("No actions.");
                return;
            }

            for (int i = 0; i < actions.Length; i++)
            {
                DOTweenActionBase action = actions[i];
                if (action == null)
                {
                    issues.Add($"Action {i + 1}: missing type (the class was renamed or removed).");
                    continue;
                }

                AddUnassignedReferences(action, action.DisplayName + ": ", issues);

                GameObject target = tweenStep.Target;
                Type requiredType = action.TargetComponentType;
                if (target != null && requiredType != null && !HasComponent(target, requiredType))
                    issues.Add($"{action.DisplayName}: target has no {requiredType.Name}.");
            }
        }

        private static bool HasComponent(GameObject target, Type componentType)
        {
            if (componentType == typeof(Transform) || componentType == typeof(GameObject))
                return true;

            if (componentType == typeof(RectTransform))
                return target.transform is RectTransform;

            return typeof(Component).IsAssignableFrom(componentType) && target.GetComponent(componentType) != null;
        }

        private static void AddUnassignedReferences(object instance, string prefix, List<string> issues)
        {
            Type type = instance.GetType();
            while (type != null && type != typeof(object))
            {
                foreach (FieldInfo field in type.GetFields(FieldFlags))
                {
                    if (!typeof(Object).IsAssignableFrom(field.FieldType))
                        continue;

                    bool serialized = field.IsPublic || field.GetCustomAttribute<SerializeField>() != null;
                    if (!serialized || field.IsNotSerialized)
                        continue;

                    var value = field.GetValue(instance) as Object;
                    if (value == null)
                        issues.Add($"{prefix}{ObjectNames.NicifyVariableName(field.Name)} is not assigned.");
                }

                type = type.BaseType;
            }
        }
    }
}
#endif
