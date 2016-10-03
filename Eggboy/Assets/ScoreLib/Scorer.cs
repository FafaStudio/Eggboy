using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Score
{
    [SerializeField]
    public string name;
    [SerializeField]
    public string description;
    [SerializeField]
    public float value;
    [SerializeField]
    public List<int> achievements;
    [SerializeField]
    public bool croissant;
    [SerializeField]
    public List<int> relatedGlobalScore;
    [SerializeField]
    public Scorer.ScoreType scoreType;
    [SerializeField]
    public int position;
    [SerializeField]
    public string tags;


    public Score()
    {
        this.name = "";
        this.description = "";
        this.value = 0;
        this.achievements = new List<int>();
        this.croissant = true;
        this.relatedGlobalScore = new List<int>();
        scoreType = Scorer.ScoreType.GameScore;
        this.position = -1;
        this.tags = "";
    }

    public float addScore(float value)
    {
        switch (scoreType)
        {
            case Scorer.ScoreType.GameScore:
                if (croissant)
                {
                    this.value += value;
                }
                else
                {
                    this.value -= value;
                }
                break;
            case Scorer.ScoreType.HighScore:
                if ((this.value > value && this.croissant) || (this.value < value && !this.croissant))
                {
                    return this.value;
                }
                this.value = value;
                break;
            case Scorer.ScoreType.CumulativeScore:
                if (croissant)
                {
                    this.value += value;
                }
                else
                {
                    this.value -= value;
                }
                break;
            default:
                break;
        }
        verifyAchievements();
        return this.value;
    }

    public void verifyAchievements()
    {
        foreach (var achievement in this.achievements)
        {
            Achiever.instance.verifyAchievement(achievement, this.value);
        }
    }
    public void addAchievement(int achievement)
    {
        this.achievements.Add(achievement);
    }
    public void removeAchievement(int achievement)
    {
        this.achievements.Remove(achievement);
    }

    public void verifyRelatedScore()
    {
        for (int i = 0; i < relatedGlobalScore.Count; i++)
        {
            Scorer.instance.addScoreValue(relatedGlobalScore[i], this.value);
        }
    }

    public void endGame()
    {
        if(scoreType == Scorer.ScoreType.GameScore)
        {
            verifyRelatedScore();
        }
    }
}



[System.Serializable]
public class Scorer : MonoBehaviour {
    [SerializeField]
    public ScoresScriptableObject scoresScriptableObject;
    //public List<Score> scores;
    public enum ScoreType { GameScore, HighScore, CumulativeScore };


    public void addScore()
    {
        scoresScriptableObject.scores.Add(new Score());
    }

    public void addScoreValue(int idScore, float value)
    {
        PlayerPrefs.SetFloat("Score_id_" + idScore, scoresScriptableObject.scores[idScore].addScore(value));
    }

    public string[] getNames(out int[] originPos)
    {
        List<string> names = new List<string>();
        List<int> listOriginPos = new List<int>();
        for (int i = 0; i < scoresScriptableObject.scores.Count; i++)
        {
            if(scoresScriptableObject.scores[i].scoreType == ScoreType.GameScore)
            {
                //names[i] = "Scores Incompatibles/"+scoresScriptableObject.scores[i].name;
            }
            else
            {
                names.Add(scoresScriptableObject.scores[i].name);
                listOriginPos.Add(i);
                /*
                names[i] = scoresScriptableObject.scores[i].name;
                originPos*/
            }
        }
        originPos = listOriginPos.ToArray();
        return names.ToArray();
    }
    // Use this for initialization
    public List<Score> scores;

    void Awake () {
        scores = scoresScriptableObject.scores;
        if(PlayerPrefs.GetFloat("Score_id_0", -1) == -1)//First start of the game
        {
            for (int i = 0; i < scores.Count; i++)
            {
                PlayerPrefs.SetFloat("Score_id_"+ i, 0);
                scores[i].value = 0;
            }
        }
        else
        {
            for (int i = 0; i < scores.Count; i++)
            {
                if(scores[i].scoreType == ScoreType.GameScore)
                {
                    PlayerPrefs.SetFloat("Score_id_" + i, 0);
                    scores[i].value = 0;
                }
                else
                {
                    scores[i].value = PlayerPrefs.GetFloat("Score_id_" + i);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        foreach (var score in scores)
        {
            score.endGame();
        }
    }

    private static Scorer s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static Scorer instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(Scorer)) as Scorer;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AManager");
                s_Instance = obj.AddComponent(typeof(Scorer)) as Scorer;
            }

            return s_Instance;
        }
    }
}
