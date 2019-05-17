using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera lookAtCamera;

    private float r2d = 180 / Mathf.PI;
    void Start ()
    {
        if (lookAtCamera == null) lookAtCamera = Camera.main;
        Debug.Log (lookAtCamera.name);
    }

    void Update ()
    {
        transform.forward = lookAtCamera.transform.forward;
    }
}