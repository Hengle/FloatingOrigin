using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

 
[CustomEditor(typeof(Transform), true)]
[CanEditMultipleObjects]
public class FloatingTransformDrawer : DefaultEditor
{ 
    Transform transform;

    void OnEnable() {
        CreateDefaultEditor(Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
        transform = target as Transform;
    }
 
    void OnDisable() {
        DestroyDefaultEditor();
    }
 
    public override void OnInspectorGUI(){
        EditorGUILayout.LabelField("Local Space", EditorStyles.boldLabel);
        DefaultInspectorGUI();
    }

    /*
        // This is the value at which Unity starts nagging about floating-point precision.
        const float minEditThreshold = 1000000.0f;

        private FloatingTransform currentFloatingObject = null;


        public override void OnEnable() {
            currentFloatingObject = null;
            base.OnEnable();
        }

        static Vector3Double ApplyChangedAxes(Vector3Double current, Vector3Double newPosition, bool changedX, bool changedY, bool changedZ) {
            current.x = changedX ? newPosition.x : current.x;
            current.y = changedY ? newPosition.y : current.y;
            current.z = changedZ ? newPosition.z : current.z;

            return current;
        }


        void OnSceneGUI() {
            if (currentFloatingObject == null) {
                return;
            }

            if (currentFloatingObject.transform != transform) {
                currentFloatingObject = null;
                return;
            }

            Vector3 transformPos = transform.position;
            Vector3Double newPos = currentFloatingObject.WorldPosition;

            for (int i = 0; i < 3; i++) {
                if (Mathf.Abs(transformPos[i]) < minEditThreshold && transformPos[i] != newPos[i]) {
                    newPos[i] = transformPos[i];
                }
            }

            currentFloatingObject.WorldPosition = newPos;
            transform.position = newPos;
        }


        bool DrawFloatingPosition() {
            if (currentFloatingObject == null) {
                return DrawTransformPosition();
            }

            EditorGUI.BeginChangeCheck();

            Rect control = EditorGUILayout.GetControlRect(true);

            Vector3Double worldPos = GUIUtil.Vector3DoubleField(control, new GUIContent("Floating Position"), currentFloatingObject.WorldPosition);

            if (EditorGUI.EndChangeCheck()) {
                Vector3Double currentPos = currentFloatingObject.WorldPosition;
                bool changedX = worldPos.x != currentPos.x;
                bool changedY = worldPos.y != currentPos.y;
                bool changedZ = worldPos.z != currentPos.z;

                Undo.RecordObject(currentFloatingObject, "Changed Floating Position (Internal)");
                currentFloatingObject.WorldPosition = worldPos;
                Undo.RecordObject(transform, "Changed Floating Position (Transform)");
                transform.position = worldPos;

                for (int i = 0; i < Selection.transforms.Length; i++) {
                    Transform selected = Selection.transforms[i];

                    if (selected.TryGetComponent<FloatingTransform>(out FloatingTransform fo)) {
                        Undo.RecordObject(selected, "Changed Floating Position (Transform)");
                        Undo.RecordObject(fo, "Changed Floating Position (Internal)");

                        Vector3Double newPos = ApplyChangedAxes(fo.WorldPosition, worldPos, changedX, changedY, changedZ);
                        fo.SyncWithTransform(newPos);
                    } else {
                        Undo.RecordObject(selected, "Changed Position (Transform)");

                        Vector3Double newPos = ApplyChangedAxes(selected.position, worldPos, changedX, changedY, changedZ);
                        selected.position = newPos;
                    }
                }
            }

            return false;
        }


        public override void OnInspectorGUI() { 
            if (currentFloatingObject != null && currentFloatingObject.transform != transform) {
                currentFloatingObject = null;
            }

            transform.TryGetComponent<FloatingTransform>(out currentFloatingObject);

            ApplyTransformChangesToSelection(DrawFloatingPosition(), DrawTransformRotation(), DrawTransformScale());


            bool hasFloatingObject = EditorGUILayout.Toggle("Floating Transform", currentFloatingObject != null);

            // Remove floating object
            if (!hasFloatingObject && currentFloatingObject != null) {
                transform.position = currentFloatingObject.WorldPosition;
                Undo.DestroyObjectImmediate(currentFloatingObject);
                EditorGUIUtility.ExitGUI();
                return;
            }

            if (hasFloatingObject && currentFloatingObject == null) {
                currentFloatingObject = transform.gameObject.AddComponent<FloatingTransform>();
                currentFloatingObject.WorldPosition = transform.position;
                Undo.RegisterCreatedObjectUndo(currentFloatingObject, "Set Floating Transform");
            }

            Vector3 pos = transform.position;

            if (Mathf.Abs(pos.x) >= minEditThreshold || Mathf.Abs(pos.y) >= minEditThreshold || Mathf.Abs(pos.z) >= minEditThreshold) {
                if (currentFloatingObject != null) {
                    EditorGUILayout.HelpBox("Scene View editing for position axes at large distances is disabled. Please adjust the position axis manually in the inspector.", MessageType.Info);
                } else {
                    EditorGUILayout.HelpBox("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.", MessageType.Warning);
                }
            }
        }
    */
}
