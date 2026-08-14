using System;
using System.Reflection;
using UnityEngine;

public static class GameObjectExtensions
{
    public static T CopyComponentTo<T>(this T original, GameObject destination) where T : Component
    {
        if (original == null || destination == null)
            return null;

        // Get the component type
        Type type = original.GetType();

        // Get or add the component to the destination
        T copy = destination.GetComponent<T>();
        if (copy == null)
        {
            copy = destination.AddComponent<T>();
        }

        // Copy all fields using reflection
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            try
            {
                object value = field.GetValue(original);
                if (value != null)
                {
                    field.SetValue(copy, value);
                }
            }
            catch (Exception ex)
            {
                // Log the error but continue copying other fields
                Debug.LogWarning($"Failed to copy field {field.Name}: {ex.Message}");
            }
        }

        return copy;
    }
}
