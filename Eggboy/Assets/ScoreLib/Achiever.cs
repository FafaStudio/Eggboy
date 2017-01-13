using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Achievement
{
    [SerializeField]
    public string name;
    [SerializeField]
    public string description;
    [SerializeField]
    public bool valided;
    [SerializeField]
    public float valueNeeded;
    [SerializeField]
    public bool croissant;
    [SerializeField]
    public int position;
    [SerializeField]
    public string tags;


    public Achievement()
    {
        this.name = "";
        this.description = "";
        this.valueNeeded = 0;
        this.valided = false;
        this.croissant = true;
        this.position = -1;
        this.tags = "";
    }

    /*
    public Achievement(string name, string description, float valueNeeded)
    {
        this.name = name;
        this.description = description;
        this.valueNeeded = valueNeeded;
        this.valided = false;
    }*/

    public bool verify(float actualValue)
    {
        if (valided)
        {
            return true;
        }
        if (((this.croissant && actualValue >= valueNeeded) || (!this.croissant && actualValue <= valueNeeded)))
        {
            valid();
            return true;
        }
        return false;
    }

    private void valid()
    {
        this.valided = true;
        AchievementPanel.instance.launchAchievement(this.name);
    }
}



[System.Serializable]
public class Achiever : MonoBehaviour{



    [SerializeField]
    public AchievementsScriptableObject achievementsScriptableObject;
    public List<Achievement> achievements;
    /*
    public void addAchievement(string name, string description, float valueNeeded)
    {
        achievementsScriptableObject.achievements.Add(new Achievement(name, description, valueNeeded));
    }*/

    void Awake()
    {
        achievements = achievementsScriptableObject.achievements;
        if (existBool("Achievement_id_0"))//First start of the game
        {
            for (int i = 0; i < achievements.Count; i++)
            {
                setBool("Achievement_id_" + i, false);
                achievements[i].valided = false;
            }
        }
        else
        {
            for (int i = 0; i < achievements.Count; i++)
            {
                achievements[i].valided = getBool("Achievement_id_" + i);
            }
        }
    }

    private bool existBool(string key)
    {
        return PlayerPrefs.GetInt(key, -1) == -1; 
    }

    private bool getBool(string key)
    {
        return PlayerPrefs.GetInt(key) == 1;
    }

    private void setBool(string key, bool state)
    {
        PlayerPrefs.SetInt(key, state ? 1 : 0);
    }

    public void verifyAchievement(int idAchievement, float value)
    {
        setBool("Achievement_id_" + idAchievement, achievements[idAchievement].verify(value));
    }

    
    public void addAchievement()
    {
        achievementsScriptableObject.achievements.Add(new Achievement());
    }
    public string[] getNames()
    {
        string[] names = new string[achievementsScriptableObject.achievements.Count];
        for (int i = 0; i < achievementsScriptableObject.achievements.Count; i++)
        {
            names[i] = achievementsScriptableObject.achievements[i].name;
        }
        return names;
    }

    private static Achiever s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static Achiever instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(Achiever)) as Achiever;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AManager");
                s_Instance = obj.AddComponent(typeof(Achiever)) as Achiever;
            }

            return s_Instance;
        }
    }

    /*
     * On associe des delegate a l'incrémentation de score
     * Le delegate de la forme delegate(Achievement, value); 
     * Le script de la forme test(Achievement, value) { Achievement.verify(value)}
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
    // Use this for initialization
}
