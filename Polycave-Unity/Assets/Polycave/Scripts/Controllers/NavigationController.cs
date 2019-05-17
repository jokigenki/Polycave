﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public float fadeTime = 0.5f;
    public OVRScreenFade fader;

    private Texture _originalTexture;
    private float _originalFadeTime;

    void Start ()
    {
        EventBus.Instance.AddListener<BubbleEvent> (OnBubbleEvent);
        fader = Camera.main.GetComponent<OVRScreenFade> ();
        _originalTexture = RenderSettings.skybox.mainTexture;
        _originalFadeTime = fader.fadeTime;
    }

    void OnDestroy ()
    {
        RenderSettings.skybox.mainTexture = _originalTexture;
    }

    private void OnBubbleEvent (BubbleEvent e)
    {
        Debug.Log ($"Bubbled {e.text}");
        StartCoroutine (ChangeSkybox (e.texture));
    }

    private IEnumerator ChangeSkybox (Texture texture)
    {
        fader.fadeTime = fadeTime;
        yield return fader.Fade (0, 1);
        RenderSettings.skybox.mainTexture = texture;
        yield return fader.Fade (1, 0);
        fader.fadeTime = _originalFadeTime;
    }
}