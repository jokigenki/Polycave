using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelector : MonoBehaviour
{
    public Camera selectionCamera;

    void Start ()
    {
        if (selectionCamera == null) selectionCamera = Camera.main;
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown (0))
        {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll (ray, 100f);
            foreach (RaycastHit hit in hits)
            {
                SelectionReactor reactor = GetReactorForHit (hit);
                if (reactor != null)
                {
                    reactor.Select ();
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