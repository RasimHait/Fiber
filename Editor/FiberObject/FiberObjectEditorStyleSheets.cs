using UnityEditor;
using UnityEngine;

namespace FiberFramework.Editor
{
    public class FiberObjectEditorStyleSheets
    {
        public readonly GUIStyle mainContainer;
        public readonly GUIStyle controllerSelector;
        public readonly GUIStyle controllerSelectionElement;
        public readonly GUIStyle controllerSelectionButton;
        public readonly GUIStyle controllerSelectionNameLabel;
        public readonly GUIStyle controllerSelectionDescriptionLabel;
        public readonly GUIStyle fieldsContainerHeader;
        public readonly GUIStyle fieldsContainerTitle;
        public readonly GUIStyle fieldsContainerFields;
        public readonly GUIStyle descriptionTextBox;
        public readonly GUIStyle contextMenuButton;
        public readonly GUIStyle infoBlockLeftLine;
        public readonly GUIStyle infoBlockRightLine;
        public readonly GUIStyle infoBlock;

        public Texture2D icon_menu;

        private Texture2D _color_controllerSelector;
        private Texture2D _color_controllerSelectorButton;
        private Texture2D _color_controllerSelectorButtonHover;
        private Texture2D _color_menuHeader;

        private void PrepareColors()
        {
            _color_controllerSelector            = GenerateColorGradient(40, new Color(0.16f, 0.16f, 0.16f), new Color(0.22f, 0.22f, 0.22f));
            _color_menuHeader                    = GenerateColorTexture(new Color(0.16f, 0.16f, 0.16f));
            _color_controllerSelectorButton      = GenerateColorTexture(new Color(0.5f, 0.5f, 0.5f));
            _color_controllerSelectorButtonHover = GenerateColorTexture(new Color(0.2f, 0.2f, 0.2f));
        }

        public FiberObjectEditorStyleSheets()
        {
            PrepareColors();

            icon_menu = Resources.Load<Texture2D>("icon_menu");

            mainContainer = new GUIStyle()
            {
                padding = new RectOffset(0, 0, 0, 0),
            };

            controllerSelector = new GUIStyle()
            {
                fixedHeight = 40,
                fontStyle   = FontStyle.Bold,
                fontSize    = 15,
                alignment   = TextAnchor.MiddleCenter,
                normal =
                {
                    background = _color_controllerSelector,
                    textColor  = Color.white
                }
            };
            controllerSelectionElement = new GUIStyle()
            {
                fixedHeight = 20,
            };

            controllerSelectionNameLabel = new GUIStyle()
            {
                normal =
                {
                    textColor = Color.white,
                },
                alignment     = TextAnchor.MiddleLeft,
                contentOffset = new Vector2(12, -18),
            };

            controllerSelectionDescriptionLabel = new GUIStyle()
            {
                normal =
                {
                    textColor = new Color(0.18f, 0.18f, 0.18f),
                },
                fontStyle     = FontStyle.Italic,
                alignment     = TextAnchor.MiddleRight,
                contentOffset = new Vector2(-3, -18),
            };

            controllerSelectionButton = new GUIStyle()
            {
                fixedHeight = 20,
                normal =
                {
                    background = _color_controllerSelectorButton
                },
                hover =
                {
                    background = _color_controllerSelectorButtonHover
                }
            };

            fieldsContainerHeader = new GUIStyle
            {
                fixedHeight = 23,
                richText    = true,
                normal =
                {
                    background = _color_menuHeader
                }
            };

            fieldsContainerFields = new GUIStyle
            {
                margin = new RectOffset(13, 1, 5, 5),
            };

            fieldsContainerTitle = new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft,
                padding   = new RectOffset(fieldsContainerFields.margin.left, 0, 0, 0),
                fontSize  = 13,
                richText  = true,
                normal =
                {
                    textColor = Color.white,
                }
            };

            contextMenuButton = new GUIStyle(EditorStyles.iconButton)
            {
                padding     = new RectOffset(2, 2, 2, 2),
                margin      = new RectOffset(0, 3, 2, 0),
                fixedWidth  = 20,
                fixedHeight = 20,
                alignment   = TextAnchor.MiddleRight
            };

            descriptionTextBox = new GUIStyle(EditorStyles.inspectorFullWidthMargins)
            {
                normal =
                {
                    background = _color_menuHeader,
                    textColor  = Color.gray
                },
                fontSize  = 11,
                alignment = TextAnchor.MiddleCenter,
                richText  = true,
                wordWrap  = true,
                padding   = new RectOffset(12, 12, 5, 5)
            };
            
            infoBlockLeftLine = new GUIStyle()
            {
                alignment = TextAnchor.UpperLeft,
                richText  = true,
                fontSize  = 11,
                wordWrap  = true,
            };
            infoBlockRightLine = new GUIStyle()
            {
                alignment = TextAnchor.MiddleRight,
                richText  = true,
                fontSize  = 11,
                wordWrap  = true,
            };
            infoBlock = new GUIStyle()
            {
                padding = new RectOffset(16, 6, 3, 0),
                normal =
                {
                    background = FiberObjectEditorStyleSheets.GenerateColorTexture(new Color(0.25f, 0.25f, 0.25f))
                }
            };
        }

        public static Texture2D GenerateColorTexture(Color col)
        {
            var result = new Texture2D(1, 1, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };

            result.SetPixel(0, 0, col);

            result.Apply();
            return result;
        }


        public static Texture2D GenerateColorGradient(int height, Color from, Color to)
        {
            var result = new Texture2D(1, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };

            for (var y = 0; y < height; y++)
            {
                var color = Color.Lerp(from, to, (float)y / height);
                result.SetPixel(0, y, color);
            }

            result.Apply();
            return result;
        }
    }
}