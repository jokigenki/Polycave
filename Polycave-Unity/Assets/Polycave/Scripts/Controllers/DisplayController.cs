using System;
using System.Collections;
using System.Collections.Generic;
using PolyblotPlayground;
using UnityEngine;

public class DisplayController : MonoBehaviour
{
    public GameObject textDisplayPrefab;
    public Transform displayParent;
    public float previewPanelSpacing = 30f;

    public float characterDistance = 0.15f;
    public float displayDistance = 10f;

    public LaserPointer laserPointer;
    public LaserPointer.LaserBeamBehavior laserBeamBehavior;

    public DataProxy dataProxy;

    private List<GameObject> _currentDisplay = new List<GameObject> ();
    private List<GameObject> _displayPool = new List<GameObject> ();

    private EnvironmentController _environmentController;

    private static float d2r = Mathf.PI / 180f;
    public void Start ()
    {
        EventBus.Instance.AddListener<DataProxySelectionEvent> (OnDataProxySelection);
        EventBus.Instance.AddListener<DataProxyChoicesEvent> (OnDataProxyChoices);
        EventBus.Instance.AddListener<DataProxyEvent> (OnDataProxyEvent);

        _environmentController = GetComponent<EnvironmentController> ();
#if !UNITY_EDITOR
        laserPointer.laserBeamBehavior = laserBeamBehavior;
#endif
    }

    public void OnDestroy ()
    {
        EventBus.Instance.RemoveListener<DataProxySelectionEvent> (OnDataProxySelection);
        EventBus.Instance.RemoveListener<DataProxyChoicesEvent> (OnDataProxyChoices);
        EventBus.Instance.RemoveListener<DataProxyEvent> (OnDataProxyEvent);
    }

    private void OnDataProxySelection (DataProxySelectionEvent e)
    {
        if (e.kanji != null) DisplayItem (e.kanji);
        else if (e.compound != null) DisplayItem (e.compound);
        else if (e.sentence != null) DisplayItem (e.sentence);
    }

    private void OnDataProxyChoices (DataProxyChoicesEvent e)
    {
        if (e.currentEvent.kanji != null)
        {
            if (e.runeType == RuneType.Up) DisplayChoices (e.currentEvent.compoundChoices);
        }
        else if (e.currentEvent.compound != null)
        {
            if (e.runeType == RuneType.Up) DisplayChoices (e.currentEvent.sentenceChoices);
            else if (e.runeType == RuneType.Down) DisplayChoices (e.currentEvent.kanjiChoices);
        }
        else if (e.currentEvent.sentence != null)
        {
            if (e.runeType == RuneType.Down) DisplayChoices (e.currentEvent.compoundChoices);
        }
    }

    private void OnDataProxyEvent (DataProxyEvent e)
    {
        if (e.type == DataProxyEventType.Reset)
        {
            ClearDisplay ();
        }
    }

    private void DisplayChoices<T> (List<T> choices)
    {
        ClearDisplay ();
        float yRot = (((float) choices.Count / 2f) - 0.5f) * -previewPanelSpacing;
        List<int> usedEnvironments = new List<int> ();
        foreach (T choice in choices)
        {
            GameObject go = CreateDisplay (yRot);
            Bubble bub = go.GetComponent<Bubble> ();
            int envIndex = _environmentController.GetEnvironmentIndex (usedEnvironments);
            Texture tex = _environmentController.GetTextureForIndex (envIndex);
            bub.DisplayAsBubble (choice, envIndex, tex, OnBubbleSelected);
            yRot += previewPanelSpacing;
        }
    }

    public void ClearDisplay ()
    {
        foreach (GameObject go in _currentDisplay)
        {
            go.SetActive (false);
            _displayPool.Add (go);
        }
        _currentDisplay.Clear ();
    }

    public void DisplayItem<T> (T item)
    {
        ClearDisplay ();
        GameObject go = CreateDisplay ();
        Bubble bub = go.GetComponent<Bubble> ();
        bub.DisplayAsTextDisplay (item);
    }

    private GameObject CreateDisplay (float yRotation = 0)
    {
        float rads = yRotation * d2r;
        GameObject go;
        if (_displayPool.Count == 0)
        {
            go = Instantiate (textDisplayPrefab, displayParent);
        }
        else
        {
            go = _displayPool[0];
            go.SetActive (true);
            _displayPool.RemoveAt (0);
        }
        Vector3 position = new Vector3 (Mathf.Sin (rads) * displayDistance, 0, Mathf.Cos (rads) * displayDistance);
        go.transform.localPosition = position;
        go.transform.localRotation = Quaternion.Euler (new Vector3 (0, yRotation, 0));
        _currentDisplay.Add (go);
        return go;
    }

    private void OnBubbleSelected (SelectionReactor reactor)
    {
        Bubble bubble = reactor.GetComponent<Bubble> ();
        Texture texture = bubble.Texture;
        EventBus.Instance.Raise (new BubbleEvent (bubble.environmentIndex, reactor.userData));
        EventBus.Instance.AddListener<NavigationEvent> (OnNavEvent);
    }

    private void OnNavEvent (NavigationEvent e)
    {
        EventBus.Instance.RemoveListener<NavigationEvent> (OnNavEvent);
        dataProxy.SetCurrentData (e.userData);
    }
}