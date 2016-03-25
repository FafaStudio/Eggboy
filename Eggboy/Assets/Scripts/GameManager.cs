using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
	
public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
	public float turnDelay = 0.1f;                          //Delay between each Player turn.
	public int playerhpPoints = 6;                    
	public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
	[HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
		
		
	private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
	private int level = 1;                                  //Current level number, expressed in game as "Day 1".
	private List<Enemy> enemies;                          //List of all Enemy units, used to issue them move commands.
	private bool enemiesMoving;                             //Boolean to check if enemies are moving.
	private List<int> levelPassed;
		
		
		
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    
			
		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
			
		//Assign enemies to a new List of Enemy objects.
		enemies = new List<Enemy>();

		//Permet de récupérer les niveaux déjà passé
		levelPassed = new List<int> ();
			
		//Get a component reference to the attached BoardManager script
		boardScript = GameObject.Find("BoardManager").GetComponent<BoardManager>();
			
		//Call the InitGame function to initialize the first level 
		InitGame();
	}

	void Start(){
		levelPassed.Add (Application.loadedLevel);
	}
		
	//This is called each time a scene is loaded.
	void OnLevelWasLoaded(int index)
	{
		levelPassed.Add (Application.loadedLevel);
		level++;
		//Call InitGame to initialize our level.
		InitGame();
	}

	public void checkIfWinLevel(){
		if (enemies.Count <= 0) {
			int nextLevel = chooseNextLevel ();
			launchNextLevel (nextLevel);
		}
	}

	private int chooseNextLevel(){
		int nextLevel = Random.Range (1, SceneManager.sceneCount); 
		for (int i = 0; i < levelPassed.Count; i++) {
			if (levelPassed [i] == nextLevel) {
				chooseNextLevel ();
			} 
		}
		return nextLevel;
	}

	void launchNextLevel(int levelIndex){
		SceneManager.LoadScene (levelIndex);
	}
		
	//Initializes the game for each level.
	void InitGame()
	{
		enemies.Clear();
		boardScript = GameObject.Find("BoardManager").GetComponent<BoardManager>();
		boardScript.SetupScene(level);
	}
		
	void Update()
	{
		//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
		if(playersTurn || enemiesMoving)
			return;

		StartCoroutine (MoveEnemies ());
	}
		
	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}
		
	public void RemoveEnemyToList(Enemy script){
		enemies.Remove (script);
		checkIfWinLevel ();
	}
		
	//GameOver is called when the player reaches 0 food points
	public void GameOver()
	{
		enabled = false;
	}
		
	//Coroutine to move enemies in sequence.
	IEnumerator MoveEnemies()
	{
		//While enemiesMoving is true player is unable to move.
		enemiesMoving = true;
			
		//Wait for turnDelay seconds, defaults to .1 (100 ms).
		yield return new WaitForSeconds(turnDelay);
			
		//Loop through List of Enemy objects.
		for (int i = 0; i < enemies.Count; i++)
		{
			//Call the MoveEnemy function of Enemy at index i in the enemies List.
			enemies[i].MoveEnemy ();
				
			//Wait for Enemy's moveTime before moving next Enemy, 
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		//Once Enemies are done moving, set playersTurn to true so player can move.
		playersTurn = true;
			
		//Enemies are done moving, set enemiesMoving to false.
		enemiesMoving = false;
	}
}
