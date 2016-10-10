﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
	
public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;                      
	public float turnDelay = 1f;                          
	public int playerhpPoints = 6;                    
	public static GameManager instance = null;              
	[HideInInspector] public bool playersTurn = true;       
		
		
	private BoardManager boardScript;                       
	private int level = 1;                                
	public List<Enemy> enemies;                         	//Liste de tous les ennemis
	private List<Trap> traps;								//Liste de tous les pièges
	private bool enemiesMoving;                             //Boolean to check if enemies are moving.
	private bool trapActioning;
	private List<int> levelPassed;
	public int totalTurns = 0;

    public Replay replay;

	//Test de levels Editor
	public bool testingLevel;
	public int levelTest;
		
	void Awake(){
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    
			
		//Permet de ne pas détruire GameManager en changeant de level
		DontDestroyOnLoad(gameObject);
			
		enemies = new List<Enemy>();
		traps = new List<Trap> ();

		//Permet de récupérer les niveaux déjà passé
		levelPassed = new List<int> ();
			
		boardScript = GameObject.Find("BoardManager").GetComponent<BoardManager>();

        replay = gameObject.GetComponent<Replay>();
		playersTurn = true;

		//Initialisation du premier level

		InitGame();

		if (SceneManager.GetActiveScene ().buildIndex == 0) {
			level--;
			levelPassed.Clear ();
			if (testingLevel) {
				testingLevel = false;
				launchNextLevel (levelTest);
			} else {
				launchNextLevel (1);
			}
		}
	}
		
	//Appeler a chaque changement de level
	void OnLevelWasLoaded(int index)
	{
		level++;
		InitGame();
	}

	public void checkIfWinLevel(){
		if (enemies.Count <= 0) {
			launchNextLevel (chooseNextLevel());
		}
	}

	private int chooseNextLevel(){
		if (PlayerPrefs.HasKey("LevelGameSeed"))
		{
			Random.seed = PlayerPrefs.GetInt("LevelGameSeed");
		}
		levelPassed.Add (SceneManager.GetActiveScene().buildIndex);
		int nextLevel = Random.Range (1, SceneManager.sceneCountInBuildSettings);
		PlayerPrefs.SetInt("LevelGameSeed", Random.seed);
		print(Random.seed);
		if (levelPassed.Count >= SceneManager.sceneCountInBuildSettings-1) {
			return nextLevel;
		}
		while (levelPassed.Contains (nextLevel)) {
			nextLevel = Random.Range (1, SceneManager.sceneCountInBuildSettings);
            PlayerPrefs.SetInt("LevelGameSeed", Random.seed);
        }
        totalTurns++;
        return nextLevel;
	}

	void launchNextLevel(int levelIndex){
		playersTurn = true;
		SceneManager.LoadScene (levelIndex);
	}
		
	//Initializes the game for each level.
	void InitGame()
	{
		enemies.Clear();
		traps.Clear ();
		boardScript = GameObject.Find("BoardManager").GetComponent<BoardManager>();
		boardScript.SetupScene(level);
		playersTurn = true;
	}
		
	void FixedUpdate()
	{
		if(playersTurn || enemiesMoving || trapActioning)
			return;
		playersTurn = false;
		StartCoroutine (LaunchTraps ());
		StartCoroutine (MoveEnemies ());
	}
		
	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}

	public void AddTrapToList(Trap script){
		traps.Add (script);
	}

	public void RemoveTrapToList(Trap script){
		traps.Remove (script);
	}
		
	public void RemoveEnemyToList(Enemy script){
		enemies.Remove (script);
		checkIfWinLevel ();
	}
		
	public void GameOver()
	{
		playersTurn = false;
		levelPassed.Remove (Application.loadedLevel);
		enabled = false;
	}
		
	IEnumerator MoveEnemies(){
		enemiesMoving = true;
		List<Enemy> enemyToLaunch = new List<Enemy>();
		yield return new WaitForSeconds(turnDelay);
		for (int i = 0; i < enemies.Count; i++)
		{
			enemyToLaunch.Add(enemies[i]);
		}
		for (int j = 0; j < enemyToLaunch.Count; j++)
		{
			enemyToLaunch[j].MoveEnemy ();
			yield return new WaitForSeconds(turnDelay/(enemies.Count+1));
		}
		playersTurn = true;
		totalTurns += 1;
		enemiesMoving = false;
	}

	IEnumerator LaunchTraps(){
		trapActioning = true;
		List<Trap> trapToLaunch = new List<Trap>();
		yield return new WaitForSeconds(turnDelay);
		for (int i = 0; i < traps.Count; i++) {
			if ((traps [i].isEnclenched) || traps [i].isActioning) {
				trapToLaunch.Add (traps [i]);
			} 
		}
		for (int j = 0; j < trapToLaunch.Count; j++) {
			trapToLaunch[j].doAction ();
			yield return new WaitForSeconds(turnDelay/(traps.Count+1));
		}
		trapActioning = false;
	}

	public BoardManager getCurrentBoard(){
		return this.boardScript;
	}

	public void checkInstanceToDestroy(){
	//Lancer juste avant l'action du joueur
		for (int i = 0; i < enemies.Count; i++) {
			if (enemies [i].enemyName == "laser") {
				enemies [i].GetComponent<EnemyLaser> ().clearTir ();
			}
		}
		clearBombs ();
	}

	public void clearBombs(){
		List<GameObject> bombesToDestroy = new List<GameObject>();
		for (int j = 0; j < traps.Count; j++) {
			if ((traps [j].name == "Bombe") || (traps [j].name == "Bombe(Clone)")) {
				if (traps [j].GetComponent<Bomb> ().isExplosing ()) {
					bombesToDestroy.Add (traps [j].gameObject);
				}
			}
		}
		for (int i = 0; i < bombesToDestroy.Count; i++) {
			bombesToDestroy [i].GetComponent<Bomb> ().resetAfterExplosion ();
		}
	}

	public void destroyLifeChests(){
		for (int i = 0; i < traps.Count; i++) {
			if (traps [i].tag == "Chest") {
				if (traps [i].GetComponent<Chest> ().getChestName() == "Life") {
					traps [i].GetComponent<Chest> ().destroyChest ();
				}
			}
		}
	}

	public List<Trap> getTrapsList(){
		return traps;
	}

}
