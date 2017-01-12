using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class InfosLevels : MonoBehaviour {

	public int offsetLevel = 2;
	//permet de recalibrer les levels dans le build avec les indices de level -> level01:passe en indice 1 avec l'offset au lieu de l'indice 3
	// -offset pour passer de la scene du build aux indices de tableaux 
	// +offset pour passer des tableaux aux scenes dans le build

	public int gameSection = 1;
	// 1 : facile et moyen, 2:moyen et difficille, 3:difficille et infame, 4:infame, 5 : win game
	//		60		  40  //	60		40		 //		60			40	 //		100   //   -

	private int statutActualLevel=0;
	//1:niveau normal, 2:magasin, 3:boss
    
    public enum Difficulte {tuto, facile, moyen, difficile, infame, boss};
    public Difficulte[] levelsDifficulty;

	private List<int> levelPassed;
	public int nbOfLevels=0;

	int levelCourant = 0;

	private List<int> levelsFacile;
	private List<int> levelsMoyen;
	private List<int> levelsDifficile;
	private List<int> levelsInfame;

	public Difficulte getLevelDifficulty(int level){
		return levelsDifficulty [level];
	}

	public void initInfosLevels(){
		levelPassed = new List<int> ();
		levelsFacile = new List<int> ();
		levelsMoyen = new List<int> ();
		levelsDifficile= new List<int> ();
		levelsInfame= new List<int> ();
		initLevelsDifficulty ();
	}

	public void initLevelsDifficulty(){
		for (int i = 3; i < levelsDifficulty.Length; i++) {
			switch(levelsDifficulty[i-offsetLevel]){
				case Difficulte.facile:
					levelsFacile.Add (i - offsetLevel);
					break;
				case Difficulte.moyen:
					levelsMoyen.Add (i - offsetLevel);
					break;
				case Difficulte.difficile:
					levelsDifficile.Add (i - offsetLevel);
					break;
				case Difficulte.infame:
					levelsInfame.Add (i - offsetLevel);
					break;
			}
		}
	}

	public int chooseNextLevel(){
		addLevel (Application.loadedLevel-offsetLevel);
		if (PlayerPrefs.HasKey("LevelGameSeed")){
			Random.seed = PlayerPrefs.GetInt("LevelGameSeed");
		}

		updateGameSection ();

		if ((getLevelsCount()== 5)||(getLevelsCount()== 17)){
		//infirmerie
			PlayerPrefs.SetInt ("LevelGameSeed", Random.seed);
			GameManager.instance.totalTurns++;
			return 2;
		}
		if ((getLevelsCount()== 11)||(getLevelsCount()== 24)||(getLevelsCount()== 35)){
		//magasin
			PlayerPrefs.SetInt ("LevelGameSeed", Random.seed);
			GameManager.instance.totalTurns++;
			return 2;
		}
		GameManager.instance.totalTurns++;
		levelCourant=chooseLevelByDifficulty();
		return levelCourant+offsetLevel;
	}

	public int chooseLevelByDifficulty(){
		int nextLevel = 3;
		int randomRange = (int)Random.Range (0f, 100f);
		switch (gameSection) {
			case 1:
				if (randomRange >= 60) 
					nextLevel = chooseNewLevelOnly (nextLevel, levelsMoyen);
				else
					nextLevel = chooseNewLevelOnly (nextLevel, levelsFacile);
				break;
			case 2:
				if (randomRange >= 60) 
					nextLevel = chooseNewLevelOnly (nextLevel, levelsDifficile);
				else
					nextLevel = chooseNewLevelOnly (nextLevel, levelsMoyen);
				break;
			case 3:
				if (randomRange >= 60) 
					nextLevel = chooseNewLevelOnly (nextLevel, levelsInfame);
				else
					nextLevel = chooseNewLevelOnly (nextLevel, levelsDifficile);
				break;
			case 4:
				nextLevel = chooseNewLevelOnly (nextLevel, levelsInfame);
				break;
		}
		return nextLevel;
	}

	public int chooseNewLevelOnly(int nextLevel, List<int> listeConcerne){
		nextLevel = listeConcerne[Random.Range (0, listeConcerne.Count)];
		PlayerPrefs.SetInt("LevelGameSeed", Random.seed);
		/*while (levelPassed.Contains (nextLevel)) {
		 * A RAJOUTER QUAND ON AURA CLASSER SUFFISAMMENT DE LEVELS
			//-2 car le level01 correspond a l'indice 3 dans les scenes du build
			nextLevel = listeConcerne[Random.Range (0, listeConcerne.Count)];
			PlayerPrefs.SetInt("LevelGameSeed", Random.seed);
		}*/
		return nextLevel;
	}

	public void updateGameSection(){
		if (levelPassed.Count == 46)
			gameSection	= 5;
		else if (levelPassed.Count == 36)
			gameSection = 4;
		else if (levelPassed.Count == 24)
			gameSection = 3;
		else if (levelPassed.Count == 12)
			gameSection = 2;
	}

	public void levelPassedToString(){
		for (int i = 0; i < levelPassed.Count; i++) {
			print (levelPassed [i].ToString ());
		}
	}

	public void clearLevels(){
		levelPassed.Clear();
	}

	public void addLevel(int levelNb){
		levelPassed.Add (levelNb);
		nbOfLevels++;
	}

	public int getLevelsCount(){
		return levelPassed.Count;
	}

	public void RemoveLevel(int levelNb){
		levelPassed.Remove (levelNb);
	}
}
