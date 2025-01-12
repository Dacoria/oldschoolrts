using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class UtilCI
{
    public static void ComponentInject(this MonoBehaviour monoBehaviour)
    {
        var injectableFields = monoBehaviour.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(x => x.GetCustomAttributes(typeof(ComponentInject), true).Length >= 1)
            .Select(x => x).ToList();

        var injectableProperties = monoBehaviour.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(x => x.GetCustomAttributes(typeof(ComponentInject), true).Length >= 1)
            .Select(x => x).ToList();

        foreach (var injectableField in injectableFields)
        {
            var fieldType = injectableField.FieldType;

            var componentInject = (ComponentInject)injectableField.GetCustomAttributes
                (typeof(ComponentInject), false).Single();
            var valueToInject = GetValueToInject(fieldType, componentInject, monoBehaviour);
            injectableField.SetValue(monoBehaviour, valueToInject);
        }

        foreach (var injectableProperty in injectableProperties)
        {
            var propertyType = injectableProperty.PropertyType;
            var componentInject = (ComponentInject)injectableProperty.GetCustomAttributes
                (typeof(ComponentInject), false).Single();
            var valueToInject = GetValueToInject(propertyType, componentInject, monoBehaviour);
            injectableProperty.SetValue(monoBehaviour, valueToInject);
        }
    }

    private static object GetValueToInject(Type type, ComponentInject componentInject, MonoBehaviour monoBehaviour)
    {
        if (type.GetInterfaces().Contains(typeof(IEnumerable)))
        {
            var list = new ArrayList();

            //var list = (IList)Activator.CreateInstance(type);
            Type componentType = null;
            if (type.IsArray)
            {
                componentType = type.GetElementType();
            }
            else
            {
                componentType = type.GetGenericArguments()[0];
            }

            if (componentInject.SearchDirection == SearchDirection.CHILDREN
                || componentInject.SearchDirection == SearchDirection.ALL
                || componentInject.SearchDirection == SearchDirection.DEFAULT)
            {
                var components = monoBehaviour.GetComponentsInChildren(componentType, true);
                foreach (var component in components)
                {
                    list.Add(component);
                }
            }

            if (componentInject.SearchDirection == SearchDirection.PARENT
                || componentInject.SearchDirection == SearchDirection.ALL
                || componentInject.SearchDirection == SearchDirection.DEFAULT)
            {
                var parentComponent = monoBehaviour.GetComponentInParent(componentType);
                if (parentComponent != null)
                {
                    list.Add(parentComponent);
                }
            }

            if (list.Count == 0 && (
                componentInject.Required == Required.DEFAULT 
                || componentInject.Required == Required.REQUIRED))
            {
                throw new Exception($"Required component '{type}' on behaviour '{monoBehaviour}' not found");
            }

            if (type.IsArray)
            {
                var array = Array.CreateInstance(componentType, list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    array.SetValue(list[i], i);
                }
                return array;
            }
            else
            {
                var l = (IList)Activator.CreateInstance(type);
                foreach (var VARIABLE in list)
                {
                    l.Add(VARIABLE);
                }

                return l;
            }
        }
        else
        {
            Component component = null;
            if (componentInject.SearchDirection == SearchDirection.CHILDREN
                || componentInject.SearchDirection == SearchDirection.ALL
                || componentInject.SearchDirection == SearchDirection.DEFAULT)
            {
                component = monoBehaviour.GetComponentInChildren(type, true);
            }

            if (component == null && (componentInject.SearchDirection == SearchDirection.PARENT
                                            || componentInject.SearchDirection == SearchDirection.ALL
                                            || componentInject.SearchDirection == SearchDirection.DEFAULT))
            {
                component = monoBehaviour.GetComponentInParent(type);
            }

            if (component == null && (
                componentInject.Required == Required.DEFAULT
                || componentInject.Required == Required.REQUIRED))
            {
                throw new Exception($"Required component '{type}' on behaviour '{monoBehaviour}' not found");
            }
            return component;
        }
    }
}