using UnityEngine;

public class OVRSelector : MonoBehaviour
{
    public LaserPointer pointer;

    void Start ()
    {
        pointer = FindObjectOfType<LaserPointer> ();
    }

    void Update ()
    {
        if (pointer == null) return;

        if (OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote))
        {
            Ray ray = new Ray (pointer.StartPoint, pointer.Forward);
            RaycastHit[] hits = Physics.RaycastAll (ray, 100f);
            foreach (RaycastHit hit in hits)
            {
                Debug.Log ($"Hit {hit.transform.name}");
                SelectionReactor reactor = GetReactorForHit (hit);
                if (reactor != null)
                {
                    Debug.Log ("Perform action");
                    reactor.PerformAction ();
                    if (!reactor.propogateHit) break;
                }
            }
        }
    }

    private SelectionReactor GetReactorForHit (RaycastHit hit)
    {
        Transform tr = hit.transform;
        while (tr != null)
        {
            SelectionReactor reactor = tr.GetComponent<SelectionReactor> ();
            if (reactor != null && reactor.enabled) return reactor;
            tr = tr.parent;
        }
        return null;
    }
}