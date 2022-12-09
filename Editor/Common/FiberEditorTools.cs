using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FiberFramework.Editor
{
    internal static class FiberEditorTools
    {
        internal static (List<Type> types, string[] strings) GetControllerList()
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
            resultNames.Insert(0, "None");

            return (resultTypes, resultNames.ToArray());
        }


        internal static Type[] GetHandlers(Type targetType)
        {
            var rootType   = typeof(IFiberHandler);
            var interfaces = targetType.GetInterfaces();

            return interfaces.Where(element => rootType.IsAssignableFrom(element) && element != rootType).ToArray();
        }


        internal static IEnumerable<FieldInfo> GetFields(Type targetType)
        {
            var fields = targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            var result = fields.Where(x => (x.IsPublic                                                      ||
                                            (!x.IsPublic && Attribute.IsDefined(x, typeof(SerializeField))) ||
                                            (!x.IsPublic && Attribute.IsDefined(x, typeof(SerializeReference)))) &&
                                           !x.IsInitOnly                                                         &&
                                           !x.IsStatic);
            return result;
        }
        
        
        internal static bool TryGetDescription(Type targetType, out string value)
        {
            if (Attribute.GetCustomAttribute(targetType, typeof(ControllerDescription)) is ControllerDescription descriptionAttribute)
            {
                value = descriptionAttribute.Description;
                return true;
            }

            value = default;
            return false;
        }
        
        
        internal static object GetFieldValue(object source, string propertyName)
        {
            var targetField = source.GetType().GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var value       = targetField?.GetValue(source);
            
            return value;
        }
    }
}