using System;
using System.Collections;
using System.Collections.Generic;
using PolyblotPlayground;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    public GameObject bubblePrefab;
    public GameObject textDisplayPrefab;
    public Transform displayParent;

    public float bubbleDistance = 1;

    public Texture[] textures;
    public float characterDistance = 0.15f;
    public Vector3 displayCentre = new Vector3 (0, 1, 1);

    private List<GameObject> _currentDisplay = new List<GameObject> ();

    public void Start ()
    {
        EventBus.Instance.AddListener<DataProxySelectionEvent> (OnDataProxySelection);
        EventBus.Instance.AddListener<DataProxyChoicesEvent> (OnDataProxyChoices);
    }

    public void OnDestroy ()
    {
        EventBus.Instance.RemoveListener<DataProxySelectionEvent> (OnDataProxySelection);
        EventBus.Instance.RemoveListener<DataProxyChoicesEvent> (OnDataProxyChoices);
    }

    private void OnDataProxySelection (DataProxySelectionEvent e)
    {
        if (e.kanji != null) DisplayKanji (e.kanji);
        else if (e.item != null) DisplayLearningItem (e.item);
        else if (e.sentence != null) DisplaySentence (e.sentence);
    }

    private void OnDataProxyChoices (DataProxyChoicesEvent e)
    {
        if (e.currentEvent.kanji != null)
        {
            if (e.runeType == RuneType.Up) DisplayLearningItemChoices (e.currentEvent.itemChoices);
        }
        else if (e.currentEvent.item != null)
        {
            if (e.runeType == RuneType.Up) DisplaySentenceChoices (e.currentEvent.sentenceChoices);
            else if (e.runeType == RuneType.Down) DisplayKanjiChoices (e.currentEvent.kanjiChoices);
        }
        else if (e.currentEvent.sentence != null)
        {
            if (e.runeType == RuneType.Down) DisplayLearningItemChoices (e.currentEvent.itemChoices);
        }
    }

    private void DisplayKanjiChoices (List<string> kanjiChoices)
    {
        // display bubbles for kanji
    }

    private void DisplayLearningItemChoices (List<LearningSetItem> itemChoices)
    {
        // display bubbles for items
    }

    private void DisplaySentenceChoices (List<ExampleSentence> sentenceChoices)
    {
        // display bubbles for sentences
    }

    public void ClearDisplay ()
    {
        // TODO: animate out
        foreach (GameObject go in _currentDisplay)
        {
            Destroy (go);
        }
    }

    public void DisplayLearningItem (LearningSetItem item)
    {
        GameObject displayGo = CreateDisplay ();
        displayGo.GetComponent<TextDisplay> ().DisplayLearningItem (item);
    }

    public void DisplayKanji (string kanji)
    {
        GameObject displayGo = CreateDisplay ();
        displayGo.GetComponent<TextDisplay> ().DisplayKanji (kanji);
    }

    public void DisplaySentence (ExampleSentence sentence)
    {
        GameObject displayGo = CreateDisplay ();
        displayGo.GetComponent<TextDisplay> ().DisplaySentence (sentence);
    }

    private GameObject CreateDisplay ()
    {
        ClearDisplay ();
        GameObject displayGo = Instantiate (textDisplayPrefab, displayCentre, Quaternion.identity, displayParent);
        _currentDisplay.Add (displayGo);
        return displayGo;
    }

    public void CreateBubbleForText (string text, Texture texture, float xOffset)
    {
        Vector3 position = displayCentre;
        position.x += xOffset;
        GameObject bubbleGO = Instantiate (bubblePrefab, position, Quaternion.identity, displayParent);
        bubbleGO.name = $"bubble_{text}";
        Bubble bubble = bubbleGO.GetComponent<Bubble> ();
        bubble.Text = text;
        bubble.SetTexture (texture);
        SelectionReactor reactor = bubbleGO.GetComponent<SelectionReactor> ();
        reactor.action += OnBubbleSelected;
    }

    private void OnBubbleSelected (SelectionReactor reactor)
    {
        Bubble bubble = reactor.GetComponent<Bubble> ();
        string text = bubble.Text;
        Texture texture = bubble.Texture;
        EventBus.Instance.Raise (new BubbleEvent (text, texture));
    }
}