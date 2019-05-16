#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

[System.Serializable]
public class Range
{
    public float min;
    public float max;
    public float current;

    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
    
    public void Set(float value)
    {
        if (value < min) value = min;
        if (value > max) value = max;
        current = value;
    }

    public float Size
    { get { return max - min; } }

    public float Centre
    { get { return min + ((max - min) / 2); } }

    public static Range operator *(Range range, float scalar)
    {
        return new Range(range.min * scalar, range.max * scalar);
    }
}

[System.Serializable]
public class RangeInt
{
    public int min;
    public int max;
    public int current;

    public RangeInt(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public void Set(int value)
    {
        if (value < min) value = min;
        if (value > max) value = max;
        current = value;
    }

    public int Size
    { get { return max - min; } }

    public float Centre
    { get { return min + ((max - min) / 2); } }

    public static Range operator *(RangeInt range, float scalar)
    {
        return new Range(range.min * scalar, range.max * scalar);
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RangeInt))]
[CustomPropertyDrawer(typeof(Range))]
public class RangeDrawer : PropertyDrawer
{
    const int propWidth = 40;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(position, label);
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        position.x = (position.width + 14) - ((propWidth * 3) + 45); // why 14?
        position = AddLabelledProp(position, property, "min", "[", 10);
        position = AddLabelledProp(position, property, "max", ",", 10);
        position = AddLabelledProp(position, property, "current", "] =", 25);
        EditorGUI.indentLevel = indent;
    }

    Rect AddLabelledProp(Rect position, SerializedProperty property, string propName, string propLabel, float propGap)
    {
        EditorGUI.LabelField(new Rect(position.x, position.y, propWidth, position.height), new GUIContent(propLabel));
        position.x += propGap;
        SerializedProperty prop = property.FindPropertyRelative(propName);
        EditorGUI.PropertyField(new Rect(position.x, position.y, propWidth, position.height), prop, GUIContent.none);
        position.x += propWidth;

        return position;
    }
}
#endif