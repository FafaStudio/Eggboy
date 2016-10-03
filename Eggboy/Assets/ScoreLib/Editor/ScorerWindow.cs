using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScorerWindow : EditorWindow
{
    Achiever achieverScript;
    Scorer scorerScript;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("FafaStudio/ScoreLib/Scorer")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(ScorerWindow));
    }
    Vector2 scrollPos;
    Texture circle;
    string searchString = "";
    bool displayTags = false;
    string actualOrder = "Ordered by Id";

    void OnGUI()
    {
        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        GUI.SetNextControlName("OrderScoresButton");
        if (GUILayout.Button(actualOrder, GUI.skin.FindStyle("ToolBarButton")))
        {
            GUI.FocusControl("OrderScoresButton");
            orderScore();
        }
        if (GUILayout.Button("Add Score", GUI.skin.FindStyle("ToolBarButton")))
        {
            scorerScript.addScore();
        }
        if (GUILayout.Button("Reset PlayerPrefs", GUI.skin.FindStyle("ToolBarButton")))
        {
            PlayerPrefs.DeleteAll();
            foreach (var score in scorerScript.scoresScriptableObject.scores)
            {
                if (score.croissant)
                {
                    score.value = 0;
                }
                else
                {
                    score.value = int.MaxValue;
                }
            }
        }
        displayTags = GUILayout.Toggle(displayTags, "Display Tags", "ToolBarButton");
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

        scorerScript = Scorer.instance;
        achieverScript = Achiever.instance;

        if (scorerScript == null || achieverScript == null)
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
            foreach (var score in scorerScript.scoresScriptableObject.scores.Where(a => a.tags.Contains(searchString) || a.name.Contains(searchString)).OrderBy(a => a.position))
            {
                if (score.position == -1)
                {
                    score.position = scorerScript.scoresScriptableObject.scores.IndexOf(score);
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Id " + scorerScript.scoresScriptableObject.scores.IndexOf(score), EditorStyles.boldLabel, GUILayout.Width(60));
                string resultCheckOperationnel;
                checkOperationel(score, out resultCheckOperationnel);
                Rect rectTexture = new Rect(new Vector2(54, i * 20 + 4), new Vector2(15, 15));
                GUI.DrawTexture(rectTexture, circle, ScaleMode.ScaleToFit, true);
                GUI.color = Color.white;
                EditorGUIUtility.labelWidth = 40;
                score.scoreType = (Scorer.ScoreType)EditorGUILayout.EnumPopup("Type", score.scoreType, GUILayout.Width(155));
                AchieverWindow.textureTooltip(rectTexture, resultCheckOperationnel);
                EditorGUIUtility.labelWidth = 40;
                score.name = EditorGUILayout.TextField("Name", score.name);
                EditorGUIUtility.labelWidth = 70;
                EditorGUILayout.PrefixLabel("Description");
                score.description = EditorGUILayout.TextArea(score.description, GUILayout.Width(150));
                EditorGUIUtility.labelWidth = 43;
                score.value = EditorGUILayout.FloatField("Value", score.value);
                if (displayTags)
                {
                    EditorGUIUtility.labelWidth = 35;
                    EditorGUILayout.PrefixLabel("Tags");
                    score.tags = EditorGUILayout.TextArea(score.tags, GUILayout.Width(100));
                }
                score.croissant = EditorGUILayout.Toggle("Croissant", score.croissant, GUILayout.Width(75));
                int achievementFlags = getAchievementFlags(score);
                achievementFlags = EditorGUILayout.MaskField("Achievements", achievementFlags, adapt(achieverScript.getNames()));
                updateListAchievement(score, achievementFlags);
                if (score.scoreType == Scorer.ScoreType.GameScore)
                {
                    int[] namesOriginIndex;
                    string[] names = adapt(scorerScript.getNames(out namesOriginIndex));
                    int relatedScoresFlags = getRelatedScoreFlags(score, namesOriginIndex);
                    relatedScoresFlags = EditorGUILayout.MaskField("Related Score", relatedScoresFlags, names);
                    updateListRelatedScore(score, relatedScoresFlags, namesOriginIndex);
                }

                if (GUILayout.Button("Remove Score"))
                {
                    removeScore(score);
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
            EditorUtility.SetDirty(scorerScript.scoresScriptableObject);
        }
        EditorGUILayout.EndVertical();
    }

    public string[] adapt(string[] origin)
    {
        for (int i = 0; i < origin.Length; i++)
        {
            if (origin[i].Contains("Scores Incompatibles"))
            {
                if (origin[i].Length == 21)
                {
                    origin[i] = "Scores Incompatibles/Unnamed Id/" + i;
                }
                else
                {
                    origin[i] = "Scores Incompatibles/" + origin[i].Substring(21, 1) + "/" + origin[i].Substring(21);
                }
            }
            else
            {
                if (origin[i].Length == 0)
                {
                    origin[i] = "Unnamed Id/" + i;
                }
                else
                {
                    origin[i] = origin[i].Substring(0, 1) + "/" + origin[i];
                }
            }
        }
        return origin;
    }
    public string extract(string modified)
    {
        return modified.Substring(2);
    }

    public int getAchievementFlags(Score score)
    {
        int achievementFlags = -1;
        for (int i = 0; i < achieverScript.achievementsScriptableObject.achievements.Count; i++)
        {
            if (!score.achievements.Contains(i))
            {
                int layer = 1 << i;
                achievementFlags -= layer;
            }
        }
        return achievementFlags;
    }

    public void updateListAchievement(Score score, int achievementFlags)
    {
        for (int i = 0; i < achieverScript.achievementsScriptableObject.achievements.Count; i++)
        {
            int layer = 1 << i;
            if ((achievementFlags & layer) != 0)
            {
                if (!score.achievements.Contains(i))
                {
                    score.achievements.Add(i);
                }
            }
            else
            {
                if (score.achievements.Contains(i))
                {
                    score.achievements.Remove(i);
                }
            }
        }
    }
    public int getRelatedScoreFlags(Score score, int[] originIndex)
    {
        int scoreFlags = -1;
        //for (int i = 0; i < scorerScript.scoresScriptableObject.scores.Count; i++)
        for (int i = 0; i < originIndex.Length; i++)
        {
            if (!score.relatedGlobalScore.Contains(originIndex[i]))
            {
                int layer = 1 << i;
                scoreFlags -= layer;
            }
        }
        return scoreFlags;
    }

    public void updateListRelatedScore(Score score, int scoreFlags, int[] originIndex)
    {
        //for (int i = 0; i < scorerScript.scoresScriptableObject.scores.Count; i++)
        for (int i = 0; i < originIndex.Length; i++)
        {
            int layer = 1 << i;
            if ((scoreFlags & layer) != 0)
            {
                if (!score.relatedGlobalScore.Contains(originIndex[i]))
                {
                    score.relatedGlobalScore.Add(originIndex[i]);
                }
            }
            else
            {
                if (score.relatedGlobalScore.Contains(originIndex[i]))
                {
                    score.relatedGlobalScore.Remove(originIndex[i]);
                }
            }
        }
    }

    private void removeScore(Score targetScore)
    {
        Scorer.instance.scoresScriptableObject.scores.Remove(targetScore);
        int position = targetScore.position;
        foreach (var score in scorerScript.scoresScriptableObject.scores)
        {
            if (score.position > position)
            {
                score.position--;
            }
        }
        /*
        int index = scorerScript.scoresScriptableObject.scores.IndexOf(targetScore);
        scorerScript.scoresScriptableObject.scores.RemoveAt(index);
        foreach (var score in scorerScript.scoresScriptableObject.scores)
        {
            score.relatedGlobalScore.Remove(index);
            for (int i = 0; i < score.relatedGlobalScore.Count; i++)
            {
                if (score.relatedGlobalScore[i] > index)
                {
                    score.relatedGlobalScore[i]--;
                }
            }
        }
        int[] indexs = new int[scorerScript.scoresScriptableObject.scores.Count+1];
        for (int i = 0; i < indexs.Length; i++)
        {
            if (i < index)
            {
                indexs[i] = i;
            }
            else if(i == index)
            {
                indexs[i] = -1;
            }
            else
            {
                indexs[i] = i-1;
            }
        }
        changeScoresIndex(indexs);*/
    }

    private void orderScore()
    {
        List<Score> orderedScores;
        this.Focus();
        switch (actualOrder)
        {
            case ("Ordered by Id"):
                actualOrder = "Ordered by name";
                orderedScores = scorerScript.scoresScriptableObject.scores.OrderBy(a => a.name).ToList();
                break;
            case ("Ordered by name"):
                actualOrder = "Ordered by Id";
                orderedScores = scorerScript.scoresScriptableObject.scores;
                break;
            default:
                actualOrder = "Ordered by Id";
                orderedScores = scorerScript.scoresScriptableObject.scores;
                break;
        }
        for (int i = 0; i < orderedScores.Count; i++)
        {
            orderedScores[i].position = i;
        }
    }

    private void checkOperationel(Score score, out string checkResult)
    {
        if (score.name == "")
        {
            GUI.color = Color.red;
            checkResult = "Name not set";
        }
        else
        {
            GUI.color = Color.green;
            checkResult = "All set";
        }
    }
    private void changeScoresIndex(int[] indexs)
    {
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in assetPaths)
        {
            if (assetPath.EndsWith(".cs") && !assetPath.Contains("ScorerWindow"))
            {
                bool changed = false;
                string[] script = File.ReadAllLines(assetPath);
                for (int i = 0; i < script.Length; i++)
                {
                    if (script[i].Contains("Scorer.instance.addScoreValue("))
                    {
                        int result;
                        if (int.TryParse(script[i][script[i].IndexOf("Scorer.instance.addScoreValue(") + 30].ToString(), out result))
                        {
                            if (indexs[result] == -1)
                            {
                                Debug.LogError("Warning, you removed an used score.\n Line " + i + " FilePath : " + assetPath);
                            } 
                            script[i] = script[i].Replace("Scorer.instance.addScoreValue(" + result, "Scorer.instance.addScoreValue(" + indexs[result]);
                            changed = true;
                        }
                    }
                }
                if (changed)
                {
                    File.WriteAllLines(assetPath, script);
                }
            }
        }
    }

}
