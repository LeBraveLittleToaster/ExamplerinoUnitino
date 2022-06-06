using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovePlayerOverTerrainScript : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;
    [SerializeField] private bool rotatePlayer;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float reachedWaypointDistance;

    private Queue<Vector3> _waypoints = new();
    
    // Update is called once per frame
    void Update()
    {
        if (_waypoints.Count == 0)
        {
            var nextPos = RandomPositionInTerrainBounds(_terrain);
            Debug.Log("Next Pos: " + nextPos);
            _waypoints.Enqueue(nextPos);
        }

        CheckAndMoveToNextCheckpoint(_waypoints);
    }

    private void LateUpdate()
    {
        transform.position = GetYPosForTerrain(_terrain, transform.position);
    }

    private Vector3 GetYPosForTerrain(Terrain terrain, Vector3 position)
    {
        Debug.Log(terrain.SampleHeight(position));
        return new Vector3(position.x, terrain.SampleHeight(position), position.z);
    }

    private void CheckAndMoveToNextCheckpoint(Queue<Vector3> waypoints)
    {
        if (waypoints.Count == 0) return;
        var curPos = transform.position;
        var target = waypoints.Peek();
        if (Vector3.Distance(new Vector3(curPos.x, 0, curPos.z), new Vector3(target.x, 0, target.z)) < reachedWaypointDistance)
        {
            Debug.Log("Waypoints reached");
            waypoints.Dequeue();
        }
        else
        {
            transform.position = Vector3.MoveTowards(curPos, target, moveSpeed * Time.deltaTime);
            if (rotatePlayer)
            {
                transform.rotation = Quaternion.LookRotation(new Vector3(target.x, 0, target.z) - new Vector3(curPos.x, 0, curPos.z));
            }
        }
    }

    private Vector3 RandomPositionInTerrainBounds(Terrain terrain)
    {
        var bounds = terrain.terrainData.bounds;
        var position = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
        return new Vector3(position.x, terrain.SampleHeight(position), position.z);
    }
}