using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathDefinition : MonoBehaviour {


    [SerializeField]
    private Transform[] points;         // Array of waypoints.

    /// <summary>
    /// Cycles through the different waypoints.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Transform> GetPathsEnumerator()
    {
       if(points == null || points.Length < 1)      // Checks to make sure that we have atleast 1 waypoint if we don't then breaks out of the function.
       {
           yield break;
       }

        var direction = 1;          // Sets the direction we travle through the array.
        var index = 1;              // Sets the array index to 1
        while (true)                // Creates a controlled infinite loop.
        {
            yield return points[index];     // returns the transform for the current waypoint.

            if(points.Length == 1)          // makes sure we have atleast 1 item in the array.
            {
                continue;                   // continues through the loop.
            }

            if (index <= 0)                 // Checks to see if we have reach the beginning of the array.
            {
                direction = 1;              // sets the direction to 1 so we can step through the array.
            }else if(index >= points.Length - 1)    // checks to see if we have reach the end of the array.
            {
                direction = -1;             // Sets the direction to -1 so we can begin going backwards through the array.
            }
            index = index + direction;      // moves us back and forth through the array.
        }
    }
    // Draws lines between the waypoints in the inspector.
    public void OnDrawGizmos()
    {
        if(points == null || points.Length < 2)         // Makes sure we have at least 2 points to draw a line to.
        {
            return;
        }
        for(int i = 1; i < points.Length; i++)  
        {
            Gizmos.DrawLine(points[i - 1].position, points[i].position);    // Draws a line between each point in the array.
        }
    }
}
