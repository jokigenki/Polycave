using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera lookAtCamera;

    void Start ()
    {
        if (lookAtCamera == null) lookAtCamera = Camera.main;
    }

    void Update ()
    {
        transform.forward = lookAtCamera.transform.forward;
    }
}