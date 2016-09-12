using UnityEditor;
using UnityEngine;

public class ScorerWindow : EditorWindow
{
    Achiever  achieverScript;
    Scorer scorerScript;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Scorer")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(ScorerWindow));
    }
    Vector2 scrollPos;
    void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();

        scorerScript = Scorer.instance;
        achieverScript = Achiever.instance;

        if (scorerScript == null || achieverScript == null)
        {
            return;
        }

        if (GUILayout.Button("Add Score"))
        {
            scorerScript.addScore();
        }
        EditorGUILayout.EndHorizontal();
        EditorStyles.textField.wordWrap = true;

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        try
        {
            foreach (var score in scorerScript.scoresScriptableObject.scores)
            {
                EditorGUILayout.BeginHorizontal();
                score.name = EditorGUILayout.TextField("Name :", score.name);
                score.value = EditorGUILayout.FloatField("Value :", score.value);
                for (int i = 0; i < score.achievements.Count; i++)
                {
                    score.achievements[i] = EditorGUILayout.Popup("Achievement " + i + " : ", score.achievements[i], achieverScript.getNames());
                    if (GUILayout.Button("Remove Achievement"))
                    {
                        score.achievements.RemoveAt(i);
                    }
                }
                if (GUILayout.Button("Add Achievement"))
                {
                    score.addAchievement(0);
                }
                if (GUILayout.Button("Remove Score"))
                {
                    scorerScript.scoresScriptableObject.scores.Remove(score);
                }
                EditorGUILayout.EndHorizontal();

            }
        }
        catch (System.NullReferenceException) { }
        catch (System.InvalidOperationException) { }
        EditorGUILayout.EndScrollView();
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(scorerScript.scoresScriptableObject);
        }
        EditorGUILayout.EndVertical();
    }
}