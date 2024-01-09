using UnityEngine;
using UnityEditor;


///<summary>Functions that the EditorGUI class does not expose but uses internally. Adapted from Unity's published C# reference</summary>
///<see href="https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/EditorGUI.cs">EditorGUI C# Reference</see>
///<remarks>This does not contain all hidden EditorGUI methods. It just contains what has been used in projects</remarks>
public static class GUIUtil
{
    /*const float prefixPaddingRight = 2;
    const float spacingSubLabel = 4;
    const float indentPerLevel = 15;

    private static float bigCharSize => EditorStyles.label.CalcSize(new GUIContent("W")).x;
    private static double[] vector3Doubles = new double[3];
    private static readonly GUIContent[] XYZLabels = { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") };
    private static int fHash = "Foldout".GetHashCode();


    public static Vector3Double Vector3DoubleField(Rect position, GUIContent label, Vector3Double value) {
        int id = GUIUtility.GetControlID(fHash, FocusType.Keyboard, position);
        position = MultiFieldPrefixLabel(position, id, label, 3);
        position.height = EditorGUIUtility.singleLineHeight;
        return Vector3DoubleField(position, value);
    }


    public static Vector3Double Vector3DoubleField(Rect position, Vector3Double value) {
        vector3Doubles[0] = value.x;
        vector3Doubles[1] = value.y;
        vector3Doubles[2] = value.z;
        position.height = EditorGUIUtility.singleLineHeight;
        
        EditorGUI.BeginChangeCheck();
        MultiDoubleField(position, XYZLabels, vector3Doubles);
        
        if (EditorGUI.EndChangeCheck()) {
            value.x = vector3Doubles[0];
            value.y = vector3Doubles[1];
            value.z = vector3Doubles[2];
        }

        return value;
    }


    static void MultiDoubleField(Rect position, GUIContent[] subLabels, double[] values, float prefixLabelWidth = -1) {
        int valCount = values.Length;
        float fieldWidth = (position.width - (valCount - 1) * spacingSubLabel) / valCount;
        
        position.width = fieldWidth;

        // Cache width and indent
        float curWidth = EditorGUIUtility.labelWidth;
        int curIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        
        for (int i = 0; i < values.Length; i++) {
            EditorGUIUtility.labelWidth = GetLabelWidth(subLabels[i], prefixLabelWidth);
            values[i] = EditorGUI.DoubleField(position, subLabels[i], values[i]);
            position.x += fieldWidth + spacingSubLabel;
        }

        // Reset width and indent
        EditorGUIUtility.labelWidth = curWidth;
        EditorGUI.indentLevel = curIndent;
    }


    static float GetLabelWidth(GUIContent label, float prefixLabelWidth = -1) {
        float LabelWidth = EditorStyles.label.CalcSize(label).x;

        if (LabelWidth > bigCharSize) {
            return prefixLabelWidth > 0 ? prefixLabelWidth : LabelWidth;
        }
        
        return prefixLabelWidth > 0 ? prefixLabelWidth : bigCharSize;
    }


    static Rect MultiFieldPrefixLabel(Rect totalPosition, int id, GUIContent label, int columns) {
        if (label == null || (label.text == string.Empty && label.image == null)) {
            return EditorGUI.IndentedRect(totalPosition);
        }

        Rect labelPosition, fieldPosition;

        if (EditorGUIUtility.wideMode) {
            labelPosition = new Rect(totalPosition.x + EditorGUI.indentLevel, totalPosition.y, EditorGUIUtility.labelWidth - EditorGUI.indentLevel, EditorGUIUtility.singleLineHeight);
            fieldPosition = totalPosition;

            fieldPosition.xMin += EditorGUIUtility.labelWidth + prefixPaddingRight;

            if (columns == 2) {
                float columnWidth = (fieldPosition.width - (3 - 1) * spacingSubLabel) / 3f;
                fieldPosition.xMax -= (columnWidth + spacingSubLabel);
            }
        } else {
            labelPosition = new Rect(totalPosition.x + EditorGUI.indentLevel, totalPosition.y, totalPosition.width - EditorGUI.indentLevel, EditorGUIUtility.singleLineHeight);
            fieldPosition = totalPosition;

            fieldPosition.xMin += EditorGUI.indentLevel + indentPerLevel;
            fieldPosition.yMin += EditorGUIUtility.singleLineHeight;
        }

        EditorGUI.HandlePrefixLabel(totalPosition, labelPosition, label, id);
        return fieldPosition;
    }*/
}
