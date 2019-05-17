using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float bubbleDistance = 1;

    public Texture[] textures;

    private float d2r = Mathf.PI / 180f;

    // Start is called before the first frame update
    void Start ()
    {
        string[] texts = new string[] { "四", "コーヒー", "貴方", "來る" };

        float rotation = (texts.Length - 1) * -30;
        float height = 1;
        int i = 0;
        foreach (string text in texts)
        {
            CreateBubbleForText (text, textures[i], rotation, height);
            rotation += 60;
            i++;
        }
    }

    public void CreateBubbleForText (string text, Texture texture, float rotation, float height)
    {
        float rads = rotation * d2r;
        Vector3 position = new Vector3 (Mathf.Sin (rads) * bubbleDistance, height, Mathf.Cos (rads) * bubbleDistance);
        GameObject bubbleGO = Instantiate (bubblePrefab, position, Quaternion.identity, transform);
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