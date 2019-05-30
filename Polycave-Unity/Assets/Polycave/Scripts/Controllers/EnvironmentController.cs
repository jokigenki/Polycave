using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public int defaultEnvironmentIndex = 1;
    public float fadeTime = 0.25f;
    public AudioSource audioSource;
    private OVRScreenFade _fader;

    private Texture _originalTexture;
    private float _originalFadeTime;
    private bool _isFading;

    public List<Texture> skyBoxes = new List<Texture> ();
    public List<AudioClip> soundscapes = new List<AudioClip> ();
    private int _currentEnviromentIndex;

    void Start ()
    {
        EventBus.Instance.AddListener<BubbleEvent> (OnBubbleEvent);
        EventBus.Instance.AddListener<DataProxyEvent> (OnDataProxyEvent);
        _fader = Camera.main.GetComponent<OVRScreenFade> ();
        _originalFadeTime = _fader.fadeTime;

        ChangeEnvironment (defaultEnvironmentIndex, null);
    }

    private void OnDataProxyEvent (DataProxyEvent e)
    {
        if (e.type == DataProxyEventType.Reset)
        {
            ChangeEnvironment (defaultEnvironmentIndex, null);
        }
    }

    void OnDestroy ()
    {
        RenderSettings.skybox.mainTexture = _originalTexture;
    }

    public int GetEnvironmentIndex (List<int> used)
    {
        used.Add (_currentEnviromentIndex);
        int index = _currentEnviromentIndex;
        while (used.Contains (index))
        {
            index = UnityEngine.Random.Range (0, skyBoxes.Count - 1);
        }
        return index;
    }

    public Texture GetTextureForIndex (int index)
    {
        return skyBoxes[index];
    }

    private void OnBubbleEvent (BubbleEvent e)
    {
        if (_isFading) return;
        ChangeEnvironment (e.environmentIndex, e.userData);
    }

    private void ChangeEnvironment (int index, System.Object userData)
    {
        _currentEnviromentIndex = index;
        Texture tex = skyBoxes[index];
        AudioClip clip = soundscapes[index];
        StartCoroutine (ChangeSkybox (tex, userData));
        StartCoroutine (ChangeSoundscape (clip));
    }

    private IEnumerator ChangeSkybox (Texture texture, System.Object userData)
    {
        _isFading = true;
        _fader.fadeTime = fadeTime;
        yield return _fader.Fade (0, 1);
        RenderSettings.skybox.mainTexture = texture;
        if (userData != null) EventBus.Instance.Raise (new NavigationEvent (NavEventType.SkyboxChanged, userData));
        yield return _fader.Fade (1, 0);
        _fader.fadeTime = _originalFadeTime;
        _isFading = false;
    }

    private IEnumerator ChangeSoundscape (AudioClip clip)
    {
        yield return FadeAudio (0);
        audioSource.clip = clip;
        audioSource.Play ();
        yield return FadeAudio (1);
    }

    private IEnumerator FadeAudio (float target)
    {
        float t = 0;
        float startVolume = audioSource.volume;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float v = t / fadeTime;
            audioSource.volume = Mathf.Lerp (startVolume, target, v);
            yield return null;
        }
        audioSource.volume = target;
    }
}

public class NavigationEvent : GameEvent
{
    public NavEventType type;
    public System.Object userData;
    public NavigationEvent (NavEventType type, System.Object userData)
    {
        this.type = type;
        this.userData = userData;
    }
}

public enum NavEventType
{
    SkyboxChanged
}