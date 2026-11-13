using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Compilation;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [InitializeOnLoad]
    public static class AnimationSequencerSetupHelper
    {
        private const string SCRIPTING_DEFINE_SYMBOL = "DOTWEEN_ENABLED";
        private const string DOTWEEN_ASSEMBLY_NAME = "DOTween.Modules";

        private static readonly NamedBuildTarget[] Targets =
        {
            NamedBuildTarget.Standalone,
            NamedBuildTarget.Android,
            NamedBuildTarget.iOS,
            NamedBuildTarget.WebGL,
        };

        static AnimationSequencerSetupHelper()
        {
            Assembly[] availableAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies);

            bool foundDOTween = false;
            for (int i = availableAssemblies.Length - 1; i >= 0; i--)
            {
                if (availableAssemblies[i].name.IndexOf(DOTWEEN_ASSEMBLY_NAME, StringComparison.Ordinal) > -1)
                {
                    foundDOTween = true;
                    break;
                }
            }

            if (foundDOTween)
            {
                foreach (NamedBuildTarget target in GetTargets())
                    AddScriptingDefineSymbol(target);
            }
            else
            {
                foreach (NamedBuildTarget target in GetTargets())
                    RemoveScriptingDefineSymbol(target);

                Debug.LogWarning("No DOTween found, animation sequencer will be disabled until DOTween setup is complete and asmdef files are created");
            }
        }

        private static IEnumerable<NamedBuildTarget> GetTargets()
        {
            var result = new List<NamedBuildTarget>(Targets);

            try
            {
                NamedBuildTarget selected = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                if (!result.Contains(selected))
                    result.Add(selected);
            }
            catch (Exception)
            {
            }

            return result;
        }

        private static List<string> GetSymbols(NamedBuildTarget target)
        {
            string symbols = PlayerSettings.GetScriptingDefineSymbols(target);
            var list = new List<string>();
            foreach (string symbol in symbols.Split(';'))
            {
                string trimmed = symbol.Trim();
                if (trimmed.Length > 0)
                    list.Add(trimmed);
            }

            return list;
        }

        private static void AddScriptingDefineSymbol(NamedBuildTarget target)
        {
            List<string> symbols = GetSymbols(target);
            if (symbols.Contains(SCRIPTING_DEFINE_SYMBOL))
                return;

            symbols.Add(SCRIPTING_DEFINE_SYMBOL);
            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", symbols));

            Debug.Log($"Adding {SCRIPTING_DEFINE_SYMBOL} for {target.TargetName}");
        }

        private static void RemoveScriptingDefineSymbol(NamedBuildTarget target)
        {
            List<string> symbols = GetSymbols(target);
            if (!symbols.Remove(SCRIPTING_DEFINE_SYMBOL))
                return;

            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", symbols));
        }
    }
}
