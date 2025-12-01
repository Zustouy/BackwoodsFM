using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Атрибут теперь виден и во время сборки
public class CustomRangeAttribute : PropertyAttribute
{
    public float min1, max1, min2, max2;

    public CustomRangeAttribute(float min1, float max1, float min2, float max2)
    {
        this.min1 = min1;
        this.max1 = max1;
        this.min2 = min2;
        this.max2 = max2;
    }
}

#if UNITY_EDITOR
// Рисователь остаётся только для редактора
[CustomPropertyDrawer(typeof(CustomRangeAttribute))]
public class CustomRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PropertyField(position, property, label);

        if (property.propertyType == SerializedPropertyType.Float)
        {
            float value = property.floatValue;
            var attr = (CustomRangeAttribute)attribute;

            bool inRange1 = value >= attr.min1 && value <= attr.max1;
            bool inRange2 = value >= attr.min2 && value <= attr.max2;

            if (!inRange1 && !inRange2)
            {
                float closest1 = Mathf.Clamp(value, attr.min1, attr.max1);
                float closest2 = Mathf.Clamp(value, attr.min2, attr.max2);
                float dist1 = Mathf.Abs(value - closest1);
                float dist2 = Mathf.Abs(value - closest2);

                property.floatValue = dist1 < dist2 ? closest1 : closest2;
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif
