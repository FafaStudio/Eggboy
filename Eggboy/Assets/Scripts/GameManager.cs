using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
	
public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;                      
	public float turnDelay = 0.5f;                          
	public int playerhpPoints = 6; 
	public int playerGolds = 0;
	public static GameManager instance = null;              
	[HideInInspector] public bool playersTurn = true;       
		
	private BoardManager boardScript;                       
	private int level = 1;                                
	public List<Enemy> enemies;                         	//Liste de tous les ennemis
	private List<Trap> traps;								//Liste de tous les pièges
	private List<Rocket> rockets;
	private bool enemiesMoving;                             //Boolean to check if enemies are moving.
	private bool trapActioning;
	private bool rocketsMoving;
	private List<int> levelPassed;
	public int totalTurns = 0;
	public int totalTurnCurLevel = 0;

    public Replay replay;

	//Test de levels Editor
	public bool testingLevel;
	public int levelTest;

	private UIGameMenu uiGame;
		
	void Awake(){
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    
			
		//Permet de ne pas détruire GameManager en changeant de level
		DontDestroyOnLoad(gameObject);
			
		enemies = new List<Enemy>();
		traps = new List<Trap> ();
		rockets = new List<Rocket> ();

		//Permet de récupérer les niveaux déjà passé
		levelPassed = new List<int> ();
			
		uiGame = GameObject.Find ("UI_Menus").GetComponent<UIGameMenu> ();
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
				launchNextLevel (levelTest+1);
			} else {
				launchNextLevel (2);
			}
		}
	}
		
	//Appeler a chaque changement de level
	void OnLevelWasLoaded(int index)
	{
		//print (SceneManager.GetSceneByName ("LevelMagasin").buildIndex.ToString());
		level++;
		InitGame();
	}

	public void restartGame(){
		levelPassed.Clear ();
		playerhpPoints = 6;
		playerGolds = 0;
		playersTurn = true;
		SceneManager.LoadScene ("BeginingLevel");
	}

	public void checkIfWinLevel(){
		if (enemies.Count <= 0) {
			launchNextLevel (chooseNextLevel());
		}
	}

	public void addLevelToList(){
		if (!(SceneManager.GetActiveScene ().buildIndex == SceneManager.GetSceneByName ("LevelMagasin").buildIndex)) {
			levelPassed.Add (SceneManager.GetActiveScene ().buildIndex - 1);
		} else {
			levelPassed.Add (0);
		}
		
	}

	private int chooseNextLevel(){
		if (PlayerPrefs.HasKey("LevelGameSeed")){
			Random.seed = PlayerPrefs.GetInt("LevelGameSeed");
		}

		addLevelToList ();
		
		int nextLevel = Random.Range (3, SceneManager.sceneCountInBuildSettings);
		PlayerPrefs.SetInt("LevelGameSeed", Random.seed);
		//print(Random.seed);
		if ((levelPassed.Count % 10 == 0)&&(levelPassed.Count!=0)){
			PlayerPrefs.SetInt("LevelGameSeed", Random.seed);
			totalTurns++;
			return 1;
		}
		if (levelPassed.Count >= SceneManager.sceneCountInBuildSettings-2) {
			//-2 pour begginingLevel et magasin, a incrémenter si on rajoute
			totalTurns++;
			return nextLevel;
		}
		while (levelPassed.Contains (nextLevel-1)) {
		//-1 car le level01 correspond a l'indice 2 dans les scenes du build
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
		totalTurnCurLevel = 0;
		enemies.Clear();
		traps.Clear ();
		rockets.Clear ();
		boardScript = GameObject.Find("BoardManager").GetComponent<BoardManager>();
		uiGame = GameObject.Find ("UI_Menus").GetComponent<UIGameMenu> ();
		boardScript.SetupScene(level);
		playersTurn = true;
	}
		
	void FixedUpdate()
	{
		if(playersTurn || enemiesMoving || trapActioning || rocketsMoving)
			return;
		playersTurn = false;
		StartCoroutine (LaunchTraps ());
		StartCoroutine (Moverockets ());
		StartCoroutine (MoveEnemies ());
	}
		
	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}

	public void AddTrapToList(Trap script){
		traps.Add (script);
	}

	public void AddRocketToList(Rocket script){
		rockets.Add (script);
	}

	public void RemoveTrapToList(Trap script){
		traps.Remove (script);
	}

	public void RemoveRocketToList(Rocket script){
		rockets.Remove (script);
	}
		
	public void RemoveEnemyToList(Enemy script){
		enemies.Remove (script);
		checkIfWinLevel ();
	}
		
	public void GameOver()
	{
		playersTurn = false;
		levelPassed.Remove (Application.loadedLevel);
		uiGame.launchGameOver ();
		enabled = false;
	}
		
	IEnumerator MoveEnemies(){
		enemiesMoving = true;
		List<Enemy> enemyToLaunch = new List<Enemy>();
		yield return new WaitForSeconds(turnDelay);
		for (int i = 0; i < enemies.Count; i++){
			enemyToLaunch.Add(enemies[i]);
		}
		for (int j = 0; j < enemyToLaunch.Count; j++){
			enemyToLaunch[j].MoveEnemy ();
			yield return new WaitForSeconds (turnDelay / (enemyToLaunch.Count+100));
		}

		for (int j = 0; j < enemyToLaunch.Count; j++){
			while (enemyToLaunch [j].endTurnEnemy != true) {
				if (enemyToLaunch [j].piege != null) {
					if ((enemyToLaunch [j].piege.gameObject.name != "Arrow") && (enemyToLaunch [j].piege.gameObject.name != "Arrow(Clone)"))
						break;
				}
				yield return new WaitForSeconds (turnDelay / (enemyToLaunch.Count+100));
			}
		}
		playersTurn = true;
		totalTurns += 1;
		totalTurnCurLevel += 1;
		enemiesMoving = false;
	}

	IEnumerator Moverockets(){
		rocketsMoving = true;
		List<Rocket> temporaire = new List<Rocket> ();
		yield return new WaitForSeconds(turnDelay);
		for (int i = 0; i < rockets.Count; i++) {
			temporaire.Add(rockets[i]);
		}
		for (int j = 0; j < temporaire.Count; j++) {
			temporaire [j].MoveBullet ();
			yield return new WaitForSeconds(turnDelay/(rockets.Count+1));
		}
		rocketsMoving = false;
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
			yield return new WaitForSeconds(1/traps.Count);
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

	public void levelPassedToString(){
		for (int i = 0; i < levelPassed.Count; i++) {
			print (levelPassed [i].ToString ());
		}
	}

	public int levelPassedCount(){
		return levelPassed.Count;
	}

}
