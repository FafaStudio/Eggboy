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
	private List<Trap> traps;
	private bool enemiesMoving;                             //Boolean to check if enemies are moving.
	private bool trapActioning;
	private List<int> levelPassed;
	public int totalTurns = 0;
		
		
		
	void Awake()
	{
		if (instance == null)
			instance = this;
		/*else if (instance != this)
			Destroy(gameObject);    */
			
		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
			
		enemies = new List<Enemy>();
		traps = new List<Trap> ();

		//Permet de récupérer les niveaux déjà passé
		levelPassed = new List<int> ();
		print ("bisous");
			
		//Get a component reference to the attached BoardManager script
		boardScript = GameObject.Find("BoardManager").GetComponent<BoardManager>();
			
		//Call the InitGame function to initialize the first level 
		InitGame();
		if (SceneManager.GetActiveScene ().buildIndex == 0) {
			level--;
			levelPassed.Clear ();
			launchNextLevel (1);
		}
	}

	void Start(){
	}
		
	//This is called each time a scene is loaded.
	void OnLevelWasLoaded(int index)
	{
		levelPassed.Add (Application.loadedLevel);
		level++;
		InitGame();
	}

	public void checkIfWinLevel(){
		if (enemies.Count <= 0) {
			int nextLevel = chooseNextLevel ();
			launchNextLevel (nextLevel);
		}
	}

	private int chooseNextLevel(){
		int nextLevel = Random.Range (1, SceneManager.sceneCountInBuildSettings); 
		//int nextLevel = 1;
		/*for (int i = 0; i < levelPassed.Count; i++) {
			if (levelPassed [i] == nextLevel) {
				chooseNextLevel ();
			} 
		}*/
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
		print (levelPassed.Count.ToString ());
		boardScript = GameObject.Find("BoardManager").GetComponent<BoardManager>();
		boardScript.SetupScene(level);
	}
		
	void Update()
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
		//While enemiesMoving is true player is unable to move.
		enemiesMoving = true;
			
		yield return new WaitForSeconds(turnDelay);
			
		//Loop through List of Enemy objects.
		for (int i = 0; i < enemies.Count; i++)
		{
			//Call the MoveEnemy function of Enemy at index i in the enemies List.
			enemies[i].MoveEnemy ();
				
			//Wait for Enemy's moveTime before moving next Enemy, 
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		playersTurn = true;
		totalTurns += 1;
		//Enemies are done moving, set enemiesMoving to false.
		enemiesMoving = false;
	}

	IEnumerator LaunchTraps(){
		trapActioning = true;
		yield return new WaitForSeconds(turnDelay);
		for (int i = 0; i < traps.Count; i++) {
			if (traps [i].isEnclenched) {
				traps [i].doAction ();
				yield return new WaitForSeconds(turnDelay);
			}
		}
		trapActioning = false;
	}
}
