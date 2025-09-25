using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace com.ktgame.animation_sequencer.editor
{
    public static class AnimationSequenceEditorGUIUtility
    {
        private static Dictionary<Type, GUIContent> _cachedTypeToDisplayName;
        public static Dictionary<Type, GUIContent> TypeToDisplayName
        {
            get
            {
                CacheDisplayTypes();
                return _cachedTypeToDisplayName;
            }
        }
        
        private static Dictionary<Type, GUIContent> _cachedTypeToInstance;
        public static Dictionary<Type, GUIContent> TypeToParentDisplay
        {
            get
            {
                CacheDisplayTypes();
                return _cachedTypeToInstance;
            }
        }

        
        private static Dictionary<Type, DOTweenActionBase> _typeToInstanceCache;
        public static Dictionary<Type, DOTweenActionBase> TypeToInstanceCache
        {
            get
            {
                CacheDisplayTypes();
                return _typeToInstanceCache;
            }
        }
        
        private static DOTweenActionsAdvancedDropdown _cachedDOTweenActionsDropdown;
        public static DOTweenActionsAdvancedDropdown DOTweenActionsDropdown
        {
            get
            {
                if (_cachedDOTweenActionsDropdown == null)
                    _cachedDOTweenActionsDropdown = new DOTweenActionsAdvancedDropdown(new AdvancedDropdownState());
                return _cachedDOTweenActionsDropdown;
            }
        }
        

        public static GUIContent GetTypeDisplayName(Type targetBaseDOTweenType)
        {
            if (TypeToDisplayName.TryGetValue(targetBaseDOTweenType, out GUIContent result))
                return result;

            return new GUIContent(targetBaseDOTweenType.Name);
        }

        private static void CacheDisplayTypes()
        {
            if (_cachedTypeToDisplayName != null)
                return;

            _cachedTypeToDisplayName = new Dictionary<Type, GUIContent>();
            _cachedTypeToInstance = new Dictionary<Type, GUIContent>();
            _typeToInstanceCache = new Dictionary<Type, DOTweenActionBase>();
            
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(typeof(DOTweenActionBase));
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];
                if (type.IsAbstract)
                    continue;
                
                DOTweenActionBase doTweenActionBaseInstance = Activator.CreateInstance(type) as DOTweenActionBase;
                if (doTweenActionBaseInstance == null)
                    continue;
                GUIContent guiContent = new GUIContent(doTweenActionBaseInstance.DisplayName);
                if (doTweenActionBaseInstance.TargetComponentType != null)
                {
                    GUIContent targetComponentGUIContent = EditorGUIUtility.ObjectContent(null, doTweenActionBaseInstance.TargetComponentType);
                    guiContent.image = targetComponentGUIContent.image;
                    GUIContent parentGUIContent = new GUIContent(doTweenActionBaseInstance.TargetComponentType.Name)
                    {
                        image = targetComponentGUIContent.image
                    };
                    _cachedTypeToInstance.Add(type, parentGUIContent);
                }
                
                _cachedTypeToDisplayName.Add(type, guiContent);
                _typeToInstanceCache.Add(type, doTweenActionBaseInstance);
            }
        }
        
        public static bool CanActionBeAppliedToTarget(Type targetActionType, GameObject targetGameObject)
        {
            if (targetGameObject == null)
            {
                return false;
            }

            if (TypeToInstanceCache.TryGetValue(targetActionType, out DOTweenActionBase actionBaseInstance))
            {
                Type requiredComponent = actionBaseInstance.TargetComponentType;
                
                if (requiredComponent == typeof(Transform))
                {
                    return true;
                }

                if (requiredComponent == typeof(RectTransform))
                {
                    return targetGameObject.transform is RectTransform;
                }

                return targetGameObject.GetComponent(requiredComponent) != null;
            }
            return false;
        }

        private static GUIContent cachedBackButtonGUIContent;
        internal static GUIContent BackButtonGUIContent
        {
            get
            {
                if (cachedBackButtonGUIContent == null)
                {
                    cachedBackButtonGUIContent = EditorGUIUtility.IconContent("d_beginButton");
                    cachedBackButtonGUIContent.tooltip = "Rewind";
                }

                return cachedBackButtonGUIContent;
            }
        }
        
        private static GUIContent cachedStepBackGUIContent;
        internal static GUIContent StepBackGUIContent
        {
            get
            {
                if (cachedStepBackGUIContent == null)
                {
                    cachedStepBackGUIContent = EditorGUIUtility.IconContent("Animation.PrevKey");
                    cachedStepBackGUIContent.tooltip = "Step Back";
                }

                return cachedStepBackGUIContent;
            }
        }
        
        private static GUIContent cachedStepNextGUIContent;
        internal static GUIContent StepNextGUIContent
        {
            get
            {
                if (cachedStepNextGUIContent == null)
                {
                    cachedStepNextGUIContent = EditorGUIUtility.IconContent("Animation.NextKey");
                    cachedStepNextGUIContent.tooltip = "Step Next";
                }

                return cachedStepNextGUIContent;
            }
        }
        
        private static GUIContent _cachedStopButtonGUIContent;
        internal static GUIContent StopButtonGUIContent
        {
            get
            {
                if (_cachedStopButtonGUIContent == null)
                {
                    _cachedStopButtonGUIContent = EditorGUIUtility.IconContent("animationdopesheetkeyframe");
                    _cachedStopButtonGUIContent.tooltip = "Stop";
                }
                return _cachedStopButtonGUIContent;
            }
        }
        
        private static GUIContent _cachedForwardButtonGUIContent;
        internal static GUIContent ForwardButtonGUIContent
        {
            get
            {
                if (_cachedForwardButtonGUIContent == null)
                {
                    _cachedForwardButtonGUIContent = EditorGUIUtility.IconContent("d_endButton");
                    _cachedForwardButtonGUIContent.tooltip = "Fast Forward";
                }
                return _cachedForwardButtonGUIContent;
            }
        }
        
        private static GUIContent _cachedPauseButtonGUIContent;
        internal static GUIContent PauseButtonGUIContent
        {
            get
            {
                if (_cachedPauseButtonGUIContent == null)
                {
                    _cachedPauseButtonGUIContent = EditorGUIUtility.IconContent("d_PauseButton@2x");
                    _cachedPauseButtonGUIContent.tooltip = "Pause";
                }
                return _cachedPauseButtonGUIContent;
            }
        }
        
        private static GUIContent _cachedPlayButtonGUIContent;
        internal static GUIContent PlayButtonGUIContent
        {
            get
            {
                if (_cachedPlayButtonGUIContent == null)
                {
                    _cachedPlayButtonGUIContent = EditorGUIUtility.IconContent("d_PlayButton@2x");
                    _cachedPlayButtonGUIContent.tooltip = "Play";
                }
                return _cachedPlayButtonGUIContent;
            }
        }
        
        private static GUIContent _cachedSaveAsDefaultGUIContent;
        internal static GUIContent SaveAsDefaultButtonGUIContent
        {
            get
            {
                if (_cachedSaveAsDefaultGUIContent == null)
                {
                    _cachedSaveAsDefaultGUIContent = EditorGUIUtility.IconContent("d_SaveAs");
                    _cachedSaveAsDefaultGUIContent.tooltip = "Save as Default";
                }
                return _cachedSaveAsDefaultGUIContent;
            }
        }
    }
}