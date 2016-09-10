using UnityEngine;
using System.Collections;

public class GameInformations : MonoBehaviour {
    public int gameSeed = 0;
	// Use this for initialization
	void Start () {
        //PlayerPrefs.DeleteAll();//ATTENTION CRASH SUR UNITY 5.3.2 Corrigé in 5.3.3 normalement
	    if(gameSeed == 0)
        {
            while(gameSeed < 1000000000)
            {
                gameSeed = new System.Random().Next();//ATTENTION CRASH SUR UNITY 5.3.2 Corrigé in 5.3.3 normalement
            }
        }
        PlayerPrefs.SetInt("GameSeed", gameSeed);
        PlayerPrefs.SetInt("LevelGameSeed", gameSeed);

    }
}
