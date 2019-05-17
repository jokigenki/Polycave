using UnityEngine;

public class OVRSelector : MonoBehaviour
{
    public Transform rayOrigin;

    void Start () { }

    void Update ()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        if (OVRInput.Get (OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTrackedRemote))
        {
            Ray ray = new Ray (rayOrigin.position, rayOrigin.forward);
            RaycastHit[] hits = Physics.RaycastAll (ray, 100f);
            foreach (RaycastHit hit in hits)
            {
                SelectionReactor reactor = GetReactorForHit (hit);
                if (reactor != null)
                {
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