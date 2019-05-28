using System;
using System.Collections;
using System.Collections.Generic;
using PolyblotPlayground;
using UnityEngine;

public class DisplayController : MonoBehaviour
{
    public GameObject bubblePrefab;
    public GameObject textDisplayPrefab;
    public GameObject textPreviewPrefab;
    public Transform displayParent;
    public float previewPanelSpacing = 30f;

    public float characterDistance = 0.15f;
    public float displayDistance = 10f;

    public LaserPointer.LaserBeamBehavior laserBeamBehavior;

    private List<GameObject> _currentDisplay = new List<GameObject> ();

    private DataProxy _dataProxy;

    public List<Texture> skyBoxes = new List<Texture> ();
    private Texture _currentSkybox;

    private static float d2r = Mathf.PI / 180f;
    public void Start ()
    {
        EventBus.Instance.AddListener<DataProxySelectionEvent> (OnDataProxySelection);
        EventBus.Instance.AddListener<DataProxyChoicesEvent> (OnDataProxyChoices);

        _dataProxy = FindObjectOfType<DataProxy> ();
        LaserPointer lp = FindObjectOfType<LaserPointer> ();
        if (lp != null) lp.laserBeamBehavior = laserBeamBehavior;

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
        ClearDisplay ();
        float yRot = (((float) choices.Count / 2f) - 0.5f) * -previewPanelSpacing;
        List<Texture> usedTextures = new List<Texture> ();
        usedTextures.Add (_currentSkybox);
        foreach (T choice in choices)
        {
            GameObject go = CreateDisplay (textPreviewPrefab, yRot);
            go.GetComponentInChildren<TextDisplay> ().DisplayData (choice);
            go.GetComponentInChildren<Collider> ().enabled = true;
            go.GetComponent<Bubble> ().SetTexture (GetTexture (usedTextures));
            SelectionReactor reactor = go.GetComponentInChildren<SelectionReactor> ();
            reactor.userData = choice;
            reactor.action += OnBubbleSelected;
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
        // TODO: animate out
        foreach (GameObject go in _currentDisplay)
        {
            Destroy (go);
        }
    }

    public void DisplayItem<T> (T item)
    {
        ClearDisplay ();
        GameObject go = CreateDisplay (textDisplayPrefab);
        go.GetComponent<TextDisplay> ().DisplayData (item);
        go.GetComponent<Collider> ().enabled = false;
    }

    private GameObject CreateDisplay (GameObject prefab, float yRotation = 0)
    {
        float rads = yRotation * d2r;
        GameObject displayGo = Instantiate (prefab, displayParent);
        Vector3 position = new Vector3 (Mathf.Sin (rads) * displayDistance, 0, Mathf.Cos (rads) * displayDistance);
        displayGo.transform.localPosition = position;
        displayGo.transform.localRotation = Quaternion.Euler (new Vector3 (0, yRotation, 0));
        _currentDisplay.Add (displayGo);
        return displayGo;
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
        _dataProxy.SetCurrentData (e.userData);
    }
}