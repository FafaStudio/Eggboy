using UnityEngine;
using System.Collections;

public class Player : MovingObject
{
	public float restartLevelDelay = 1f;
	private bool takesDamageThisLevel = false;
	private bool takesDamageThisTurn = false;

	private Animator animator;

	public float timeBetweenTurn = 0.2f;
	private const float MAX_TIME_BETWEEN_TURN = 0.2f;

	private int hp;
	private int golds;
	private int combo = 1;

	private UIPlayer uiManager;

	private CameraManager camera;

	private string actionDuTour = "nothing"; // move, wait, nothing

	private int variationDirection = 1;


	protected override void Start(){
		animator = GetComponent<Animator>();
		manager = GameManager.instance;
		hp = manager.playerhpPoints;
		golds = manager.playerGolds;

		uiManager = GameObject.Find("PlayerPanel").GetComponent<UIPlayer>();
		uiManager.updateLife();
		uiManager.updateGolds ();
		uiManager.updateLevel (manager.getInfosLevels().getLevelsCount());

		camera = GameObject.Find("Main Camera").GetComponent<CameraManager>();
		caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x, transform.position.y));
		GameManager.instance.getCurrentBoard ().setObjectOnGrid((int)caseExacte.position.x, (int)caseExacte.position.y, 1, this.gameObject);
		//GameManager.instance.levelPassedToString();

		if (GameManager.instance.PlayerHasItem ("MagicianOblivion")) {
			GameManager.instance.setInitialDelay (2);
		}

		base.Start();
	}

	protected override bool Move(int xDir, int yDir){
		Vector2 end = caseExacte.position + new Vector2 (xDir, yDir);
        try
        {
            blockingObject = manager.getCurrentBoard().gridPositions[(int)(caseExacte.position.x + xDir), (int)(caseExacte.position.y + yDir)].nodeObject;
        }
        catch (System.Exception)
        {
            return false;
        }
		if(blockingObject==null){
			if (piege != null) {
				piege.TriggerExit ();
			}
			//animator.SetTrigger("isRunning");
			caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x + xDir, transform.position.y + yDir));
			GameManager.instance.getCurrentBoard ().setObjectOnGrid((int)end.x, (int)end.y, 1, this.gameObject);
			GameManager.instance.getCurrentBoard ().setObjectOnGrid((int)transform.position.x, (int)transform.position.y, 1,null);
			StartCoroutine(SmoothMovement(end));
			return true;
		}
		return false;
	}

	/*private void onDisable(){
		GameManager.instance.playerhpPoints = hp;
	}*/


	private bool CheckIfGameOver(){
		if (hp <= 0)
		{
			if (piege != null) {
				piege.TriggerExit ();
			}
		//	animator.SetTrigger("isDead");
			enabled = false;
			GameManager.instance.GameOver();
			return true;
		}
		else
			return false;
	}

	protected override IEnumerator SmoothMovement(Vector3 end){
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		while (sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
		testPiege ();
	}

	protected override void testPiege(){
		manager.getCurrentBoard ().testCasePiege (this);
		if (!isTrap){
			manager.playersTurn = false;
			setIsUnderTrapEffect(false);
		}
		else{
			setIsUnderTrapEffect(true);
			piege.declencherPiege();
		}
	}

	protected override IEnumerator OnCantMove(){
		manager.playersTurn = true;
		if (blockingObject.tag == "Wall") {
			//animator.SetTrigger ("Blase");
			blockingObject = null;
			yield return null;
		} else if (blockingObject.tag == "Enemy") {
			animator.SetTrigger ("Attack");
			animator.SetInteger ("positionAttack", 0);
			manager.enabled = false;
			new WaitForSeconds(0.05f);

			Scorer.instance.addScoreValue (7, 1);
			if (blockingObject.GetComponent<Zombi> () == null) {
				if (GameManager.instance.PlayerHasItem ("CapitalismSymbol")) {
					gainGolds ((blockingObject.GetComponent<Enemy> ().goldsLoot+3),0);
				} else {
					gainGolds (blockingObject.GetComponent<Enemy> ().goldsLoot,0);
				}
			}
			/*SYSTEME DE COMBO ENLEVER POUR LE MOMENT, multiplier les goldsLoot par la variable combo dans les deux lignes au dessus pour le rajouter
			 * if (GameObject.Find ("LevelDesignShop") == null) {
				if (GameObject.Find ("LevelDesign").GetComponent<EnumLevel> ().nbTourOpti < manager.totalTurnCurLevel) {
					combo += 1;
					if (combo > 4)
						combo = 4;
				} else {
					combo = 1;
				}
			}*/
			if (GameManager.instance.PlayerHasItem ("VampiricLink")) {
				int luck = (int)Random.Range (0f, 19f);
				if (luck == 19)
					gainHps (1);
			}
			manager.enabled = true;
			blockingObject.GetComponent<Enemy>().Die ();
			blockingObject = null;

			if (!GameManager.instance.PlayerHasItem ("Krav-MagaBook")) 
				manager.playersTurn = false;
			if (piege != null) {
				if (piege.gameObject.name != "BoutonOn-Off") {
					if(piege.gameObject.name=="Arrow"||piege.gameObject.name=="Arrow(Clone)")
						manager.playersTurn = true;
					testPiege ();
				}
			}
			yield return null;
		} else if (blockingObject.tag == "Chest") {
			blockingObject.GetComponent<Chest> ().setPlayer (this);
			blockingObject.GetComponent<Chest> ().openChest ();
			blockingObject = null;
			yield return null;
		} else if (blockingObject.tag == "Item") {
			blockingObject.GetComponent<Item> ().GainLoot (this);
			blockingObject = null;
			yield return null;
		}
	}

	void Update()
	{
		if ((!manager.playersTurn)||(underTrapEffect))
		{
			//permet de ne pas se préoccuper de update si ce n'est pas le tour du joueur
			return;
		}

		//permet de tempo le jeu notamment pour l'activation des pièges
		if (timeBetweenTurn > 0f)
		{
			timeBetweenTurn -= Time.deltaTime;
			return;
		}

		if (Input.GetKeyDown (KeyCode.L)) {
			GameManager.instance.killAllsEnemies ();
		}

		if (Input.GetKeyDown (KeyCode.G)) {
			gainGolds(300,0);
		}

		int horizontal = 0;
		int vertical = 0;
        if (manager.replay.debugMod)
        {
            if (manager.replay.actionsParTours.Count == manager.totalTurns){
                return;
            }
            switch (manager.replay.actionsParTours[manager.totalTurns])
            {
                case 0:
                    actionDuTour = "wait";
                    break;
                case 1:
                    horizontal = -1;
                    this.transform.localScale = new Vector2(-1f, 1f);
                    actionDuTour = "move";
                    break;
                case 2:
                    horizontal = 1;
                    this.transform.localScale = new Vector2(1f, 1f);
                    actionDuTour = "move";
                    break;
                case 3:
                    vertical = -1;
                    actionDuTour = "move";
                    break;
                case 4:
                    vertical = 1;
                    actionDuTour = "move";
                    break;
                default:
                    actionDuTour = "nothing";
                    break;
            }
        }
        else
        {
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");

            if (horizontal != 0)
            {
                // pour empecher le mouvement en diagonale
                vertical = 0;
                switch (horizontal)
                {
                    case -1:
                        this.transform.localScale = new Vector2(-1f, 1f);
                        manager.replay.addAction(1);
                        break;
                    case 1:
                        this.transform.localScale = new Vector2(1f, 1f);
                        manager.replay.addAction(2);
                        break;
                }
            }

			vertical *= variationDirection;
			horizontal *= variationDirection;


            if (horizontal != 0 || vertical != 0)
            {
                if(vertical == -1)
                {
                    manager.replay.addAction(3);
                }
                else if (vertical == 1)
                {
                    manager.replay.addAction(4);
                }
                actionDuTour = "move";
            }
            else if (Input.GetButtonDown("Confirm")){
                manager.replay.addAction(0);
                actionDuTour = "wait";
            }
            else
                actionDuTour = "nothing";
        }
			
		if (actionDuTour != "nothing")
		{
			takesDamageThisTurn = false;
			GameManager.instance.checkInstanceToDestroy ();
			if (underTrapNewTurnEffect)
			{
				if (manager.playersTurn)
				{
                    manager.replay.changeAction(0, manager.totalTurns);
					piege.declencherPiegeNewTurn();
				}
				return;
			}
			timeBetweenTurn = MAX_TIME_BETWEEN_TURN;
			if (actionDuTour == "move")
			{
				AttemptMove(horizontal, vertical);
			}
			else if (actionDuTour == "wait")
			{
				if (piege != null) {
					if(piege.gameObject.name!="BoutonOn-Off")
						testPiege ();
				}
				manager.playersTurn = false;
			}
		}
	}

	public void passTurn()
	{
		timeBetweenTurn = MAX_TIME_BETWEEN_TURN;
		manager.playersTurn = false;
	}

	public void varyDirection(){
		variationDirection *=-1;
	}

	public void resetTakeDamageThisLevel(){
		takesDamageThisLevel = false;
	}

//HP & GOLDS______________________________________________________________________________________________________________

	public bool loseHP(){
	// return false if the player don't lose hp
		if (takesDamageThisTurn)
			return false;
		if ((manager.PlayerHasItem ("MagicianTunic")) && (!takesDamageThisLevel)) {
			takesDamageThisLevel = true;
			takesDamageThisTurn = true;
			return false;
		}else if(manager.PlayerHasItem ("MagicianTunic")){
			//si le joueur a l'item mais qu'il a déjà perdu de la vie ce level
			manager.destroyLifeChests ();
			launchLoseHp ();
			return true;
		}
		else {
			//le cas classique ou le joueur n'a pas l'item magician tunic
			if (!takesDamageThisLevel) {
				takesDamageThisLevel = true;
				manager.destroyLifeChests ();
			}
			launchLoseHp ();
			return true;
		}
	}

	public void launchLoseHp(){
		takesDamageThisTurn = true;
		combo = 1;
		camera.setShake (0.6f);
		animator.SetTrigger ("isDamaged");
		manager.playerhpPoints -= 1;
		hp = manager.playerhpPoints;
		uiManager.updateLife ();
		CheckIfGameOver ();
	}

	public void gainHps(int hpToGain){
		manager.playerhpPoints += hpToGain;
		if (manager.playerhpPoints > manager.maxPlayerHpPoints) {
			manager.playerhpPoints = manager.maxPlayerHpPoints;
		}
		hp = manager.playerhpPoints;
		uiManager.updateLife ();
	}

	public bool loseGolds(int goldToLose){
		if (manager.playerGolds - goldToLose < 0) {
			return false;
		} else {
			manager.playerGolds -= goldToLose;
			golds = manager.playerGolds;
			uiManager.updateGolds ();
			StartCoroutine(uiManager.updateGoldsLaunch (goldToLose, 0, false));
			return true;
		}
	}

	public void gainGolds(int goldsToGain, int specialCombo){
		if (GameManager.instance.PlayerHasItem ("CreamerPants")) {
			goldsToGain = (int)(goldsToGain*1.3);
		}
		manager.playerGolds+= goldsToGain;
		if (specialCombo != 1) 
			StartCoroutine (uiManager.updateGoldsLaunch (goldsToGain, combo, true));
		else
			StartCoroutine (uiManager.updateGoldsLaunch (goldsToGain, specialCombo, true));
		golds= manager.playerGolds;
	}

	public int getHp(){
		return hp;
	}

	public int getGolds(){
		return golds;
	}

	public UIPlayer getUIPlayer(){
		return this.uiManager;
	}

}
