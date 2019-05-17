using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEvent : GameEvent
{
    public string text;
    public Texture texture;

    public BubbleEvent (string text, Texture texture)
    {
        this.text = text;
        this.texture = texture;
    }
}