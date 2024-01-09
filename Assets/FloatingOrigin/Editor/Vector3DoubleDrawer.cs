using UnityEngine;
using UnityEditor;

/*[CustomPropertyDrawer(typeof(Vector3Double))]
public class Vector3DoubleDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var xVal = property.FindPropertyRelative("x");
        var yVal = property.FindPropertyRelative("y");
        var zVal = property.FindPropertyRelative("z");

        Vector3Double val = new Vector3Double(xVal.doubleValue, yVal.doubleValue, zVal.doubleValue);
        val = GUIUtil.Vector3DoubleField(position, new GUIContent(property.displayName), val);

        xVal.doubleValue = val.x;
        yVal.doubleValue = val.y;
        zVal.doubleValue = val.z;
    }
}*/
