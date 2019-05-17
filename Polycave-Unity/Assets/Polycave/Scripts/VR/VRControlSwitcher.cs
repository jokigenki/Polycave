using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VRControlSwitcher : MonoBehaviour
{
    public List<GameObject> vrGameObjects = new List<GameObject> ();
    public List<MonoBehaviour> vrControllerScripts = new List<MonoBehaviour> ();
    public List<GameObject> editorGameObjects = new List<GameObject> ();
    public List<MonoBehaviour> editorControllerScripts = new List<MonoBehaviour> ();

    void Awake ()
    {
#if UNITY_EDITOR
        EnableVR (false);
#else
        EnableVR (true);
#endif
    }

    public void EnableVR (bool value)
    {
        vrControllerScripts.ForEach (c => c.enabled = value);
        vrGameObjects.ForEach (c => c.SetActive (value));
        editorControllerScripts.ForEach (c => c.enabled = !value);
        editorGameObjects.ForEach (c => c.SetActive (!value));
    }
}