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

    public List<Texture> skyBoxes = new List<Texture> ();
    private Texture _currentSkybox;

    private List<GameObject> _currentDisplay = new List<GameObject> ();
    private List<GameObject> _displayPool = new List<GameObject> ();

    private static float d2r = Mathf.PI / 180f;
    public void Start ()
    {
        EventBus.Instance.AddListener<DataProxySelectionEvent> (OnDataProxySelection);
        EventBus.Instance.AddListener<DataProxyChoicesEvent> (OnDataProxyChoices);

        laserPointer.laserBeamBehavior = laserBeamBehavior;

        _currentSkybox = RenderSettings.skybox.mainTexture;
    }

    public void OnDestroy ()
    {
        EventBus.Instance.RemoveListener<DataProxySelectionEvent> (OnDataProxySelection);
        EventBus.Instance.RemoveListener<DataProxyChoicesEvent> (OnDataProxyChoices);
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

    private void DisplayChoices<T> (List<T> choices)
    {
        Debug.Log ($"Displaying {choices.Count} choices");
        ClearDisplay ();
        float yRot = (((float) choices.Count / 2f) - 0.5f) * -previewPanelSpacing;
        List<Texture> usedTextures = new List<Texture> ();
        usedTextures.Add (_currentSkybox);
        foreach (T choice in choices)
        {
            GameObject go = CreateDisplay (yRot);
            Bubble bub = go.GetComponent<Bubble> ();
            bub.DisplayAsBubble (choice, GetTexture (usedTextures), OnBubbleSelected);
            yRot += previewPanelSpacing;
        }
    }

    private Texture GetTexture (List<Texture> usedTextures)
    {
        Texture tex = _currentSkybox;
        while (usedTextures.Contains (tex))
        {
            tex = Randomer.FromList (skyBoxes);
        }
        usedTextures.Add (tex);
        return tex;
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
        EventBus.Instance.Raise (new BubbleEvent (texture, reactor.userData));
        EventBus.Instance.AddListener<NavigationEvent> (OnNavEvent);
    }

    private void OnNavEvent (NavigationEvent e)
    {
        EventBus.Instance.RemoveListener<NavigationEvent> (OnNavEvent);
        dataProxy.SetCurrentData (e.userData);
    }
}