using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif


public static class ReferenceFrame
{
    private static List<FloatingTransform> trackedTransforms = new List<FloatingTransform>(256);


    private static void OnSceneLoad(Scene scene, LoadSceneMode mode) 
    {
        if (mode == LoadSceneMode.Single)
            return;
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void PreSceneLoad() 
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        SceneManager.sceneLoaded += OnSceneLoad;
    }


#if UNITY_EDITOR
    private static void OnApplicationStateChange(PlayModeStateChange stateChange) 
    {
        if (stateChange == PlayModeStateChange.EnteredEditMode || stateChange == PlayModeStateChange.EnteredPlayMode)
            return;
    }


    [InitializeOnLoadMethod]
    static void InitializeOnLoad() 
    {
        EditorApplication.playModeStateChanged -= OnApplicationStateChange;
        EditorApplication.playModeStateChanged += OnApplicationStateChange;
    }
#endif


    ///<summary>Adds a FloatingTransform to the tracked transforms updated by the Origin Manager</summary>
    ///<param name="foTransform">The FloatingTransform to track</param>
    public static void TrackTransform(FloatingTransform foTransform) 
    {
        if (foTransform.transform.parent != null) 
        {
            if (foTransform.transform.parent.GetComponentInParent<FloatingTransform>() != null) 
                throw new InvalidHierarchyException("Floating Transform cannot be child of another Floating Transform!");
        }

        trackedTransforms.Add(foTransform);
    }


    ///<summary>Removes a FloatingTransform from the tracked transforms updated by the Origin Manager</summary>
    ///<param name="foTransform">The FloatingTransform to untrack</param>
    public static void UntrackTransform(FloatingTransform foTransform) 
    {
        trackedTransforms.Remove(foTransform);
    }
}
