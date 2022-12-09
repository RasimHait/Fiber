using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FiberFramework.Editor
{
    public class FiberObjectEditorContextMenu : PopupWindowContent
    {
        private readonly object             _targetCopyPaste;
        private readonly object             _targetEdit;
        private readonly object             _targetReset;
        private readonly FiberObject        _parent;
        private          SerializedProperty _serializedProperty;


        public FiberObjectEditorContextMenu(object copyOperationTarget, object resetOperationTarget, object editOperationTarget, FiberObject targetParent)
        {
            _targetCopyPaste = copyOperationTarget;
            _targetReset     = resetOperationTarget;
            _targetEdit      = editOperationTarget;
            _parent          = targetParent;
        }


        public override Vector2 GetWindowSize()
        {
            return new Vector2(50, 88);
        }


        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("Reset"))
            {
                ResetValue();
                UpdateHandlers();
                editorWindow.Close();
            }

            if (GUILayout.Button("Paste"))
            {
                OnPaste();
                UpdateHandlers();
                editorWindow.Close();
            }

            if (GUILayout.Button("Copy"))
            {
                OnCopy();
                editorWindow.Close();
            }

            if (GUILayout.Button("Edit"))
            {
                _targetEdit.GetType().OpenScript();
                editorWindow.Close();
            }

            GUILayout.EndVertical();

            editorWindow.Repaint();
        }


        private void UpdateHandlers()
        {
            var method = _parent.GetType().GetMethod("UpdateHandlers", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            method?.Invoke(_parent, null);
        }

        private void ResetValue()
        {
            switch (_targetReset)
            {
                case FiberModel:
                    _parent.RefreshModel();
                    break;
                case FiberView:
                    _parent.RefreshView();
                    break;
                case FiberControllerConfigurations:
                    _parent.RefreshConfigurations();
                    break;
            }

            EditorUtility.SetDirty(_parent);
        }


        private void OnCopy()
        {
            CopyToClipboard();
        }


        private void OnPaste()
        {
            if (PasteFromClipboard())
            {
                EditorUtility.SetDirty(_parent);
            }
        }


        private void CopyToClipboard()
        {
            var dataString = JsonUtility.ToJson(_targetCopyPaste);
            GUIUtility.systemCopyBuffer = dataString;
        }


        private bool PasteFromClipboard()
        {
            var dataString = GUIUtility.systemCopyBuffer;

            try
            {
                var last = JsonUtility.ToJson(_targetCopyPaste);
                JsonUtility.FromJsonOverwrite(dataString, _targetCopyPaste);
                var current = JsonUtility.ToJson(_targetCopyPaste);

                if (!current.Equals(last))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                // ignored
            }

            return false;
        }
    }
}