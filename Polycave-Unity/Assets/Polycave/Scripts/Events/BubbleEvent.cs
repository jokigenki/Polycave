using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEvent : GameEvent
{
    public Texture texture;
    public System.Object userData;

    public BubbleEvent (Texture texture, System.Object userData)
    {
        this.texture = texture;
        this.userData = userData;
    }
}