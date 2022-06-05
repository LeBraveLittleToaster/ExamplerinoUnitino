using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 125f;

    private float curRot = 0f;

    // Update is called once per frame
    void Update()
    {
        curRot += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0,0,curRot);
    }
}
