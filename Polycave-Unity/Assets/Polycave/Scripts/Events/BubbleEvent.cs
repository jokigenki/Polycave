using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEvent : GameEvent
{
    public int environmentIndex;
    public System.Object userData;

    public BubbleEvent (int environmentIndex, System.Object userData)
    {
        this.environmentIndex = environmentIndex;
        this.userData = userData;
    }
}