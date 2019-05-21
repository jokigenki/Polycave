﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public float fadeTime = 0.25f;
    private OVRScreenFade _fader;

    private Texture _originalTexture;
    private float _originalFadeTime;
    private bool _isFading;

    void Start ()
    {
        EventBus.Instance.AddListener<BubbleEvent> (OnBubbleEvent);
        _fader = Camera.main.GetComponent<OVRScreenFade> ();
        _originalTexture = RenderSettings.skybox.mainTexture;
        _originalFadeTime = _fader.fadeTime;
    }

    void OnDestroy ()
    {
        RenderSettings.skybox.mainTexture = _originalTexture;
    }

    private void OnBubbleEvent (BubbleEvent e)
    {
        if (_isFading) return;
        StartCoroutine (ChangeSkybox (e.texture));
    }

    private IEnumerator ChangeSkybox (Texture texture)
    {
        _isFading = true;
        _fader.fadeTime = fadeTime;
        yield return _fader.Fade (0, 1);
        RenderSettings.skybox.mainTexture = texture;
        yield return _fader.Fade (1, 0);
        _fader.fadeTime = _originalFadeTime;
        _isFading = false;
    }
}