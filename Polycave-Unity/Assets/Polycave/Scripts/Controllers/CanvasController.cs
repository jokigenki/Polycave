using System;
using System.Collections;
using System.Collections.Generic;
using PolyblotPlayground;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public List<string> introPanelText = new List<string> ();
    private Button _upButton;
    private Button _downButton;
    private Button _backButton;
    private TextMeshProUGUI _title;
    private GameObject _introPanel;
    private Button _introPanelNextButton;
    private TextMeshProUGUI _introPanelText;

    private DataProxySelectionEvent _currentEvent;

    private float _buttonPos;
    private int _introCount = 0;

    void Awake ()
    {
        _upButton = transform.Find ("UpButton").GetComponent<Button> ();
        _downButton = transform.Find ("DownButton").GetComponent<Button> ();
        _backButton = transform.Find ("BackButton").GetComponent<Button> ();
        _title = transform.Find ("Title").GetComponent<TextMeshProUGUI> ();
        _introPanel = transform.Find ("Panel").gameObject;
        _introPanelText = _introPanel.transform.Find ("Text").GetComponent<TextMeshProUGUI> ();
        _introPanelNextButton = _introPanel.transform.Find ("NextButton").GetComponent<Button> ();
        _introPanelNextButton.onClick.AddListener (OnIntroNext);

        _upButton.onClick.AddListener (OnUpClicked);
        _downButton.onClick.AddListener (OnDownClicked);
        _backButton.onClick.AddListener (OnBackClicked);
        _buttonPos = _upButton.transform.localPosition.x;

        _upButton.gameObject.SetActive (false);
        _downButton.gameObject.SetActive (false);
        _backButton.gameObject.SetActive (false);
        _title.gameObject.SetActive (false);
        _introPanel.SetActive (false);

        EventBus.Instance.AddListener<DataProxySelectionEvent> (OnDataProxySelection);
        EventBus.Instance.AddListener<DataProxyEvent> (OnDataProxyEvent);
    }

    private void OnDataProxyEvent (DataProxyEvent e)
    {
        if (e.type == DataProxyEventType.Ready)
        {
            ClearButtons ();
            OnIntroNext ();
        }
        else if (e.type == DataProxyEventType.Reset)
        {
            _introCount = 0;
            ClearButtons ();
            OnIntroNext ();
        }
    }

    private void OnIntroNext ()
    {
        if (_introCount == introPanelText.Count)
        {
            _introPanel.SetActive (false);
            EventBus.Instance.Raise (new DataProxyEvent (DataProxyEventType.Start));
        }
        else
        {
            _introPanel.SetActive (true);
            _introPanelText.text = GetText ();
            _title.gameObject.SetActive (false);
            _introCount++;
        }
    }

    private string GetText ()
    {
        string text = introPanelText[_introCount];
        return text.Replace ("|", "\n");
    }

    private void OnDataProxySelection (DataProxySelectionEvent e)
    {
        _currentEvent = e;

        TextMeshProUGUI upTmp = _upButton.GetComponentInChildren<TextMeshProUGUI> ();
        if (e.compound != null) upTmp.text = "sentences";
        else if (e.kanji != null) upTmp.text = "words";

        TextMeshProUGUI dnTmp = _downButton.GetComponentInChildren<TextMeshProUGUI> ();
        if (e.compound != null) dnTmp.text = "kanji";
        else if (e.sentence != null) dnTmp.text = "words";

        UpdateButtons ();
        _title.gameObject.SetActive (_currentEvent.navType == NavType.Choice);
    }

    private void UpdateButtons ()
    {
        bool upActive = _currentEvent.navType == NavType.Display && _currentEvent.HasUp;
        bool dnActive = _currentEvent.navType == NavType.Display && _currentEvent.HasDown;
        _upButton.gameObject.SetActive (upActive);
        _downButton.gameObject.SetActive (dnActive);
        _backButton.gameObject.SetActive (_currentEvent.navType == NavType.Choice);

        if (upActive && dnActive)
        {
            SetButtonX (_upButton, _buttonPos);
            SetButtonX (_downButton, -_buttonPos);
        }
        else
        {
            SetButtonX (_upButton, 0);
            SetButtonX (_downButton, 0);
        }
    }

    private void ClearButtons ()
    {
        _upButton.gameObject.SetActive (false);
        _downButton.gameObject.SetActive (false);
        _backButton.gameObject.SetActive (false);
    }

    private void SetButtonX (Button btn, float x)
    {
        Vector3 pos = btn.transform.localPosition;
        pos.x = x;
        btn.transform.localPosition = pos;
    }

    private void OnUpClicked ()
    {
        if (_currentEvent.HasUp)
        {
            _currentEvent.navType = NavType.Choice;
            EventBus.Instance.Raise (new DataProxyChoicesEvent (RuneType.Up, _currentEvent));
            UpdateButtons ();
            UpdateTitle (RuneType.Up, _currentEvent);
        }
    }

    private void OnDownClicked ()
    {
        if (_currentEvent.HasDown)
        {
            _currentEvent.navType = NavType.Choice;
            EventBus.Instance.Raise (new DataProxyChoicesEvent (RuneType.Down, _currentEvent));
            UpdateButtons ();
            UpdateTitle (RuneType.Down, _currentEvent);
        }
    }

    private void UpdateTitle (RuneType rType, DataProxySelectionEvent e)
    {
        _title.gameObject.SetActive (_currentEvent.navType == NavType.Choice);

        int kC = e.kanjiChoices?.Count ?? 0;
        int cC = e.compoundChoices?.Count ?? 0;
        int sC = e.sentenceChoices?.Count ?? 0;

        string val = e.kanji != null && cC > 0 && rType == RuneType.Up ? "Pick a word" :
            e.compound != null && sC > 0 && rType == RuneType.Up ? "Pick a sentence" :
            e.compound != null && kC > 0 && rType == RuneType.Down ? "Pick a kanji" :
            e.sentence != null && cC > 0 && rType == RuneType.Down ? "Pick a word" :
            "";

        _title.text = val;
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