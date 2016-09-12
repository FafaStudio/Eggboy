using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Score
{
    [SerializeField]
    public string name;
    [SerializeField]
    public float value;
    [SerializeField]
    public List<int> achievements;

    public Score(string name)
    {
        this.name = name;
        this.value = 0;
        this.achievements = new List<int>();
    }
    public Score()
    {
        this.name = "";
        this.value = 0;
        this.achievements = new List<int>();
    }

    public void addScore(float value)
    {
        this.value += value;
        verifyAchievements();
    }

    private void verifyAchievements()
    {
        foreach (var achievement in this.achievements)
        {
            Achiever.instance.achievementsScriptableObject.achievements[achievement].verify(this.value);
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
}



[System.Serializable]
public class Scorer : MonoBehaviour {
    [SerializeField]
    public ScoresScriptableObject scoresScriptableObject;
    //public List<Score> scores;

    public void addScore()
    {
        scoresScriptableObject.scores.Add(new Score());
    }

    public void addScoreValue(int idScore, float value)
    {
        scoresScriptableObject.scores[idScore].addScore(value);
    }
    // Use this for initialization
    void Awake () {
        addScoreValue(0, 1000);
	}
	
	// Update is called once per frame
	void Update () {
	
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
