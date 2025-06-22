using FieldEditorTool;
using System;
using System.Reflection;
using UnityEngine;

public class DataComponentTracker : MonoBehaviour
{
    DataComponent component;
#if UNITY_EDITOR
    readonly Color gizmoColor = new(0.5f, 0.66f, 0.69f, 0.15f);
    void OnDrawGizmos()
    {

        if (component == null) component = GetComponent<DataComponent>();
        if (component == null) return;

        if (component.Data is FieldData field)
        {
            Gizmos.color = gizmoColor;
            Vector3 center = field.Position + (Vector3)field.Size / 2f;
            Vector3 size = field.Size;

            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, size);
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
            Gizmos.DrawWireCube(Vector3.zero, size);
        }

        SetValue(component.Data, "Position", transform.localPosition);
        SetValue(component.Data, "Rotation", transform.localEulerAngles);
    }
    static void SetValue(object data, string fieldName, object value)
    {
        Type valueType = value?.GetType();

        FieldInfo field = data.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null && field.FieldType.IsAssignableFrom(valueType))
        {
            field.SetValue(data, value);
            return;
        }

        PropertyInfo property = data.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null && property.CanWrite && property.PropertyType.IsAssignableFrom(valueType))
        {
            property.SetValue(data, value);
        }
    }
#endif
}
