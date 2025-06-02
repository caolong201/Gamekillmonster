using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 offset;
    public float smoothSpeed = 0.125f;

    Vector3 desiredPos;
    Vector3 smoothPos;

    private void Start()
    {
    }

    void Update()
    {
        desiredPos = new Vector3(target.position.x, 0, target.position.z) + offset;
        smoothPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = new Vector3(desiredPos.x, desiredPos.y, smoothPos.z);
    }
}