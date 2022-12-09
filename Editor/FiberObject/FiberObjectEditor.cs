using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FiberFramework.Editor
{
    [CustomEditor(typeof(FiberObject))]
    public class FiberObjectEditor : UnityEditor.Editor
    {
        private       (List<Type> types, string[] names) _controllers;
        private       FiberObject                        _fiberObject;
        private       int                                _currentIndex;
        private       FiberObjectEditorStyleSheets       _styleSheets;
        private       Rect                               _contentRect;
        private       DrawFieldsOptions                  _nextDrawFieldsOptions;
        private const string                             _configurationsFieldName = "_configurations";
        private const string                             _controllerFieldName     = "_controller";
        private const string                             _modelFieldName          = "_model";
        private const string                             _viewFieldName           = "_view";
        private       FiberObjectEditorStyleSheets       getStyleSheets => _styleSheets ??= new FiberObjectEditorStyleSheets();


        private void OnEnable()
        {
            _fiberObject           = target as FiberObject;
            _nextDrawFieldsOptions = new DrawFieldsOptions();
            _controllers           = FiberEditorTools.GetControllerList();
        }


        public override void OnInspectorGUI()
        {
            _contentRect.width = Screen.width;

            EditorGUILayout.BeginVertical(new GUIStyle { fixedHeight = _contentRect.height - 12 });

            GUILayout.BeginArea(_contentRect);
            DrawContent();
            GUILayout.EndArea();

            EditorGUILayout.Space(-1);
            EditorGUILayout.EndVertical();

            Repaint();
        }


        private void DrawContent()
        {
            serializedObject.Update();

            var targetRect = EditorGUILayout.BeginVertical(getStyleSheets.mainContainer);

            DrawController();
            DrawConfigurations();
            DrawModel();
            DrawView();


            EditorGUILayout.EndVertical();

            if (targetRect != default)
            {
                _contentRect = targetRect;
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawController()
        {
            _currentIndex = _fiberObject.HasController ? _controllers.types.IndexOf(_fiberObject.GetControllerType) : 0;

            if (GUILayout.Button(_controllers.names[_currentIndex], getStyleSheets.controllerSelector, GUILayout.Width(Screen.width)))
            {
                PopupWindow.Show(new Rect(0, 0, 0, 40), new FiberObjectEditorListMenu(Screen.width, _controllers, OnSelectController, getStyleSheets));
            }

            DrawDescription(_controllers.types[_currentIndex]);
        }


        private void DrawDescription(Type targetType)
        {
            if (targetType != null && FiberEditorTools.TryGetDescription(targetType, out var description))
            {
                EditorGUILayout.LabelField(description, getStyleSheets.descriptionTextBox);
            }
        }

        private void OnSelectController(int controllerID)
        {
            _currentIndex = controllerID;

            if (controllerID == 0)
            {
                _fiberObject.Reset();

                EditorUtility.SetDirty(target);
                return;
            }

            var targetType = _controllers.types[controllerID];
            _fiberObject.Construct(targetType);

            EditorUtility.SetDirty(target);
            GC.Collect();
        }


        private void DrawConfigurations()
        {
            if (!_fiberObject.HasController) return;

            ModifyDrawFieldOptions(typeof(FiberControllerConfigurations), _configurationsFieldName, "Controller");
            var controller = FiberEditorTools.GetFieldValue(_fiberObject, _controllerFieldName);

            _nextDrawFieldsOptions.contextMenuEditTarget = controller;
            _nextDrawFieldsOptions.infoSource            = _fiberObject.GetControllerType;

            DrawPropertyFields(_nextDrawFieldsOptions);
        }


        private void DrawModel()
        {
            if (!_fiberObject.HasModel) return;

            ModifyDrawFieldOptions(_fiberObject.GetModelType, _modelFieldName, "Model");

            DrawPropertyFields(_nextDrawFieldsOptions);
        }


        private void DrawView()
        {
            if (!_fiberObject.HasView) return;

            ModifyDrawFieldOptions(_fiberObject.GetViewType, _viewFieldName, "View");

            DrawPropertyFields(_nextDrawFieldsOptions);
        }


        private void ModifyDrawFieldOptions(Type targetType, string fieldName, string title)
        {
            var fieldValue = FiberEditorTools.GetFieldValue(_fiberObject, _viewFieldName);

            _nextDrawFieldsOptions.rootPropertyType       = targetType;
            _nextDrawFieldsOptions.rootPropertyName       = fieldName;
            _nextDrawFieldsOptions.contextMenuResetTarget = fieldValue;
            _nextDrawFieldsOptions.contextMenuCopyTarget  = fieldValue;
            _nextDrawFieldsOptions.contextMenuEditTarget  = fieldValue;
            _nextDrawFieldsOptions.infoSource             = targetType;
            _nextDrawFieldsOptions.headerTitle            = title;
        }


        private void DrawInfoBlock(Type targetType)
        {
            EditorGUILayout.BeginHorizontal(getStyleSheets.infoBlock);

            var handlers       = FiberEditorTools.GetHandlers(targetType);
            var handlersString = handlers.Length > 0 ? "" : "No Handlers";

            foreach (var handler in handlers)
            {
                var targetName = handler.Name;

                targetName = targetName.Replace('I', default);
                targetName = targetName.Replace("Handler", default);

                handlersString += targetName;

                if (handler != handlers.Last())
                {
                    handlersString += ", ";
                }
            }

            var width = (_contentRect.width / 2) - 13;

            EditorGUILayout.LabelField($"<color=grey>{targetType.Name}</color>", getStyleSheets.infoBlockLeftLine, GUILayout.Width(width));
            EditorGUILayout.LabelField($"<color=#89D679>{handlersString}</color>", getStyleSheets.infoBlockRightLine, GUILayout.Width(width));

            EditorGUILayout.EndHorizontal();
        }


        private void DrawPropertyFields(DrawFieldsOptions options)
        {
            var property = serializedObject.FindProperty(options.rootPropertyName);
            var fields   = FiberEditorTools.GetFields(options.rootPropertyType);

            DrawPropertyBlockHeader(options.contextMenuCopyTarget, options.contextMenuResetTarget, options.contextMenuEditTarget, options.headerTitle);
            DrawInfoBlock(options.infoSource);

            EditorGUILayout.BeginVertical(getStyleSheets.fieldsContainerFields);

            EditorStyles.foldoutHeader.StartEdit();
            EditorStyles.foldoutHeader.padding.left = 15;
            EditorStyles.foldoutHeader.fontStyle    = FontStyle.Normal;

            EditorStyles.foldout.StartEdit();
            EditorStyles.foldout.padding = new RectOffset(15, 0, 0, 0);

            foreach (var field in fields)
            {
                var drawTarget = property.FindPropertyRelative(field.Name);

                if (drawTarget != null)
                {
                    EditorGUILayout.PropertyField(drawTarget, true);
                }
            }

            EditorStyles.foldout.StopEdit();
            EditorStyles.foldoutHeader.StopEdit();

            EditorGUILayout.EndVertical();
        }


        private void DrawPropertyBlockHeader(object targetForCopyPaste, object targetForReset, object targetForEdit, string title)
        {
            EditorGUILayout.BeginHorizontal(getStyleSheets.fieldsContainerHeader);

            EditorGUILayout.LabelField(title, getStyleSheets.fieldsContainerTitle);

            var rect = EditorGUILayout.BeginHorizontal(GUILayout.Width(20));

            rect.x -= 30;

            if (!Application.isPlaying && GUILayout.Button(new GUIContent(getStyleSheets.icon_menu), getStyleSheets.contextMenuButton))
            {
                PopupWindow.Show(rect, new FiberObjectEditorContextMenu(targetForCopyPaste, targetForReset, targetForEdit, _fiberObject));
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private class DrawFieldsOptions
        {
            public string rootPropertyName;
            public string headerTitle;
            public object contextMenuCopyTarget;
            public object contextMenuResetTarget;
            public object contextMenuEditTarget;
            public Type   infoSource;
            public Type   rootPropertyType;
        }
    }
}