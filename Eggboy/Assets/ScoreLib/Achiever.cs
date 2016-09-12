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
    bool valided;
    [SerializeField]
    public float valueNeeded;

    public Achievement()
    {
        this.name = "";
        this.description = "";
        this.valueNeeded = 0;
        this.valided = false;
    }
    public Achievement(string name, string description, float valueNeeded)
    {
        this.name = name;
        this.description = description;
        this.valueNeeded = valueNeeded;
        this.valided = false;
    }

    public void verify(float actualValue)
    {
        if (!valided && actualValue >= valueNeeded)
        {
            valid();
        }
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

    public void addAchievement(string name, string description, float valueNeeded)
    {
        achievementsScriptableObject.achievements.Add(new Achievement(name, description, valueNeeded));
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
