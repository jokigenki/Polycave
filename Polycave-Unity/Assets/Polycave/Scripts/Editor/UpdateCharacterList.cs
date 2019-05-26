using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class UpdateCharacterList : EditorWindow
{
    [SerializeField] SheetsReference[] sheetRefs = { };
    [SerializeField] string destination = "Text";

    private string pythonScriptPath;

    string error;
    string pythonOutput;

    // Add menu named "My Window" to the Window menu
    [MenuItem ("Nekologic/Update Character List")]
    static void Init ()
    {
        // Get existing open window or if none, make a new one:
        UpdateCharacterList window = (UpdateCharacterList) EditorWindow.GetWindow (typeof (UpdateCharacterList));
        window.Show ();
    }

    protected void OnEnable ()
    {
        var data = EditorPrefs.GetString ("UpdateCharacterListPrefs", JsonUtility.ToJson (this, false));
        JsonUtility.FromJsonOverwrite (data, this);
    }

    protected void OnDisable ()
    {
        var data = JsonUtility.ToJson (this, false);
        EditorPrefs.SetString ("UpdateCharacterListPrefs", data);
    }

    void OnGUI ()
    {
        // Example:
        // SheetId = 1sp6kiEZRgVdQj4HDqIUBrJbfUI09-iUVqBCMqxTSsXE
        // Range = Sentences!A1:F

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject (target);
        SerializedProperty sheetIdsProp = so.FindProperty ("sheetRefs");

        EditorGUILayout.PropertyField (sheetIdsProp, true);
        so.ApplyModifiedProperties ();
        destination = EditorGUILayout.TextField ("Destination", destination);

        error = Validate ();

        using (new EditorGUI.DisabledScope (error == "" || pythonOutput == ""))
        {
            string text = error != "" ? error : pythonOutput;
            GUILayout.Label (text, EditorStyles.boldLabel);
        }

        using (new EditorGUI.DisabledScope (error != ""))
        {
            if (GUILayout.Button ("Update"))
            {
                DoUpdateCharacterList (sheetRefs, destination);
            }
        }
    }

    private string Validate ()
    {
        if (!HasPythonScript ())
        {
            return "No Python/character_list_generator.py script in project root";
        }

        if (!HasCredentials ())
        {
            return "Missing gSheets credentials.json file";
        }

        if (!HasClientSecret ())
        {
            return "Missing gSheets client_secret.json file";
        }

        if (!HasSheetId ())
        {
            return "Enter a Google Sheets ID";
        }

        if (string.IsNullOrEmpty (destination))
        {
            return "Enter a destination";
        }

        return "";
    }

    private bool HasSheetId ()
    {
        foreach (SheetsReference sRef in sheetRefs)
        {
            if (!string.IsNullOrEmpty (sRef.id)) return true;
        }
        return false;
    }

    private bool HasCredentials ()
    {
        string url = FileUtilities.Combine (GetPythonFolder (), "credentials.json");
        return File.Exists (url);
    }

    private bool HasClientSecret ()
    {
        string url = FileUtilities.Combine (GetPythonFolder (), "client_secret.json");
        return File.Exists (url);
    }

    private bool HasPythonScript ()
    {
        return File.Exists (GetPythonPath ());
    }

    private string GetPythonFolder ()
    {
        return FileUtilities.Combine (Application.dataPath, "..", "Python/");
    }

    private string GetPythonPath ()
    {
        return FileUtilities.Combine (GetPythonFolder (), "character_list_generator.py");
    }

    private string GetParams ()
    {
        string refs = string.Join (" ", sheetRefs.Select (sRef => sRef.ToString ()).ToArray ());
        return $"{destination} \"{refs}\"";
    }

    public void DoUpdateCharacterList (SheetsReference[] sheetIds, string destination)
    {
        string prams = GetParams ();
        Debug.Log (prams);
        System.Diagnostics.Process p = new System.Diagnostics.Process ();
        p.StartInfo.FileName = "python";
        p.StartInfo.Arguments = $"character_list_generator.py {prams}";
        // Pipe the output to itself - we will catch this later
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.CreateNoWindow = true;

        // Where the script lives
        p.StartInfo.WorkingDirectory = GetPythonFolder ();
        p.StartInfo.UseShellExecute = false;

        Debug.Log ($"python {p.StartInfo.Arguments}");
        p.Start ();
        pythonOutput = p.StandardOutput.ReadToEnd ();
        Debug.Log (pythonOutput);
        p.WaitForExit ();
        p.Close ();
    }
}

[System.Serializable]
public class SheetsReference
{
    public string id;
    public string[] ranges;

    override public string ToString ()
    {
        if (ranges == null || ranges.Length == 0) return id;
        string rangeStr = System.String.Join ("|", ranges);
        return $"{id}|{rangeStr}";
    }
}