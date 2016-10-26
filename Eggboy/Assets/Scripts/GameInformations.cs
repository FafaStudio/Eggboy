using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInformations : MonoBehaviour {

	//SEED_________________________________________________________________
    public int gameSeed = 0;

	//ITEMS________________________________________________________________
	public GameObject[] passifItems;
	private List<GameObject> pastPassifs;

	void Start () {
		//SEED__________________________________________________________________
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
		//_____________________________________________________________________

		pastPassifs = new List<GameObject>();
    }


//ITEMS PASSIFS________________________________________________________________
	public GameObject choosePassifItem(GameObject itemToTest){
		if (pastPassifs.Count == passifItems.Length) {
			//hp up_________________
			return chooseHPUP();
		}
		while (pastPassifs.Contains (itemToTest)) {
			itemToTest = passifItems [(int)Random.Range (0f, passifItems.Length)];
		}
		addPastPassif (itemToTest);
		return itemToTest;
	}

	void addPastPassif(GameObject toAdd){
		pastPassifs.Add (toAdd);
	}

	public GameObject chooseHPUP(){
		for (int i = 0; i < passifItems.Length; i++) {
			if (passifItems [i].GetComponent<PassifItem> ().itemName == "ExpirationDate")
				return passifItems [i];
		}
		return passifItems [0];
	}


}
