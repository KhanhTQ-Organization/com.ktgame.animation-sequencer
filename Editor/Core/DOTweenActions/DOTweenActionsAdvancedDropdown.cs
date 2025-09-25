using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace com.ktgame.animation_sequencer.editor
{
    public sealed class DOTweenActionAdvancedDropdownItem : AdvancedDropdownItem
    {
        private Type _baseDOTweenActionType;
        public Type BaseDOTweenActionType => _baseDOTweenActionType;

        public DOTweenActionAdvancedDropdownItem(Type baseDOTweenActionType, string displayName) : base(displayName)
        {
            this._baseDOTweenActionType = baseDOTweenActionType;
        }
    }
    
    public sealed class DOTweenActionsAdvancedDropdown : AdvancedDropdown
    {
        private Action<DOTweenActionAdvancedDropdownItem> _callback;
        private SerializedProperty _actionsList;
        private GameObject _targetGameObject;

        public DOTweenActionsAdvancedDropdown(AdvancedDropdownState state) : base(state)
        {
            this.minimumSize = new Vector2(200, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("DOTween Actions");

            foreach (var typeToDisplayGUI in AnimationSequenceEditorGUIUtility.TypeToDisplayName)
            {
                Type baseDOTweenActionType = typeToDisplayGUI.Key;

                AdvancedDropdownItem targetFolder = root;

                if (AnimationSequenceEditorGUIUtility.TypeToParentDisplay.TryGetValue(baseDOTweenActionType, out GUIContent parent))
                {
                    AdvancedDropdownItem item = targetFolder.children.FirstOrDefault(dropdownItem =>
                        dropdownItem.name.Equals(parent.text, StringComparison.Ordinal));
                    
                    if (item == null)
                    {
                        item = new AdvancedDropdownItem(parent.text)
                        {
                            icon = (Texture2D) parent.image
                        };
                        targetFolder.AddChild(item);
                    }
                    
                    targetFolder = item;
                }

                DOTweenActionAdvancedDropdownItem doTweenActionAdvancedDropdownItem = 
                    new DOTweenActionAdvancedDropdownItem(baseDOTweenActionType, typeToDisplayGUI.Value.text)
                {
                    enabled = !IsTypeAlreadyInUse(_actionsList, baseDOTweenActionType) && AnimationSequenceEditorGUIUtility.CanActionBeAppliedToTarget(baseDOTweenActionType, _targetGameObject)
                };
                
                if (typeToDisplayGUI.Value.image != null)
                {
                    doTweenActionAdvancedDropdownItem.icon = (Texture2D) typeToDisplayGUI.Value.image;
                }
                
                targetFolder.AddChild(doTweenActionAdvancedDropdownItem);
            }
            
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            _callback?.Invoke(item as DOTweenActionAdvancedDropdownItem);
        }

        public void Show(Rect rect, SerializedProperty actionsListSerializedProperty, Object targetGameObject, Action<DOTweenActionAdvancedDropdownItem> 
        onActionSelectedCallback)
        {
            _callback = onActionSelectedCallback;
            this._actionsList = actionsListSerializedProperty;
            if (targetGameObject is GameObject target)
            {
                this._targetGameObject = target;
            }

            base.Show(rect);
        }

        private bool IsTypeAlreadyInUse(SerializedProperty actionsSerializedProperty, Type targetType)
        {
            if (string.IsNullOrEmpty(targetType.FullName))
            {
                return false;
            }

            for (var i = 0; i < actionsSerializedProperty.arraySize; i++)
            {
                SerializedProperty actionElement = actionsSerializedProperty.GetArrayElementAtIndex(i);
                if (actionElement.managedReferenceFullTypename.IndexOf(targetType.FullName, StringComparison.Ordinal) > -1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}