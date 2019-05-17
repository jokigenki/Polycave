using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bubble : MonoBehaviour
{
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

    public void SetTexture (Texture texture)
    {
        _texture = texture;
        SetTextureOnSurface (_inside, texture);
        SetTextureOnSurface (_outside, texture);
    }

    private void SetTextureOnSurface (GameObject target, Texture texture)
    {
        Renderer renderer = target.GetComponent<Renderer> ();
        renderer.material.SetTexture ("_MainTex", texture);
    }
}