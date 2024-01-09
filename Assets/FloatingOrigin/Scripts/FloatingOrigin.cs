using UnityEngine;


[AddComponentMenu("Floating Origin"), DefaultExecutionOrder(300)]
public class FloatingOrigin : MonoBehaviour
{
    private static FloatingOrigin _instance;

    public static FloatingOrigin Instance 
    {
        get 
        {
            if (_instance == null) 
                throw new MissingComponentException("No FloatingOrigin Instance is currently available");   

            return _instance;
        }
    }

    public OriginUpdateStage updateStage = OriginUpdateStage.FixedUpdate;
    public Transform originFocus;
 

    void Awake() 
    {
        if (_instance == null) 
        {
            _instance = this;
        } 
        else 
        {
            Debug.LogError(
                "Only one FloatingOrigin can be active in the scene at any given time." +
                "To change the currently active origin focus, set it through script or in the inspector", 
            this);
        }
    }
}
