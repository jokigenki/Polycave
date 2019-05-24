using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public AnimationCurve curve = AnimationCurve.EaseInOut (0, 0, 1, 1);
    public float animateInDuration = 0.5f;
    private TextMeshPro _text;
    private GameObject _inside;
    private GameObject _outside;
    private Texture _texture;

    public Texture Texture { get { return _texture; } }

    void Awake ()
    {
        _text = transform.Find ("Text").GetComponent<TextMeshPro> ();
        _inside = transform.Find ("SphereInside").gameObject;
        _outside = transform.Find ("SphereOutside").gameObject;
    }

    public string Text
    {
        get { return _text.text; }
        set { _text.text = value; }
    }

    public void AnimateIn ()
    {
        StartCoroutine (_AnimateIn ());
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