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
        private const string                             _configurationsFieldName = "_configurations";
        private const string                             _modelFieldName          = "_model";
        private const string                             _viewFieldName           = "_view";

        private FiberObjectEditorStyleSheets getStyleSheets
        {
            get
            {
                _styleSheets ??= new FiberObjectEditorStyleSheets();
                return _styleSheets;
            }
        }

        private void OnEnable()
        {
            _controllers = GetControllerList();
            _fiberObject = target as FiberObject;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginVertical(getStyleSheets.mainContainer);

            DrawController();
            DrawConfigurations();
            DrawModel();
            DrawView();


            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawController()
        {
            _currentIndex = _fiberObject.HasController ? _controllers.types.IndexOf(_fiberObject.GetControllerType) : 0;
            var newIndex = EditorGUILayout.Popup("", _currentIndex, _controllers.names, getStyleSheets.controllerSelector,
                GUILayout.Height(40));

            if (newIndex != 0)
            {
                DrawDescription(_controllers.types[newIndex]);
            }
            
            if (_currentIndex == newIndex) return;

            if (newIndex == 0)
            {
                _fiberObject.Reset();

                EditorUtility.SetDirty(target);
                return;
            }

            var controllerType = _controllers.types[newIndex];
            _fiberObject.Construct(controllerType);

            EditorUtility.SetDirty(target);
            GC.Collect();
        }


        private void DrawConfigurations()
        {
            if (!_fiberObject.HasController) return;
            DrawFields(typeof(FiberControllerConfigurations), _configurationsFieldName, _fiberObject.RefreshConfigurations, false);
        }


        private void DrawModel()
        {
            if (!_fiberObject.HasModel) return;
            DrawFields(_fiberObject.GetModelType, _modelFieldName, _fiberObject.RefreshModel);
        }


        private void DrawView()
        {
            if (!_fiberObject.HasView) return;
            DrawFields(_fiberObject.GetViewType, _viewFieldName, _fiberObject.RefreshView);
        }


        private void DrawDescription(Type targetType)
        {
            if (Attribute.GetCustomAttribute(targetType, typeof(ControllerDescription)) is ControllerDescription descriptionAttribute)
            {
                EditorGUILayout.BeginVertical(getStyleSheets.fieldsContainer);

                EditorGUILayout.TextArea(descriptionAttribute.Description, getStyleSheets.descriptionTextBox);

                EditorGUILayout.EndVertical();
            }
        }


        private void DrawFields(Type targetType, string propertyName, Action onReset, bool showType = true)
        {
            var property = serializedObject.FindProperty(propertyName);
            var fields   = GetFields(targetType);

            EditorGUILayout.BeginVertical(getStyleSheets.fieldsContainer);

            EditorGUILayout.BeginHorizontal(getStyleSheets.fieldsContainerHeader);
            EditorGUILayout.LabelField($"{ObjectNames.NicifyVariableName(propertyName)}", getStyleSheets.fieldsContainerTitle,
                GUILayout.Width(0));

            var typeLabel = showType ? "[" + targetType.Name + "]" : "";
            EditorGUILayout.LabelField($"{typeLabel} ", getStyleSheets.fieldsContainerDetails);


            if (!Application.isPlaying && GUILayout.Button("Reset"))
            {
                onReset?.Invoke();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(getStyleSheets.fieldsContainerFields);

            foreach (var field in fields)
            {
                var drawTarget = property.FindPropertyRelative(field.Name);
                if (drawTarget == null) break;

                EditorGUILayout.PropertyField(drawTarget, true);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }


        private static (List<Type> types, string[] names) GetControllerList()
        {
            var type        = typeof(IFiberController);
            var assemblies  = AppDomain.CurrentDomain.GetAssemblies();
            var typesRAW    = new List<Type>();
            var resultTypes = new List<Type>();
            var resultNames = new List<string>();

            foreach (var x in assemblies)
            {
                typesRAW.AddRange(x.GetTypes());
            }

            resultTypes.AddRange(typesRAW.Where(t => type.IsAssignableFrom(t)                                          &&
                                                     t != type                                                         &&
                                                     !t.IsGenericType                                                  &&
                                                     Attribute.GetCustomAttribute(t, typeof(HiddenController)) == null &&
                                                     t                                                         != typeof(FiberController.Single)));

            resultTypes = resultTypes.Where(x => x != null).OrderBy(x => x.Name).ToList();

            resultNames.AddRange(resultTypes.Select(t =>
            {
                var nameAttribute = Attribute.GetCustomAttribute(t, typeof(ControllerName));

                return nameAttribute == null ? ObjectNames.NicifyVariableName(t.Name) : ((ControllerName)nameAttribute).CustomName;
            }));

            resultTypes.Insert(0, null);
            resultNames.Insert(0, "Select Controller");

            return (resultTypes, resultNames.ToArray());
        }


        private static IEnumerable<FieldInfo> GetFields(Type targetType)
        {
            var fields = targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            var result = fields.Where(x => (x.IsPublic                                                      ||
                                            (!x.IsPublic && Attribute.IsDefined(x, typeof(SerializeField))) ||
                                            (!x.IsPublic && Attribute.IsDefined(x, typeof(SerializeReference)))) &&
                                           !x.IsInitOnly                                                         &&
                                           !x.IsStatic);
            return result;
        }
    }


    public class FiberObjectEditorStyleSheets
    {
        public readonly GUIStyle mainContainer;
        public readonly GUIStyle controllerSelector;
        public readonly GUIStyle fieldsContainerHeader;
        public readonly GUIStyle fieldsContainerTitle;
        public readonly GUIStyle fieldsContainerDetails;
        public readonly GUIStyle fieldsContainer;
        public readonly GUIStyle fieldsContainerFields;
        public readonly GUIStyle descriptionTextBox;

        public FiberObjectEditorStyleSheets()
        {
            mainContainer = new GUIStyle(EditorStyles.helpBox);

            var gradient = new Gradient();

            var colorKey = new GradientColorKey[2];
            colorKey[0].color = new Color(0.1f, 0.1f, 0.1f);
            colorKey[0].time  = 0.0f;
            colorKey[1].color = new Color(0.1f, 0.1f, 0.1f);
            colorKey[1].time  = 1.0f;

            var alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 0.5f;
            alphaKey[0].time  = 0.0f;
            alphaKey[1].alpha = 0.0f;
            alphaKey[1].time  = 1.0f;

            gradient.SetKeys(colorKey, alphaKey);

            controllerSelector = new GUIStyle(EditorStyles.popup)
            {
                fixedHeight = 40,
                fontStyle   = FontStyle.Bold,
                fontSize    = 15,
                alignment   = TextAnchor.MiddleCenter,
                normal =
                {
                    background = GenerateColorGradient(10, 10, gradient),
                    textColor  = Color.white
                }
            };

            fieldsContainerHeader = new GUIStyle
            {
                fixedHeight = 23,
                normal =
                {
                    background = GenerateColorTexture(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.5f))
                }
            };

            fieldsContainerTitle = new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                padding   = new RectOffset(10, 0, 0, 0),
                normal =
                {
                    textColor = Color.white
                }
            };

            fieldsContainerDetails = new GUIStyle()
            {
                alignment = TextAnchor.MiddleRight,
                normal =
                {
                    textColor = Color.gray
                }
            };

            fieldsContainer = new GUIStyle
            {
                normal =
                {
                    background = GenerateColorTexture(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f))
                }
            };

            fieldsContainerFields = new GUIStyle
            {
                margin = new RectOffset(10, 0, 0, 0),
            };

            descriptionTextBox = new GUIStyle(EditorStyles.inspectorFullWidthMargins)
            {
                normal =
                {
                    background = GenerateColorTexture(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f)),
                    textColor  = Color.gray
                },
                fontSize  = 11,
                alignment = TextAnchor.MiddleCenter,
                richText  = true,
                wordWrap  = true,
                padding   = new RectOffset(12, 12, 12, 12)
            };
        }

        private static Texture2D GenerateColorTexture(int width, int height, Color col)
        {
            var result = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    result.SetPixel(x, y, col);
                }
            }

            result.Apply();
            return result;
        }


        private static Texture2D GenerateColorGradient(int width, int height, Gradient grad)
        {
            var result = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var t   = y * (1f / (width - 1));
                    var col = grad.Evaluate(t);
                    result.SetPixel(x, y, col);
                }
            }

            result.Apply();
            return result;
        }
    }
}