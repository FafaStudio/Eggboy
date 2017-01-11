using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
	
public class GameManager : MonoBehaviour{
//CONTIENT TOUT LESSENTIEL AU DEROULEMENT DUNE PARTIE

	public float levelStartDelay = 2f;                      
	public float turnDelay = 0.5f;                          
	public int playerhpPoints; 
	public int maxPlayerHpPoints = 6;
	public int playerGolds = 0;

	//les items du joueur pour la partie en cours
	private List<PassifItem> passifItems;

	public static GameManager instance = null;              
	[HideInInspector] public bool playersTurn = true;       
		
	private BoardManager boardScript;   
	private GameObject levelDesign; 						// contient tous les mobs, les pieges du niveau courant
	private int level = 1;                                
	public List<Enemy> enemies;                         	//Liste de tous les ennemis
	private List<Trap> traps;								//Liste de tous les pièges
	private List<Rocket> rockets;
	public bool enemiesMoving;                             //Boolean to check if enemies are moving.
	public bool trapActioning;
	public bool rocketsMoving;
	private List<int> levelPassed;
	public int totalTurns = 0;
	public int totalTurnCurLevel = 0;

	//pour l'item qui fait que les 3 premiers tours, les ennemis font rien
	private int initialDelay = 0;
	public void setInitialDelay(int delay){initialDelay = delay;}

    public Replay replay;

	//Test de levels Editor
	public bool testingLevel;
	public int levelTest;

	private UIGameMenu uiGame;

	public bool enleverClassementEnnemis;

	public bool isInfoUI=true;
		
	void Awake(){
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    
			
		//Permet de ne pas détruire GameManager en changeant de level
		DontDestroyOnLoad(gameObject);
			
		passifItems = new List<PassifItem> ();
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

		if (SceneManager.GetActiveScene ().buildIndex == 1) {
			level--;
			levelPassed.Clear ();
            if (Application.isEditor && PlayerPrefs.GetInt("level", -1) != -1)
            {
                launchNextLevel(PlayerPrefs.GetInt("level") + 2);
                return;
            }

            if (testingLevel) {
				testingLevel = false;
				launchNextLevel (levelTest+2);
			} else {
				launchNextLevel (3);
			}
		}
	}
		
	//Appeler a chaque changement de level
	void OnLevelWasLoaded(int index)
	{
		level++;
		InitGame();
	}

	public void restartGame(){
		maxPlayerHpPoints = 6;
		levelPassed.Clear ();
		resetPassifItems ();
		playerhpPoints = maxPlayerHpPoints;
		playerGolds = 0;
		playersTurn = true;
		totalTurnCurLevel = 0;
		totalTurns = 0;
		enabled = true;
		SceneManager.LoadScene ("Level01");
	}

	public void checkIfWinLevel(){
		if (enemies.Count <= 0) {
			if((GameManager.instance.PlayerHasItem ("Bento"))&&(levelPassed.Count%2==1)){
				GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ().gainHps (1);
			}
			launchNextLevel (chooseNextLevel());
		}
	}

	public void addLevelToList(){
		if (!(SceneManager.GetActiveScene ().buildIndex == SceneManager.GetSceneByName ("LevelMagasin").buildIndex)) {
			levelPassed.Add (SceneManager.GetActiveScene ().buildIndex - 2);
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
		if (levelPassed.Count < 20) {
			if (((levelPassed.Count % 5) == 0) && (levelPassed.Count != 0)) {
				//if(levelPassed.Count  <3){
				PlayerPrefs.SetInt ("LevelGameSeed", Random.seed);
				totalTurns++;
				return 2;
			}
		} else {
			if (((levelPassed.Count % 10) == 0) && (levelPassed.Count != 0)) {
				PlayerPrefs.SetInt ("LevelGameSeed", Random.seed);
				totalTurns++;
				return 2;
			}
		}
		if (levelPassed.Count >= SceneManager.sceneCountInBuildSettings-3) {
			//-3 pour begginingLevel, magasin et le mainMenu, a incrémenter si on rajoute
			totalTurns++;
			return nextLevel;
		}
		while (levelPassed.Contains (nextLevel-2)) {
		//-2 car le level01 correspond a l'indice 3 dans les scenes du build
			nextLevel = Random.Range (3, SceneManager.sceneCountInBuildSettings);
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
		levelDesign = GameObject.Find ("LevelDesign");
		uiGame = GameObject.Find ("UI_Menus").GetComponent<UIGameMenu> ();
		boardScript.SetupScene(level);
		playersTurn = true;
	}
		
	void FixedUpdate()
	{
		if(playersTurn || enemiesMoving || trapActioning || rocketsMoving )
			return;
		//si le joueur a l'item magician oblivion
		if (initialDelay > 0) {
			initialDelay--;
			playersTurn = true;
			return;
		}
		StartCoroutine (LaunchTraps ());
		StartCoroutine (Moverockets ());
		StartCoroutine (MoveEnemies ());
	}

	void Update(){
		if (Input.GetButton ("UIInGame")) {
			isInfoUI = true;
		} else
			isInfoUI = false;
	}
		
	public void AddEnemyToList(Enemy script){
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
		if(!enleverClassementEnnemis)
			boardScript.launchQuickSort ();
		List<Enemy> enemyToLaunch = new List<Enemy>();
		//yield return new WaitForSeconds(turnDelay);
		for (int i = 0; i < enemies.Count; i++){
			enemyToLaunch.Add(enemies[i]);
		/*	print ("___");
			print(enemyToLaunch [i].gameObject.name.ToString ());
			print (enemyToLaunch [i].caseExacte.distanceVO);*/
		}
		for (int j = 0; j < enemyToLaunch.Count; j++){
			enemyToLaunch[j].MoveEnemy ();
		/*	while (enemyToLaunch [j].endTurnEnemy != true) {
				yield return new WaitForSeconds(0.1f);
			}*/
			yield return new WaitForSeconds (turnDelay / (enemyToLaunch.Count+100));
		}

		for (int j = 0; j < enemyToLaunch.Count; j++){
			while (enemyToLaunch [j].endTurnEnemy != true) {
				
				yield return new WaitForSeconds (turnDelay / (enemyToLaunch.Count+100));
			}
		}
	//	print ("cest finis");
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
		for (int i = 0; i < traps.Count; i++) {
			if ((traps [i].isEnclenched) || traps [i].isActioning) {
				trapToLaunch.Add (traps [i]);
			} 
		}
		for (int j = 0; j < trapToLaunch.Count; j++) {
			trapToLaunch[j].doAction ();
		}
		yield return null;
		trapActioning = false;
	}

	public BoardManager getCurrentBoard(){
		return this.boardScript;
	}

	public void checkInstanceToDestroy(){
	//Lancer juste avant l'action du joueur
		clearLasers();
		clearBombs ();
		resetSpikes ();
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

	public void clearLasers(){
		for (int i = 0; i < enemies.Count; i++) {
			if (enemies [i].enemyName == "laser") {
				enemies [i].GetComponent<EnemyLaser> ().clearTir ();
			}
		}
	}

	public void resetSpikes(){
		for (int i = 0; i < traps.Count; i++) {
			if ((traps [i].name == "Spike") || (traps [i].name == "Spike(Clone)")) {
				if (traps [i].GetComponent<Spike> ().isActioning) {
					traps [i].GetComponent<Spike> ().resetSpike ();
				}
			}
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

	public void AddPassifItem(PassifItem item){
		passifItems.Add (item);
	}

	public bool PlayerHasItem(string name){
		for (int i = 0; i < passifItems.Count; i++) {
			if (passifItems [i].itemName == name) {
				return true;
			}
		}
		return false;
	}

	public void resetPassifItems(){
		foreach (Transform child in this.transform) {
			GameObject.Destroy(child.gameObject);
		}
		passifItems.Clear ();
	}

	public GameInformations getGameInformations(){
		return this.GetComponent<GameInformations> ();
	}

}
