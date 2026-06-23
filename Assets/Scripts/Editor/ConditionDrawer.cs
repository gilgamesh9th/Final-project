using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var keyProp   = property.FindPropertyRelative("variableKey");
        var compProp  = property.FindPropertyRelative("comparison");
        var valueProp = property.FindPropertyRelative("value");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing    = EditorGUIUtility.standardVerticalSpacing;
        Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

        EditorGUI.PropertyField(rect, keyProp);
        rect.y += lineHeight + spacing;

        EditorGUI.PropertyField(rect, compProp);
        rect.y += lineHeight + spacing;

        Comparison comp = (Comparison)compProp.enumValueIndex;
        if (comp != Comparison.Even && comp != Comparison.Odd)
        {
            EditorGUI.PropertyField(rect, valueProp);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing    = EditorGUIUtility.standardVerticalSpacing;

        var compProp = property.FindPropertyRelative("comparison");
        Comparison comp = (Comparison)compProp.enumValueIndex;

        int lines = (comp == Comparison.Even || comp == Comparison.Odd) ? 2 : 3;
        return lines * lineHeight + (lines - 1) * spacing;
    }
}