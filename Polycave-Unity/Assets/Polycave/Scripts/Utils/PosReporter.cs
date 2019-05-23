using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PosReporter : MonoBehaviour
{
    void Start ()
    {
        string log = $"-----------------------------------------------------------------\n";
        GameObject[] gobs = SceneManager.GetActiveScene ().GetRootGameObjects ();
        foreach (GameObject go in gobs)
        {
            string enabled = go.activeSelf ? "o" : "x";
            log += $"{enabled} {go.transform.name}\n";
            log = DebugChildren (go.transform, log, 1);
        }
        Debug.Log (log);
    }

    private string DebugChildren (Transform transform, string log, int v)
    {
        string indent = new String ('-', v);
        indent = new String ('-', v + 1);
        foreach (Transform tr in transform)
        {
            string enabled = transform.gameObject.activeSelf ? "o" : "x";
            string mesh = tr.GetComponent<MeshRenderer> ()?.name ?? "";
            log += $"{indent}{enabled} {tr.name} {mesh}\n";
            if (tr.childCount > 0) log = DebugChildren (tr, log, v + 1);
        }
        return log;
    }
}