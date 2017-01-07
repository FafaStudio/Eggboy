using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class TestLevel : MonoBehaviour {

    static TestLevel()
    {
        EditorApplication.update += Update;
    }
    static string originalScene;

    public static void BeginLevel()
    {
        originalScene = EditorSceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt("level", int.Parse(originalScene.Substring(5)));

        SceneManager.LoadScene("BeginingLevel", LoadSceneMode.Single);

        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }


    static bool state = false;

    static void Update()
    {
        if(EditorApplication.isPlaying != state)
        {
            state = EditorApplication.isPlaying;
            if (state)
            {
                if (EditorSceneManager.GetActiveScene().name.IndexOf("Level") == 0)
                {
                    BeginLevel();
                }
            }
            else
            {
                PlayerPrefs.SetInt("level", -1);
            }
        }
    }

}
