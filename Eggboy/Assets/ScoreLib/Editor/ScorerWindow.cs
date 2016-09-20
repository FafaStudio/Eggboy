using System.Collections.Generic;
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
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        circle = AssetDatabase.LoadAssetAtPath("Assets/ScoreLib/Sprites/Circle.png", typeof(Texture)) as Texture;

        scorerScript = Scorer.instance;
        achieverScript = Achiever.instance;

        if (scorerScript == null || achieverScript == null)
        {
            return;
        }
        if (GUILayout.Button("Reset PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
            foreach (var score in scorerScript.scoresScriptableObject.scores)
            {
                score.value = 0;
            }
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
            int i = 0;
            foreach (var score in scorerScript.scoresScriptableObject.scores)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Id " + scorerScript.scoresScriptableObject.scores.IndexOf(score), EditorStyles.boldLabel, GUILayout.Width(60));
                checkOperationel(score);
                GUI.DrawTexture(new Rect(new Vector2(54, i * 20 + 4), new Vector2(15, 15)), circle, ScaleMode.ScaleToFit, true);
                GUI.color = Color.white;
                EditorGUIUtility.labelWidth = 40;
                score.scoreType = (Scorer.ScoreType)EditorGUILayout.EnumPopup("Type", score.scoreType, GUILayout.Width(155));
                EditorGUIUtility.labelWidth = 40;
                score.name = EditorGUILayout.TextField("Name", score.name);
                EditorGUIUtility.labelWidth = 43;
                score.value = EditorGUILayout.FloatField("Value", score.value);
                score.croissant = EditorGUILayout.Toggle("Croissant", score.croissant, GUILayout.Width(75));
                int achievementFlags = getAchievementFlags(score);
                achievementFlags = EditorGUILayout.MaskField("Achievements", achievementFlags, adapt(achieverScript.getNames()));
                updateListAchievement(score, achievementFlags);
                if(score.scoreType == Scorer.ScoreType.GameScore)
                {
                    int relatedScoresFlags = getRelatedScoreFlags(score);
                    relatedScoresFlags = EditorGUILayout.MaskField("Related Score", relatedScoresFlags, adapt(scorerScript.getNames()));
                    updateListRelatedScore(score, relatedScoresFlags);
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
                if(origin[i].Length == 21)
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
    public int getRelatedScoreFlags(Score score)
    {
        int scoreFlags = -1;
        for (int i = 0; i < scorerScript.scoresScriptableObject.scores.Count; i++)
        {
            if (!score.relatedGlobalScore.Contains(i))
            {
                int layer = 1 << i;
                scoreFlags -= layer;
            }
        }
        return scoreFlags;
    }

    public void updateListRelatedScore(Score score, int scoreFlags)
    {
        for (int i = 0; i < scorerScript.scoresScriptableObject.scores.Count; i++)
        {
            int layer = 1 << i;
            if ((scoreFlags & layer) != 0)
            {
                if (!score.relatedGlobalScore.Contains(i))
                {
                    score.relatedGlobalScore.Add(i);
                }
            }
            else
            {
                if (score.relatedGlobalScore.Contains(i))
                {
                    score.relatedGlobalScore.Remove(i);
                }
            }
        }
    }

    private void removeScore(Score targetScore)
    {
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
    }

    private void checkOperationel(Score score)
    {
        if (score.name == "")
        {
            GUI.color = Color.red;
        }
        else
        {
            GUI.color = Color.green;
        }
    }
}