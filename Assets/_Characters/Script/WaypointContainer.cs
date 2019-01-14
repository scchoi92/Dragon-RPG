using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class WaypointContainer : MonoBehaviour
    {
        [SerializeField] GameObject[] waypoints;

        void OnDrawGizmos()
        {
            if(transform.childCount != waypoints.Length)
            {
                print(transform.name + " waypoint array should be fixed.");
            }
            
            for (int waypointIndex = 0; waypointIndex < waypoints.Length - 1; waypointIndex++)
            {
                Gizmos.color = new Color(255f, 255f, 255f, 1f);
                Gizmos.DrawLine(waypoints[waypointIndex].transform.position, waypoints[waypointIndex + 1].transform.position);

                Gizmos.DrawWireSphere(waypoints[waypointIndex].transform.position, 0.1f);
            }
            Gizmos.color = new Color(255f, 255f, 255f, 1f);
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].transform.position, waypoints[0].transform.position);
            Gizmos.DrawWireSphere(waypoints[waypoints.Length - 1].transform.position, 0.1f);
        }
    }
}