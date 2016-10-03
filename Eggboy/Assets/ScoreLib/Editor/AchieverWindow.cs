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
    string searchString = "";
    bool displayTags = false;
    string actualOrder = "Ordered by Id";
    void OnGUI()
    {
        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        GUI.SetNextControlName("OrderAchievementButton");
        if (GUILayout.Button(actualOrder, GUI.skin.FindStyle("ToolBarButton")))
        {
            GUI.FocusControl("OrderAchievementButton");
            orderAchievements();
        }
        if (GUILayout.Button("Add Achievement", GUI.skin.FindStyle("ToolBarButton")))
        {
            achieverScript.addAchievement();
        }
        displayTags = GUILayout.Toggle(displayTags, "Display Tags",  "ToolBarButton");
        GUILayout.FlexibleSpace();
        searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(140));
        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            // Remove focus if cleared
            searchString = "";
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        circle = AssetDatabase.LoadAssetAtPath("Assets/ScoreLib/Sprites/Voyant.png", typeof(Texture)) as Texture;
        achieverScript = Achiever.instance;
        if (achieverScript == null)
        {
            return;
        }
        EditorGUILayout.EndHorizontal();
        EditorStyles.textField.wordWrap = true;

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        try
        {
            int i = 0;
            foreach (var achievement in Achiever.instance.achievementsScriptableObject.achievements.Where(a => a.tags.Contains(searchString) || a.name.Contains(searchString)).OrderBy(a => a.position))
            {
                if(achievement.position == -1)
                {
                    achievement.position = Achiever.instance.achievementsScriptableObject.achievements.IndexOf(achievement);
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 40;
                achievement.name = EditorGUILayout.TextField("Name", achievement.name, GUILayout.Width(140), GUILayout.Height(50));
                string resultCheckOperationnel;
                checkOperationel(achievement, out resultCheckOperationnel);
                Rect rectTexture1 = new Rect(new Vector2(5, i * 52 + 20), new Vector2(15, 15));
                GUI.DrawTexture(rectTexture1, circle, ScaleMode.ScaleToFit, true);//52 est parfait je suppose 50+1+1 donc height+margehaute+margebasse
                string resultCheckUsed;
                checkUsed(i, out resultCheckUsed);
                Rect rectTexture2 = new Rect(new Vector2(25, i * 52 + 20), new Vector2(15, 15));
                GUI.DrawTexture(rectTexture2, circle, ScaleMode.ScaleToFit, true);
                GUI.color = Color.white;
                textureTooltip(rectTexture1, resultCheckOperationnel);
                textureTooltip(rectTexture2, resultCheckUsed);
                /*
                if (rect.Contains(Event.current.mousePosition))
                {
                    Rect test = new Rect(GUILayoutUtility.GetLastRect().x + 40, GUILayoutUtility.GetLastRect().y, GUILayoutUtility.GetLastRect().width - 40, GUILayoutUtility.GetLastRect().height);
                    GUI.Box(test, "");
                    EditorGUI.HelpBox(test, "tooltip", MessageType.Warning);
                }*/
                EditorGUIUtility.labelWidth = 70;
                EditorGUILayout.PrefixLabel("Description");
                achievement.description = EditorGUILayout.TextArea(achievement.description, GUILayout.Height(50), GUILayout.Width(150));
                EditorGUIUtility.labelWidth = 95;
                achievement.valueNeeded = EditorGUILayout.FloatField("Required Value", achievement.valueNeeded);
                if (displayTags)
                {
                    EditorGUIUtility.labelWidth = 35;
                    EditorGUILayout.PrefixLabel("Tags");
                    achievement.tags = EditorGUILayout.TextArea(achievement.tags, GUILayout.Height(50), GUILayout.Width(100));
                }

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
        Achiever.instance.achievementsScriptableObject.achievements.Remove(achievement);
        int position = achievement.position;
        foreach (var achivement in achieverScript.achievementsScriptableObject.achievements)
        {
            if(achivement.position > position)
            {
                achivement.position--;
            }
        }
        /*
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
        */
    }

    private void orderAchievements()
    {
        List<Achievement> orderedAchievements;
        this.Focus();
        switch (actualOrder)
        {
            case ("Ordered by Id"):
                actualOrder = "Ordered by name";
                orderedAchievements = achieverScript.achievementsScriptableObject.achievements.OrderBy(a => a.name).ToList();
                break;
            case ("Ordered by name"):
                actualOrder = "Ordered by Id";
                orderedAchievements = achieverScript.achievementsScriptableObject.achievements;
                break;
            default:
                actualOrder = "Ordered by Id";
                orderedAchievements = achieverScript.achievementsScriptableObject.achievements;
                break;
        }
        for (int i = 0; i < orderedAchievements.Count; i++)
        {
            orderedAchievements[i].position = i;
        }
        //achieverScript.achievementsScriptableObject.achievements = orderedAchievements;
        /*
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
        achieverScript.achievementsScriptableObject.achievements = orderedAchievements;*/
    }

    private void checkOperationel(Achievement achievement, out string checkResult)
    {
        if (achievement.name == "" || achievement.valueNeeded == 0)
        {
            GUI.color = Color.red;
            checkResult = "Name or Value not set";
        }
        else if(achievement.description == "")
        {
            GUI.color = new Color(1, 0.6f, 0);
            checkResult = "Description not set";
        }
        else
        {
            GUI.color = Color.green;
            checkResult = "All set";
        }
    }

    private void checkUsed(int index, out string checkResult)
    {
        foreach (var score in Scorer.instance.scoresScriptableObject.scores)
        {
            if (score.achievements.Contains(index))
            {
                GUI.color = Color.green;
                checkResult = "Used";
                return;
            }
        }
        checkResult = "Not used";
        GUI.color = Color.red;
    }

    public static void textureTooltip(Rect rect, string content)
    {
        if (rect.Contains(Event.current.mousePosition))
        {
            GUIStyle guistyle = new GUIStyle(GUI.skin.GetStyle("Box"));
            guistyle.fontSize = 10;
            guistyle.wordWrap = true;
            Vector2 contentSize = guistyle.CalcSize(new GUIContent(content));
            Rect toolTipRect = new Rect(rect.x + rect.width / 2, rect.y - contentSize.y / 2, contentSize.x, contentSize.y);
            GUI.Box(toolTipRect, content, guistyle);
        }
    }
}