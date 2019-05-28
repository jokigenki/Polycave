using System;
using UnityEngine;

public class SelectionReactor : MonoBehaviour
{
    public bool propogateHit = false;
    public Action<SelectionReactor> overAction;
    public Action<SelectionReactor> outAction;
    public Action<SelectionReactor> selectionAction;
    public System.Object userData;

    private bool _isOver;

    public void Select ()
    {
        if (selectionAction != null) selectionAction (this);
    }

    public void Over ()
    {
        if (_isOver) return;
        _isOver = true;
        if (overAction != null) overAction (this);
    }

    public void Out ()
    {
        if (!_isOver) return;
        _isOver = false;
        if (outAction != null) outAction (this);
    }

}