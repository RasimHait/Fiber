using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FiberFramework.Editor
{
    public static class FiberEditorExtensions
    {
        public static bool OpenScript(this Type type, params string[] folders)
        {
            var asset = GetScriptAsset(type, folders);
            if (asset == null)
            {
                Debug.LogError("Edit script error: Make sure the file name is same as type name!");
                return false;
            }

            return AssetDatabase.OpenAsset(asset.GetInstanceID(), 0, 0);
        }

        public static MonoScript GetScriptAsset(this Type type, params string[] folders)
        {
            var typeName = type.Name;
            var filter   = $"t:script {typeName}";

            var assets = AssetDatabase.FindAssets(filter, folders);
            var assetGuid = assets.FirstOrDefault(
                x => string.Equals(typeName,
                    Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(x)),
                    StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(assetGuid))
                return null;

            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(assetGuid));
            return asset;
        }

        private static readonly Dictionary<GUIStyle, GUIStyle> _styles = new Dictionary<GUIStyle, GUIStyle>();

        public static void StartEdit(this GUIStyle style)
        {
            style.StopEdit();
            try
            {
                _styles.Add(style, new GUIStyle(style));
            }
            catch (Exception e)
            {
                Debug.Log("You must StopEdit this style first.");
            }
        }

        public static void StopEdit(this GUIStyle style)
        {
            if (_styles.ContainsKey(style))
            {
                var fields     = style.GetType().GetFields(BindingFlags.Public);
                var properties = style.GetType().GetProperties(BindingFlags.Public);

                foreach (var field in fields)
                {
                    var targetValue = field.GetValue(_styles[style]);
                    field.SetValue(style, targetValue);
                }
                
                foreach (var property in properties)
                {
                    var targetValue = property.GetValue(_styles[style]);
                    property.SetValue(style, targetValue);
                }

                _styles.Remove(style);
            }
        }
    }
}