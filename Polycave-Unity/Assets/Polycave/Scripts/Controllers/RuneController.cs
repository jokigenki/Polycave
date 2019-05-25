using System;
using System.Collections;
using System.Collections.Generic;
using PolyblotPlayground;
using UnityEngine;
using UnityEngine.UI;

public class RuneController : MonoBehaviour
{
    private Button _upButton;
    private Button _downButton;

    private DataProxySelectionEvent _currentEvent;

    void Awake ()
    {
        _upButton = transform.Find ("UpButton").GetComponent<Button> ();
        _downButton = transform.Find ("DownButton").GetComponent<Button> ();

        EventBus.Instance.AddListener<DataProxySelectionEvent> (OnDataProxySelection);

        _upButton.onClick.AddListener (OnUpClicked);
        _downButton.onClick.AddListener (OnDownClicked);
    }

    private void OnDataProxySelection (DataProxySelectionEvent e)
    {
        _upButton.gameObject.SetActive (e.HasUp);
        _downButton.gameObject.SetActive (e.HasDown);
        _currentEvent = e;
    }

    private void OnUpClicked ()
    {
        if (_currentEvent.HasUp)
            EventBus.Instance.Raise (new DataProxyChoicesEvent (RuneType.Up, _currentEvent));
    }

    private void OnDownClicked ()
    {
        if (_currentEvent.HasDown)
            EventBus.Instance.Raise (new DataProxyChoicesEvent (RuneType.Down, _currentEvent));
    }
}

public enum RuneType
{
    Up,
    Down
}