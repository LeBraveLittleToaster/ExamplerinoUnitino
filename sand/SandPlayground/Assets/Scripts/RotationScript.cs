using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 125f;

    void Update()
    {
        var transform1 = transform;
        transform.RotateAround(transform1.position, transform1.forward, rotationSpeed * Time.deltaTime);
    }
}
