using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Meter))]
public class MeterDrawer : PropertyDrawer
{
    public float lineH = EditorGUIUtility.singleLineHeight;
    public float lineBreak = EditorGUIUtility.singleLineHeight + 4;
    public float lineCount = 3.5f;

    public static Color defaultMeterColor = Color.cyan, defaultMeterBgColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);
    public static Gradient defaultGradient = new Gradient();

    public bool showExtra = true;
    public float LineCount => lineCount + (showExtra ? 2.75f : 0);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var val = property.FindPropertyRelative("_value");
        var min = property.FindPropertyRelative("_min");
        var max = property.FindPropertyRelative("_max");
        var up = property.FindPropertyRelative("_rateUp");
        var down = property.FindPropertyRelative("_rateDown");

        var color = property.FindPropertyRelative("_colorMain");
        var bgColor = property.FindPropertyRelative("_colorBackground");
        var gradient = property.FindPropertyRelative("_meterGradient");
        var type = property.FindPropertyRelative("_colorType");

        float defaultLabelWidth = EditorGUIUtility.labelWidth;

        float percent = Mathf.Clamp01((val.floatValue - min.floatValue) / (max.floatValue - min.floatValue));

        Rect bg = position;
        bg.height -= 2f;

        GUI.Box(bg, GUIContent.none);

        int line = 0;
        int extraSpace = 0;
        float x = position.x;
        float y = position.y;
        float w = position.width;
        float fieldWidth = 40f;

        Rect nameRect = new Rect(position.x, position.y + (lineBreak * line), position.width, lineH);
        Rect fillButtonRect = new Rect(x + w - fieldWidth, y + 2, fieldWidth, lineH);
        Rect rateButtonRect = new Rect(x + (w / 2f) - (fieldWidth * 0.75f), y + 2, fieldWidth * 1.5f, lineH);
        line++;
        extraSpace += 6;
        Rect bgRect = new Rect(x, y + (lineBreak * line), w, lineH + extraSpace);
        Rect fillRect = new Rect(x, y + (lineBreak * line), w * percent, lineH + extraSpace);

        Rect minRect = new Rect(x + 2, y + (lineBreak * line) + extraSpace / 2, fieldWidth, lineH);
        Rect valRect = new Rect(x + w / 2 - fieldWidth / 2, y + (lineBreak * line) + extraSpace / 2, fieldWidth, lineH);
        Rect maxRect = new Rect(x - 2 + w - fieldWidth, y + (lineBreak * line) + extraSpace / 2, fieldWidth, lineH);
        line++;

        Rect minLabelRect = new Rect(x + 2, y + (lineBreak * line) + extraSpace / 2, fieldWidth, lineH);
        Rect valLabelRect = new Rect(x + w / 2 - fieldWidth / 2, y + (lineBreak * line) + extraSpace / 2, fieldWidth, lineH);
        Rect maxLabelRect = new Rect(x + w - 2 - fieldWidth, y + (lineBreak * line) + extraSpace / 2, fieldWidth, lineH);
        float newFieldWidth = fieldWidth * 1.25f;

        float arrowOffset = Mathf.Min(fieldWidth, w / 6);

        Rect downRect = new Rect(x + w / 2 - arrowOffset * 2f, y + (lineBreak * line) + extraSpace, newFieldWidth, lineH);
        Rect upRect = new Rect(x + w / 2 + arrowOffset, y + (lineBreak * line) + extraSpace, newFieldWidth, lineH);
        line++;
        Rect colorTypeRect = new Rect(x + w / 2, y + (lineBreak * line) + 2, w / 2, lineH);
        Rect defaultColorRect = new Rect(x, y + (lineBreak * line) + 4, w / 3, lineH);
        line++;
        Rect colorPickerRect = new Rect(x, y + (lineBreak * line) + 4, w, lineH);
        line++;
        Rect colorBgRect = new Rect(x, y + (lineBreak * line) + 4, w, lineH);

        EditorGUI.DrawRect(bgRect, bgColor.colorValue);
        Color fillCol = (Meter.ColorType)type.enumValueIndex == Meter.ColorType.single ? color.colorValue : GetGradient(gradient).Evaluate(percent);
        EditorGUI.DrawRect(fillRect, fillCol);
        EditorGUI.DrawRect(valRect, new Color(.3f, .3f, .3f, .8f));
        EditorGUI.DrawRect(minRect, new Color(.3f, .3f, .3f, .8f));
        EditorGUI.DrawRect(maxRect, new Color(.3f, .3f, .3f, .8f));

        EditorGUI.LabelField(nameRect, property.displayName);
        if (GUI.Button(rateButtonRect, "Rates x1"))
        {
            up.floatValue = 1;
            down.floatValue = 1;
        }
        if (GUI.Button(fillButtonRect, "Fill"))
        {
            val.floatValue = max.floatValue;
        }

        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUI.BeginChangeCheck();

        float oldMin = min.floatValue;
        float oldMax = max.floatValue;

        min.floatValue = EditorGUI.FloatField(minRect, min.floatValue, style);
        val.floatValue = EditorGUI.FloatField(valRect, val.floatValue, style);
        max.floatValue = EditorGUI.FloatField(maxRect, max.floatValue, style);

        EditorGUIUtility.labelWidth = 32f;

        style.fontStyle = FontStyle.Normal;
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUI.LabelField(minLabelRect, "Min", style);
        EditorGUI.LabelField(valLabelRect, "Current", style);
        EditorGUI.LabelField(maxLabelRect, "Max", style);

        EditorGUIUtility.labelWidth = 25;

        GUIContent lbl = new GUIContent();
        lbl.text = "x ->";
        Rect upLabelRect = upRect;
        upLabelRect.width /= 2;
        EditorGUI.PropertyField(upLabelRect, up, GUIContent.none);
        upLabelRect.x += upLabelRect.width;
        EditorGUI.LabelField(upLabelRect, lbl);

        lbl.text = "<- x";
        Rect downLabelRect = downRect;
        downLabelRect.width /= 2;
        EditorGUI.LabelField(downLabelRect, lbl);
        downLabelRect.x += downLabelRect.width;
        EditorGUI.PropertyField(downLabelRect, down, GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            if (min.floatValue != oldMin && min.floatValue > max.floatValue)
                min.floatValue = max.floatValue;
            if (max.floatValue != oldMax && max.floatValue < min.floatValue)
                max.floatValue = min.floatValue;
        }

        showExtra = EditorGUI.Foldout(nameRect, showExtra, "", false);
        if (showExtra)
        {
            
            EditorGUIUtility.labelWidth = defaultLabelWidth / 3;
            EditorGUI.PropertyField(colorTypeRect, type);
            EditorGUIUtility.labelWidth = defaultLabelWidth;

            switch ((Meter.ColorType)type.enumValueIndex)
            {
                case Meter.ColorType.single:
                    if (GUI.Button(defaultColorRect, "Default Colors"))
                    {
                        color.colorValue = defaultMeterColor;
                        bgColor.colorValue = defaultMeterBgColor;
                    }
                    color.colorValue = EditorGUI.ColorField(colorPickerRect, "Meter Color", color.colorValue);
                    break;
                case Meter.ColorType.gradient:
                    EditorGUI.PropertyField(colorPickerRect, gradient);
                    break;
                default:
                    Debug.LogError("Invalid color type selection, check colorType is valid");
                    break;
            }
            bgColor.colorValue = EditorGUI.ColorField(colorBgRect, "Meter Background", bgColor.colorValue);
        }

        EditorGUIUtility.labelWidth = defaultLabelWidth;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return lineBreak * LineCount;
    }

    public Gradient GetGradient(SerializedProperty gradientProperty)
    {
        System.Reflection.PropertyInfo propertyInfo = typeof(SerializedProperty).GetProperty("gradientValue", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (propertyInfo == null) { return null; }
        else { return propertyInfo.GetValue(gradientProperty, null) as Gradient; }
    }
}
