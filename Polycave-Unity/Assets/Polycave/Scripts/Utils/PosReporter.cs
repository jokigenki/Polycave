using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosReporter : MonoBehaviour
{
    void Update ()
    {
        Debug.Log ($"{name}: localPos: {transform.localPosition} worldPos: {transform.position} rot: {transform.rotation.eulerAngles}");
    }
}