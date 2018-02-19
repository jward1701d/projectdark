using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour
{
    #region ENUM
    // Choice between to moving options.
    public enum FollowType
    {
        MoveTowards,
        Lerp
    }
    #endregion

    #region Fields / Variables
    [SerializeField]
    private FollowType type = FollowType.MoveTowards;   // sets the movement type to defualt of move towards.
    [SerializeField]
    private PathDefinition path;                        // path for the object to follow.
    [SerializeField]
    private float speed;                                // the speed the object travels along the path.
    [SerializeField]      
    private float MaxDistanceToGoal = 0.1f;             // the distance the object is form the current waypoint.

    private IEnumerator<Transform> currentPoint;        // the target point the object is moving towards.
    #endregion

    #region Unity Methods { Start(), Update()}
    public void Start()
    {
        if(path == null)                                // Makes sure the path is not null.
        {
            Debug.LogError("Path cannot be null", gameObject);  // Gives a log error if the path is null letting the user knwo they need a path.
            return;
        }

        currentPoint = path.GetPathsEnumerator();       // sets the current waypoint in the path.
        currentPoint.MoveNext();                        // Moves toward that waypoint.
        if(currentPoint.Current == null)                // checks if the current point is null.
        {
            return;
        }
        transform.position = currentPoint.Current.position;     // moves the object toward the waypoinys position.
    }
    public void Update()
    {
        if(currentPoint == null || currentPoint.Current == null)    // Makes we have no null points in out path.
        {
            return;
        }
        if(type == FollowType.MoveTowards)
        {
            // Moves the object at a steady speed from point to point without stopping.
            transform.position = Vector3.MoveTowards(transform.position, currentPoint.Current.position, Time.deltaTime * speed);    
        }else if(type == FollowType.Lerp)
        {
            // Moves the object in a smooth elastic motion from point to point.
            transform.position = Vector3.Lerp(transform.position, currentPoint.Current.position, Time.deltaTime * speed);
        }

        // uses a generics variable to calculate the objects distance between it's current position and the target position. Uses sqrMagnitude becasue
        // it is less system intensive than division. 
        var distanceSquared = (transform.position - currentPoint.Current.position).sqrMagnitude; 
        if(distanceSquared < (MaxDistanceToGoal * MaxDistanceToGoal))   // Checks if the object is close enough to the current waypoint to continue to the next.
        {
            currentPoint.MoveNext();
        }
    }
    #endregion
}
