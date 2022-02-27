using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float checkpointReachedDist = 0.1f;

    private string playerId = null;
    private List<Vector3> waypoints = new List<Vector3>();


    public void AddWaypoints(IEnumerable<Vector3> waypoint)
    {
        waypoints.AddRange(waypoint);
    }
    
    private void Update()
    {
        if (waypoints.Count == 0) return;
        if (IsCheckpointReached())
        {
            waypoints.RemoveAt(0);
        }
        else
        {
            MoveTowardsNextCheckpoint(waypoints[0]);
        }
    }

    private void MoveTowardsNextCheckpoint(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        transform.LookAt(target);
    }

    private bool IsCheckpointReached()
    {
        return Vector3.Distance(transform.position, waypoints[0]) < checkpointReachedDist;
    }
}
