using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class DefaultEditor : Editor
{
    Editor defaultEditor;
    
    protected void CreateDefaultEditor(Type defaultEditorType) {
        defaultEditor = Editor.CreateEditor(targets, defaultEditorType);
    }


    // Very important to call, or else there can be memory leaks
    protected void DestroyDefaultEditor() {
        MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (disableMethod != null) {
            disableMethod.Invoke(defaultEditor,null);
        }

        DestroyImmediate(defaultEditor);
    }


    protected void DefaultInspectorGUI() {
        if (defaultEditor == null) {
            throw new Exception("Default Editor does not exist. Valid Editor type must be created with CreateDefaultEditor()");
        }

        defaultEditor.OnInspectorGUI();
    }
}
