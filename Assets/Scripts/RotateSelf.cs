using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Xoay quanh trục Y

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
