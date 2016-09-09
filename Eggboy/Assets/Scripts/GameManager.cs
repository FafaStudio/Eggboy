using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
	
public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;                      
	public float turnDelay = 1.5f;                          
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

		//Call the InitGame function to initialize the first level 
		InitGame();
		if (SceneManager.GetActiveScene ().buildIndex == 0) {
			level--;
			levelPassed.Clear ();
			launchNextLevel (1);
		}
	}
		
	//This is called each time a scene is loaded.
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
			yield return new WaitForSeconds(turnDelay/enemies.Count);
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
			if ((traps [i].isEnclenched)||traps[i].isActioning) {
				traps [i].doAction ();
				yield return new WaitForSeconds(turnDelay);
			}
		}
		trapActioning = false;
	}

	public BoardManager getCurrentBoard(){
		return this.boardScript;
	}

	public void checkInstanceToDestroy(){
		for (int i = 0; i < enemies.Count; i++) {
			if (enemies [i].enemyName == "laser") {
				enemies [i].GetComponent<EnemyLaser> ().clearTir ();
			}
		}
	}

}
