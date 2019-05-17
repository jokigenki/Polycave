using System;
using UnityEngine;

public class SelectionReactor : MonoBehaviour
{
    public bool propogateHit = false;
    public Action<SelectionReactor> action;

    public void PerformAction ()
    {
        if (action != null) action (this);
    }
}