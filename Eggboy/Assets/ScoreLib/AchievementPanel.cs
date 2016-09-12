using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AchievementPanel : MonoBehaviour {
    Animator anim;
    public Text achievementNameText;
	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        anim = GetComponent<Animator>();
        achievementNameText = transform.FindChild("AchievementName").GetComponent<Text>();
	}
	
    public void launchAchievement(string achievementName)//working
    {
        anim.SetTrigger("start");
        achievementNameText.text = achievementName;
    }

    private static AchievementPanel s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static AchievementPanel instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(AchievementPanel)) as AchievementPanel;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AManager");
                s_Instance = obj.AddComponent(typeof(AchievementPanel)) as AchievementPanel;
            }

            return s_Instance;
        }
    }

}
