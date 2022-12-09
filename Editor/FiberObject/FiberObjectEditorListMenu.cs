using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FiberFramework.Editor
{
    public class FiberObjectEditorListMenu : PopupWindowContent
    {
        private readonly Action<int>                        _onSelect;
        private readonly float                              _width;
        private readonly (List<Type> types, string[] names) _elements;
        private readonly FiberObjectEditorStyleSheets       _style;


        public FiberObjectEditorListMenu(float width, (List<Type> types, string[] names) elements, Action<int> onSelect, FiberObjectEditorStyleSheets style)
        {
            _width    = width;
            _elements = elements;
            _onSelect = onSelect;
            _style    = style;
        }


        public override Vector2 GetWindowSize()
        {
            return new Vector2(_width, _elements.types.Count * (_style.controllerSelectionButton.fixedHeight + 1));
        }


        public override void OnGUI(Rect rect)
        {
            for (var i = 0; i < _elements.names.Length; i++)
            {
                var targetType = _elements.types[i];
                var targetName = _elements.names[i];

                EditorGUILayout.BeginVertical(_style.controllerSelectionElement);

                if (GUILayout.Button("", _style.controllerSelectionButton))
                {
                    _onSelect?.Invoke(i);
                    editorWindow.Close();
                }

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(targetName, _style.controllerSelectionNameLabel);

                if (i != 0 && FiberEditorTools.TryGetDescription(targetType, out var description))
                {
                    GUILayout.Label(description, _style.controllerSelectionDescriptionLabel);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(1);
            }

            editorWindow.Repaint();
        }
    }
}