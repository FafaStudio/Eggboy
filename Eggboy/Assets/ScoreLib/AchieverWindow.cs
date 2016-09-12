using UnityEditor;
using UnityEngine;

public class AchieverWindow : EditorWindow
{
    Achiever achieverScript;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Achiever")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(AchieverWindow));
    }
    Vector2 scrollPos;
    void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();

        //achieverScript = EditorGUILayout.ObjectField("Achiever Script:", achieverScript, typeof(Achiever), true) as Achiever;
        achieverScript = Achiever.instance;
        if (achieverScript == null)
        {
            return;
        }
        if (GUILayout.Button("Add Achievement"))
        {
            achieverScript.addAchievement();
        }
        EditorGUILayout.EndHorizontal();
        EditorStyles.textField.wordWrap = true;

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        try
        {
            foreach (var achievement in Achiever.instance.achievementsScriptableObject.achievements)
            {
                EditorGUILayout.BeginHorizontal();
                achievement.name = EditorGUILayout.TextField("Achievement Name :", achievement.name);
                EditorGUILayout.PrefixLabel("Achievement Description:");
                achievement.description = EditorGUILayout.TextArea(achievement.description, GUILayout.Height(50), GUILayout.Width(150));
                achievement.valueNeeded = EditorGUILayout.FloatField("Achievement Required Value :", achievement.valueNeeded);
                if (GUILayout.Button("Remove Achievement"))
                {
                    Achiever.instance.achievementsScriptableObject.achievements.Remove(achievement);
                }
                EditorGUILayout.EndHorizontal();

            }
        }
        catch (System.NullReferenceException) { }
        catch (System.InvalidOperationException) { }
        EditorGUILayout.EndScrollView();
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(achieverScript.achievementsScriptableObject);
        }
        EditorGUILayout.EndVertical();
    }
}