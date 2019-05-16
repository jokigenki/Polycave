using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

/*
MIT License

Copyright (c) 2018 株式会社Nekologic

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
namespace NekoUtils
{

    public static class DataUtils
    {

#if UNITY_EDITOR
        public static List<string> createdFiles = new List<string> ();

        public static void CleanUp ()
        {
            foreach (string url in createdFiles)
            {
                UnityEditor.FileUtil.DeleteFileOrDirectory (url);
                UnityEditor.FileUtil.DeleteFileOrDirectory (url + ".meta");
            }

            if (createdFiles.Count > 0) UnityEditor.AssetDatabase.Refresh ();
            createdFiles.Clear ();
        }
#endif

        public static T LoadResourceJson<T> (this string path, bool stripWhitespace = false, bool log = true)
        {
            string text = path.ResourceFileToString (stripWhitespace, log);
            if (text != null) return JsonConvert.DeserializeObject<T> (text);
            return default (T);
        }

        public static string ResourceFileToString (this string path, bool stripWhitespace = true, bool log = true)
        {
            TextAsset targetFile = Resources.Load<TextAsset> (path);
            if (targetFile != null)
            {
                string text = targetFile.text;
                if (stripWhitespace) text = Regex.Replace (text, @"\r\n?|\n| ?", "");
                return text;
            }
            else if (log) Debug.Log ("Cannot find " + path + "!");

            return null;
        }

        public static IEnumerator LoadJson<T> (this string path, Action<T> callback, bool stripWhitespace = false, bool log = true)
        {
            yield return path.FileToString ((text) =>
            {
                if (text != null) callback (JsonConvert.DeserializeObject<T> (text));
                else callback (default (T));
            }, stripWhitespace, log);
        }

        public static IEnumerator FileToString (this string path, Action<string> callback, bool stripWhitespace = true, bool log = true)
        {
            UnityWebRequest www = UnityWebRequest.Get (path);
            yield return www.SendWebRequest ();
            if (www.isNetworkError || www.isHttpError)
            {
                if (log) Debug.Log ($"Error loading file from {path}: {www.error}");
            }
            else
                callback (www.downloadHandler.text);
        }

        public static void SaveStringToFile (string path, string data)
        {
            Debug.Log ("Saving to " + path);
#if UNITY_EDITOR
            bool overwrite = File.Exists (path);
#endif
            using (FileStream fs = new FileStream (path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter (fs))
                {
                    writer.Write (data);
                }
            }
#if UNITY_EDITOR
            if (!overwrite) createdFiles.Add (path);
            UnityEditor.AssetDatabase.Refresh ();
#endif
        }

        public static void DeleteFile (string path, string name, string ext)
        {
            string url = path + name + ext;
            if (!File.Exists (url))
            {
                //Debug.Log("Delete, file not found at " + url);
                return;
            }
            //Debug.Log("DELETE file: " + url);
#if UNITY_EDITOR
            UnityEditor.FileUtil.DeleteFileOrDirectory (url);
            UnityEditor.FileUtil.DeleteFileOrDirectory (url + ".meta ");
            UnityEditor.AssetDatabase.Refresh ();
#else
            System.IO.File.Delete (url);
#endif
        }

        public static void CopyFile (string path, string source, string target, string ext, bool overwrite = true)
        {
            string sourceUrl = path + source + ext;
            string targetUrl = path + target + ext;
            if (!File.Exists (sourceUrl))
            {
                Debug.Log ("Copy,file not found at " + sourceUrl);
                return;
            }
            if (File.Exists (targetUrl))
            {
                if (overwrite) DeleteFile (path, target, ext);
                else
                {
                    Debug.Log ("Copy, file already exists at " + targetUrl);
                    return;
                }
            }
            File.Copy (sourceUrl, targetUrl);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh ();
#endif
        }

        public static void CopyFile (string sourceUrl, string targetUrl)
        {
            Texture2D image = Resources.Load (sourceUrl) as Texture2D;
            Debug.Log ("LOADED IMAGE FROM RESOURCES: " + image);
            byte[] bytes = image.EncodeToPNG ();
            File.WriteAllBytes (targetUrl, bytes);
        }

        public static List<T> Deserialize<T> (this List<IDeserializer<T>> list)
        {
            List<T> deserialized = new List<T> ();
            foreach (IDeserializer<T> item in list)
            {
                deserialized.Add (item.Deserialize ());
            }

            return deserialized;
        }
    }
}