using UnityEngine;
using System.Collections;

public class Player : MovingObject
{
	public float restartLevelDelay = 1f;
	private bool takesDamageThisLevel = false;

	private Animator animator;

	public float timeBetweenTurn = 0.2f;
	private const float MAX_TIME_BETWEEN_TURN = 0.2f;

	private int hp;
	private int golds;
	private int combo = 1;

	private UIPlayer uiManager;

	private CameraManager camera;

	private string actionDuTour = "nothing"; // move, wait, nothing

	protected override void Start(){
		animator = GetComponent<Animator>();
		manager = GameManager.instance;
		hp = manager.playerhpPoints;
		golds = manager.playerGolds;

		uiManager = GameObject.Find("PlayerPanel").GetComponent<UIPlayer>();
		uiManager.updateLife();
		uiManager.updateGolds ();
		uiManager.updateLevel (manager.levelPassedCount());

		camera = GameObject.Find("Main Camera").GetComponent<CameraManager>();
		caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x, transform.position.y));
		//GameManager.instance.levelPassedToString();

		base.Start();
	}

	protected override bool Move(int xDir, int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2(xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast(start, end, blockingLayer);
		boxCollider.enabled = true;
		if ((hit.transform == null) || (hit.transform.tag == "Bullet")){
			if (piege != null) {
				piege.TriggerExit ();
			}
			animator.SetTrigger("isRunning");
			caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x + xDir, transform.position.y + yDir));
			GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)end.x, (int)end.y, 1, this);
			GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)transform.position.x, (int)transform.position.y, 1,null);
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
			animator.SetTrigger("isDead");
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

	protected override void OnCantMove(GameObject col){
		//caseExacte = new BoardManager.Node (1, new Vector2 (transform.position.x, transform.position.y ));
		if (col.gameObject.tag == "Wall") {
			animator.SetTrigger ("Blase");
			manager.playersTurn = true;
			return;
		} else if (col.gameObject.tag == "Enemy") {
			manager.playersTurn = false;
			Scorer.instance.addScoreValue (7, 1);
			if (col.gameObject.GetComponent<Zombi> () == null) {
				gainGolds (col.GetComponent<Enemy> ().goldsLoot * combo);
			}
			if (GameObject.Find ("LevelDesign").GetComponent<EnumLevel> ().nbTourOpti < manager.totalTurnCurLevel) {
				combo += 1;
				if (combo > 4)
					combo = 4;
			} else {
				combo = 1;
			}
			col.GetComponent<Enemy> ().Die ();
			if (piege != null) {
				if (piege.gameObject.name != "BoutonOn-Off")
					testPiege ();
			}
			return;
		} else if (col.gameObject.tag == "Chest") {
			col.GetComponent<Chest> ().setPlayer (this);
			col.GetComponent<Chest> ().openChest ();
			return;
		} else if (col.gameObject.tag == "Item") {
			col.GetComponent<Item> ().GainLoot (this);
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
		if (CheckIfGameOver())
		{
			this.enabled = false;
			return;
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
            else if (Input.GetKeyDown(KeyCode.Space)){
                manager.replay.addAction(0);
                actionDuTour = "wait";
            }
            else
                actionDuTour = "nothing";
        }
			
		if (actionDuTour != "nothing")
		{
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

	public void resetTakeDamageThisLevel(){
		takesDamageThisLevel = false;
	}

//HP & GOLDS______________________________________________________________________________________________________________

	public void loseHP(){
		if (!takesDamageThisLevel) {
			takesDamageThisLevel = true;
			manager.destroyLifeChests ();
		}
		combo = 1;
		camera.setShake(0.6f);
		animator.SetTrigger("isDamaged");
		manager.playerhpPoints -= 1;
		hp = manager.playerhpPoints;
		uiManager.updateLife();
		//CheckIfGameOver ();
	}

	public void gainHps(int hpToGain){
		manager.playerhpPoints += hpToGain;
		if (manager.playerhpPoints > 6) {
			manager.playerhpPoints = 6;
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

	public void gainGolds(int goldsToGain){
		manager.playerGolds+= goldsToGain;
		StartCoroutine(uiManager.updateGoldsLaunch (goldsToGain, combo, true));
		golds= manager.playerGolds;
		uiManager.updateGolds();
	}

	public int getHp(){
		return hp;
	}

	public int getGolds(){
		return golds;
	}
}
