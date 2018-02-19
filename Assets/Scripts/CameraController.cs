using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private float xMax;                     // Max range the camera will go along the X axis.
    [SerializeField]    
    private float yMax;                     // Max range the camera will go along the Y axis.
    [SerializeField]
    private float xMin;                     // Minium range the camera will go along the X axis.
    [SerializeField]
    private float yMin;                     // Minium range the camera will go along the Y axis.

    private Transform target;               // The target for the camera to follow.

    #region Unity Methods {Start(), LateUpdate()}
    // Use this for initialization
    void Start () {
        target = Player.Instance.transform;     // Sets the target of the camera to the player.
	}
	
    void LateUpdate()
    {
        // Clamps the cameras range between the min and max we set for it in the inspector.
        transform.position = new Vector3(Mathf.Clamp(target.position.x,
            xMin, 
            xMax), 
            Mathf.Clamp(target.position.y, 
            yMin, 
            yMax),
            transform.position.z);
    }

    #endregion
}
