﻿using System;
using System.Collections;
using System.Collections.Generic;
using PolyblotPlayground;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuneController : MonoBehaviour
{
    private Button _upButton;
    private Button _downButton;
    private Button _backButton;

    private DataProxySelectionEvent _currentEvent;

    void Awake ()
    {
        _upButton = transform.Find ("UpButton").GetComponent<Button> ();
        _downButton = transform.Find ("DownButton").GetComponent<Button> ();
        _backButton = transform.Find ("BackButton").GetComponent<Button> ();

        EventBus.Instance.AddListener<DataProxySelectionEvent> (OnDataProxySelection);

        _upButton.onClick.AddListener (OnUpClicked);
        _downButton.onClick.AddListener (OnDownClicked);
        _backButton.onClick.AddListener (OnBackClicked);
    }

    private void OnDataProxySelection (DataProxySelectionEvent e)
    {
        _currentEvent = e;

        _upButton.gameObject.SetActive (e.HasUp);
        TextMeshProUGUI upTmp = _upButton.GetComponentInChildren<TextMeshProUGUI> ();
        if (e.item != null) upTmp.text = "sentence";
        else if (e.kanji != null) upTmp.text = "compound";

        _downButton.gameObject.SetActive (e.HasDown);
        TextMeshProUGUI dnTmp = _downButton.GetComponentInChildren<TextMeshProUGUI> ();
        if (e.item != null) dnTmp.text = "kanji";
        else if (e.sentence != null) dnTmp.text = "compound";
        UpdateButtons ();
    }

    private void UpdateButtons ()
    {
        _upButton.gameObject.SetActive (_currentEvent.navType == NavType.Display);
        _downButton.gameObject.SetActive (_currentEvent.navType == NavType.Display);
        _backButton.gameObject.SetActive (_currentEvent.navType == NavType.Choice);
    }

    private void OnUpClicked ()
    {
        if (_currentEvent.HasUp)
        {
            _currentEvent.navType = NavType.Choice;
            EventBus.Instance.Raise (new DataProxyChoicesEvent (RuneType.Up, _currentEvent));
            UpdateButtons ();
        }
    }

    private void OnDownClicked ()
    {
        if (_currentEvent.HasDown)
        {
            _currentEvent.navType = NavType.Choice;
            EventBus.Instance.Raise (new DataProxyChoicesEvent (RuneType.Down, _currentEvent));
            UpdateButtons ();
        }
    }

    private void OnBackClicked ()
    {
        _currentEvent.navType = NavType.Display;
        EventBus.Instance.Raise (_currentEvent);
        UpdateButtons ();
    }
}

public enum RuneType
{
    Up,
    Down,
    Back
}

public enum NavType
{
    Display,
    Choice
}