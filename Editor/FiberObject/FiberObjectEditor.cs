using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace FiberFramework.Editor
{
    [CustomEditor(typeof(FiberObject))]
    public class FiberObjectEditor : UnityEditor.Editor
    {
        private       (List<Type> types, string[] names) _controllers;
        private       FiberObject                        _fiberObject;
        private       int                                _currentIndex;
        private       FiberObjectEditorStyleSheets       _styleSheets;
        private       Rect                               _contentRect = Rect.zero;
        private       BlockData                          _controllerData;
        private       BlockData                          _modelData;
        private       BlockData                          _viewData;
        private const string                             _configurationsFieldName = "_configurations";
        private const string                             _controllerFieldName     = "_controller";
        private const string                             _modelFieldName          = "_model";
        private const string                             _viewFieldName           = "_view";
        private       FiberObjectEditorStyleSheets       getStyleSheets => _styleSheets ??= new FiberObjectEditorStyleSheets();
        private       float                              _fullWidth     => _inspectorContainer.contentContainer.worldBound.width;
        private       ScrollView                         _inspectorContainer;
        private       EditorWindow                       _inspector;
       

        private void OnEnable()
        {
            _fiberObject  = target as FiberObject;
            _controllers  = FiberEditorTools.GetControllerList();
            _currentIndex = _fiberObject!.HasController ? _controllers.types.IndexOf(_fiberObject.GetControllerType) : 0;
            FillData();
            var windowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            _inspector = EditorWindow.GetWindow(windowType);
        }


        private void TryGetRoot()
        {
            try
            {
                _inspectorContainer = (ScrollView)_inspector.rootVisualElement.ElementAt(1).ElementAt(1);
            }
            catch (Exception e)
            {
                // ignored
            }
        }


        public override void OnInspectorGUI()
        {
            if (_inspectorContainer != null)
            {
                _contentRect.width = _fullWidth;

                EditorGUILayout.BeginVertical(new GUIStyle { fixedHeight = _contentRect.height - 12 });

                GUILayout.BeginArea(_contentRect);
                DrawContent();
                GUILayout.EndArea();

                EditorGUILayout.Space(0);
                EditorGUILayout.EndVertical();
            }
            else
            {
                TryGetRoot();
            }
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
            if (GUILayout.Button(_controllers.names[_currentIndex], getStyleSheets.controllerSelector, GUILayout.Width(_fullWidth)))
            {
                PopupWindow.Show(new Rect(0, 0, 0, 40), new FiberObjectEditorListMenu(_fullWidth, _controllers, OnSelectController, getStyleSheets));
            }

            if (_controllerData.information != default)
            {
                EditorGUILayout.LabelField(_controllerData.information, getStyleSheets.descriptionTextBox);
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

            FillData();

            EditorUtility.SetDirty(target);
            GC.Collect();
        }


        private void DrawConfigurations()
        {
            if (!_fiberObject.HasController) return;
            DrawPropertyFields(_controllerData);
        }


        private void DrawModel()
        {
            if (!_fiberObject.HasModel) return;
            DrawPropertyFields(_modelData);
        }


        private void DrawView()
        {
            if (!_fiberObject.HasView) return;
            DrawPropertyFields(_viewData);
        }


        private void DrawInfoBlock(BlockData data)
        {
            EditorGUILayout.BeginHorizontal(getStyleSheets.infoBlock);

            var width = (_fullWidth / 2f) - 13f;

            EditorGUILayout.LabelField($"<color=grey>{data.targetObjectToEdit.GetType().Name}</color>", getStyleSheets.infoBlockLeftLine, GUILayout.Width(width));
            EditorGUILayout.LabelField($"<color=#89D679>{data.handlers}</color>", getStyleSheets.infoBlockRightLine, GUILayout.Width(width));

            EditorGUILayout.EndHorizontal();
        }


        private void DrawPropertyFields(BlockData data)
        {
            var property = serializedObject.FindProperty(data.targetField);

            DrawPropertyBlockHeader(data);
            DrawInfoBlock(data);

            EditorGUILayout.BeginVertical(getStyleSheets.fieldsContainerFields);

            EditorStyles.foldoutHeader.StartEdit(x =>
            {
                x.padding.left = 15;
                x.fontStyle    = FontStyle.Normal;
            });

            EditorStyles.foldout.StartEdit(x => { x.padding = new RectOffset(15, 0, 0, 0); });

            foreach (var field in data.fields)
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


        private void DrawPropertyBlockHeader(BlockData data)
        {
            EditorGUILayout.BeginHorizontal(getStyleSheets.fieldsContainerHeader);

            EditorGUILayout.LabelField(data.displayName, getStyleSheets.fieldsContainerTitle);

            var rect = EditorGUILayout.BeginHorizontal(GUILayout.Width(20));

            rect.x -= 30;

            if (!Application.isPlaying && GUILayout.Button(new GUIContent(getStyleSheets.icon_menu), getStyleSheets.contextMenuButton))
            {
                PopupWindow.Show(rect, new FiberObjectEditorContextMenu(data.targetObjectToCopy, data.targetObjectToReset, data.targetObjectToEdit, _fiberObject));
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }


        private void FillData()
        {
            _controllerData = FillControllerData();
            _modelData      = FillModelData();
            _viewData       = FillViewData();
        }


        private BlockData FillModelData()
        {
            var type         = _fiberObject.GetModelType;
            var targetObject = FiberEditorTools.GetFieldValue(_fiberObject, _modelFieldName);

            var data = new BlockData
            {
                targetField         = _modelFieldName,
                targetObject        = targetObject,
                targetType          = type,
                targetObjectToEdit  = targetObject,
                targetObjectToCopy  = targetObject,
                targetObjectToReset = targetObject,
                handlers            = BuildHandlersString(type),
                displayName         = "Model",
                fields              = FiberEditorTools.GetFields(type)
            };

            return data;
        }


        private BlockData FillViewData()
        {
            var type         = _fiberObject.GetViewType;
            var targetObject = FiberEditorTools.GetFieldValue(_fiberObject, _viewFieldName);

            var data = new BlockData
            {
                targetField         = _viewFieldName,
                targetObject        = targetObject,
                targetType          = type,
                targetObjectToEdit  = targetObject,
                targetObjectToCopy  = targetObject,
                targetObjectToReset = targetObject,
                handlers            = BuildHandlersString(type),
                displayName         = "View",
                fields              = FiberEditorTools.GetFields(type)
            };

            return data;
        }


        private BlockData FillControllerData()
        {
            var type         = _fiberObject.GetControllerType;
            var fieldsSource = typeof(FiberControllerConfigurations);
            var config       = FiberEditorTools.GetFieldValue(_fiberObject, _configurationsFieldName);
            var controller   = FiberEditorTools.GetFieldValue(_fiberObject, _controllerFieldName);

            var data = new BlockData
            {
                targetField         = _configurationsFieldName,
                targetObject        = config,
                targetType          = fieldsSource,
                targetObjectToEdit  = controller,
                targetObjectToCopy  = controller,
                targetObjectToReset = config,
                handlers            = BuildHandlersString(type),
                displayName         = "Controller",
                fields              = FiberEditorTools.GetFields(fieldsSource)
            };

            FiberEditorTools.TryGetDescription(type, out data.information);

            return data;
        }


        private string BuildHandlersString(Type type)
        {
            var handlersLine = "";
            var handlers     = FiberEditorTools.GetHandlers(type);

            if (handlers.Length == 0)
            {
                return "No Handlers";
            }

            foreach (var handler in handlers)
            {
                var targetName = handler.Name;

                targetName = targetName.Replace('I', default);
                targetName = targetName.Replace("Handler", default);

                handlersLine += targetName;

                if (handler != handlers.Last())
                {
                    handlersLine += ", ";
                }
            }

            return handlersLine;
        }


        private class BlockData
        {
            public string                 targetField;
            public object                 targetObject;
            public object                 targetObjectToEdit;
            public object                 targetObjectToCopy;
            public object                 targetObjectToReset;
            public Type                   targetType;
            public string                 information;
            public string                 handlers;
            public string                 displayName;
            public IEnumerable<FieldInfo> fields;
        }
    }
}