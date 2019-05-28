using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRControllerTracking : MonoBehaviour
{
    public Transform controller;
    public static bool leftHanded { get; private set; }

    void Awake ()
    {
#if UNITY_EDITOR
        leftHanded = false; // (whichever you want to test here)
#else
        leftHanded = OVRInput.GetControllerPositionTracked (OVRInput.Controller.LTrackedRemote);
#endif
    }

    void Update ()
    {
        OVRInput.Controller c = leftHanded ? OVRInput.Controller.LTrackedRemote : OVRInput.Controller.RTrackedRemote;
        if (OVRInput.GetControllerPositionTracked (c))
        {
            controller.localRotation = OVRInput.GetLocalControllerRotation (c);
            controller.localPosition = OVRInput.GetLocalControllerPosition (c);
        }
    }
}