using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class AchieverWindow : EditorWindow
{
    Achiever achieverScript;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("FafaStudio/ScoreLib/Achiever")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one. 
        EditorWindow.GetWindow(typeof(AchieverWindow));
    }
    Vector2 scrollPos;
    Texture circle;
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        circle = AssetDatabase.LoadAssetAtPath("Assets/ScoreLib/Sprites/Circle.png", typeof(Texture)) as Texture;
        achieverScript = Achiever.instance;
        if (achieverScript == null)
        {
            return;
        }
        GUI.SetNextControlName("OrderAchievementButton");
        if (GUILayout.Button("Order Achievements"))
        {
            GUI.FocusControl("OrderAchievementButton");
            orderAchievements();
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
            int i = 0;
            foreach (var achievement in Achiever.instance.achievementsScriptableObject.achievements)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 40;
                achievement.name = EditorGUILayout.TextField("Name", achievement.name, GUILayout.Width(140), GUILayout.Height(50));
                checkOperationel(achievement);
                GUI.DrawTexture(new Rect(new Vector2(5, i * 52 + 20), new Vector2(15, 15)), circle, ScaleMode.ScaleToFit, true);//52 est parfait je suppose 50+1+1 donc height+margehaute+margebasse
                GUI.color = Color.green;
                GUI.DrawTexture(new Rect(new Vector2(25, i * 52 + 20), new Vector2(15, 15)), circle, ScaleMode.ScaleToFit, true);
                GUI.color = Color.white;
                EditorGUIUtility.labelWidth = 70;
                EditorGUILayout.PrefixLabel("Description");
                achievement.description = EditorGUILayout.TextArea(achievement.description, GUILayout.Height(50), GUILayout.Width(150));
                EditorGUIUtility.labelWidth = 95;
                achievement.valueNeeded = EditorGUILayout.FloatField("Required Value", achievement.valueNeeded);
                EditorGUIUtility.labelWidth = 60;
                achievement.croissant = EditorGUILayout.Toggle("Croissant", achievement.croissant, GUILayout.Width(75));
                if (GUILayout.Button("Remove Achievement"))
                {
                    removeAchievement(achievement);
                }
                EditorGUILayout.EndHorizontal();
                i++;
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

    private void removeAchievement(Achievement achievement)
    {
        int index = Achiever.instance.achievementsScriptableObject.achievements.IndexOf(achievement);
        achieverScript.achievementsScriptableObject.achievements.RemoveAt(index);
        foreach (var score in Scorer.instance.scoresScriptableObject.scores)
        {
            score.achievements.Remove(index);
            for (int i = 0; i < score.achievements.Count; i++)
            {
                if(score.achievements[i] > index)
                {
                    score.achievements[i]--;
                }
            }
        }
    }

    private void orderAchievements()
    {
        this.Focus();
        List<Achievement> orderedAchievements = achieverScript.achievementsScriptableObject.achievements.OrderBy(a => a.name).ToList();
        int[] orderedIndex = new int[orderedAchievements.Count];
        for (int i = 0; i < achieverScript.achievementsScriptableObject.achievements.Count; i++)
        {
            orderedIndex[i] = orderedAchievements.IndexOf(achieverScript.achievementsScriptableObject.achievements[i]);
        }

        foreach (var score in Scorer.instance.scoresScriptableObject.scores)
        {
            for (int i = 0; i < score.achievements.Count; i++)
            {
                score.achievements[i] = orderedIndex[score.achievements[i]];
            }
        }
        achieverScript.achievementsScriptableObject.achievements = orderedAchievements;
    }

    private void checkOperationel(Achievement achievement)
    {
        if (achievement.name == "" || achievement.valueNeeded == 0)
        {
            GUI.color = Color.red;
        }
        else if(achievement.description == "")
        {
            GUI.color = new Color(1, 0.6f, 0);
        }
        else
        {
            GUI.color = Color.green;
        }
    }
}