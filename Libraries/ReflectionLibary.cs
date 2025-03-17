using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class ReflectionLibrary
{
    public static object GetPrivateField(object obj, string fieldName)
    {
        FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return field?.GetValue(obj);
    }

    public static void SetPrivateField(object obj, string fieldName, object value)
    {
        FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(obj, value);
    }

    public static object InvokePrivateMethod(object obj, string methodName, params object[] parameters)
    {
        MethodInfo method = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        return method?.Invoke(obj, parameters);
    }
    
    public static Dictionary<string, object> GetAllProperties(object obj)
    {
        return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                   .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj));
    }

    public static Dictionary<string, object> GetAllFields(object obj)
    {
        return obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                   .ToDictionary(field => field.Name, field => field.GetValue(obj));
    }

    public static void TraverseObject(object obj, Action<string, object> action, string prefix = "")
    {
        if (obj == null) return;
        
        Type type = obj.GetType();

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            object value = field.GetValue(obj);
            string fieldName = prefix + field.Name;
            action(fieldName, value);
            if (value != null && !IsPrimitive(value))
                TraverseObject(value, action, fieldName + ".");
        }

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (!property.CanRead) continue;
            object value = property.GetValue(obj);
            string propName = prefix + property.Name;
            action(propName, value);
            if (value != null && !IsPrimitive(value))
                TraverseObject(value, action, propName + ".");
        }
    }

    private static bool IsPrimitive(object obj)
    {
        return obj is string || obj.GetType().IsPrimitive;
    }

    public static Type GetTypeByName(string typeName)
    {
        return Type.GetType(typeName) ?? AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
    }

    public static object CreateInstance(Type type, params object[] args)
    {
        return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, args, null);
    }

    public static object GetStaticField(Type type, string fieldName)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        return field?.GetValue(null);
    }

    public static void SetStaticField(Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        field?.SetValue(null, value);
    }

    public static object InvokeStaticMethod(Type type, string methodName, params object[] parameters)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        return method?.Invoke(null, parameters);
    }

    public static IEnumerable<MethodInfo> GetAllMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    }

    public static IEnumerable<FieldInfo> GetAllFields(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    }

    public static IEnumerable<Type> GetNestedTypes(Type type)
    {
        return type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
    }

    public static bool HasAttribute<T>(MemberInfo member) where T : Attribute
    {
        return member.GetCustomAttribute<T>() != null;
    }

    public static IEnumerable<Type> GetAllTypesFromAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
    }

    public static Type GetPrivateOrInternalClass(string fullName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == fullName);
    }
}
