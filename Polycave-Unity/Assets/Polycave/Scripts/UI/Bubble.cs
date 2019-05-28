using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public AnimationCurve curve = AnimationCurve.EaseInOut (0, 0, 1, 1);
    public float animateInDuration = 0.5f;
    private GameObject _inside;
    private GameObject _outside;
    private SpriteRenderer _bg;
    private Texture _texture;
    private TextDisplay _textDisplay;
    private GameObject _highlight;
    private SelectionReactor _reactor;

    public Texture Texture { get { return _texture; } }

    void Awake ()
    {
        _inside = transform.Find ("SphereInside").gameObject;
        _outside = transform.Find ("SphereOutside").gameObject;
        Transform textDisplay = transform.Find ("TextDisplay");
        _bg = textDisplay.GetComponent<SpriteRenderer> ();
        _textDisplay = textDisplay.GetComponent<TextDisplay> ();
        _highlight = transform.Find ("Highlight").gameObject;
        _reactor = GetComponent<SelectionReactor> ();

        SetHighlight (false);
    }

    public void AnimateIn ()
    {
        StartCoroutine (_AnimateIn ());
    }

    public void SetHighlight (bool value)
    {
        _highlight.SetActive (value);
    }

    private IEnumerator _AnimateIn ()
    {
        float scaleV = 2f;
        Vector3 scaleVec = new Vector3 (scaleV, scaleV, scaleV);
        _inside.transform.localScale = scaleVec;
        _outside.transform.localScale = scaleVec;

        float t = 0;
        SetAlphaOnSurface (_inside, 0);
        SetAlphaOnSurface (_outside, 0);
        float insideTarget = 0.2f;
        float outsideTarget = 0.6f;
        while (t < animateInDuration)
        {
            t += Time.deltaTime;
            float v = curve.Evaluate (t / animateInDuration);
            float insideV = Mathf.Lerp (0, insideTarget, v);
            float outsideV = Mathf.Lerp (0, outsideTarget, v);
            SetAlphaOnSurface (_inside, insideV);
            SetAlphaOnSurface (_outside, outsideV);
            scaleV = Mathf.Lerp (2, 1, v);
            scaleVec.x = scaleV;
            scaleVec.y = scaleV;
            scaleVec.z = scaleV;
            _inside.transform.localScale = scaleVec;
            _outside.transform.localScale = scaleVec;
            yield return null;
        }
    }

    public void DisplayAsBubble (System.Object data, Texture texture, Action<SelectionReactor> selectionAction)
    {
        _textDisplay.DisplayData (data);
        SetTexture (texture);
        _bg.enabled = false;
        _inside.SetActive (true);
        _outside.SetActive (true);
        _reactor.userData = data;
        _reactor.selectionAction = selectionAction;
        SetHighlight (false);
    }

    public void DisplayAsTextDisplay (System.Object data)
    {
        _textDisplay.DisplayData (data);
        _bg.enabled = true;
        _inside.SetActive (false);
        _outside.SetActive (false);
        SetHighlight (false);
    }

    public void SetTexture (Texture texture)
    {
        _texture = texture;
        SetTextureOnSurface (_inside, texture);
        SetTextureOnSurface (_outside, texture);
    }

    private void SetTextureOnSurface (GameObject target, Texture texture)
    {
        _inside.SetActive (texture != null);
        _outside.SetActive (texture != null);
        if (texture == null) return;

        Renderer renderer = target.GetComponent<Renderer> ();
        renderer.material.SetTexture ("_MainTex", texture);
    }

    private void SetAlphaOnSurface (GameObject target, float value)
    {
        Renderer renderer = target.GetComponent<Renderer> ();
        renderer.material.SetFloat ("Transparency", value);
    }
}