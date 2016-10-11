using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject {

	public string enemyName = ""; // necro, laser, frog, blob, rocket, basic

	public int playerDamage;                            //The amount of food points to subtract from the player when attacking.
	
	protected Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
	protected Transform target;                           //Transform to attempt to move toward each turn.
	public bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.
	protected bool isDead = false;

	public bool endTurnEnemy = true;

	public int goldsLoot;

	protected override void Start (){
		manager = GameManager.instance;
	    manager.AddEnemyToList (this);
		animator = GetComponent<Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)transform.position.x, (int)transform.position.y, -1, this);
		caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x, transform.position.y));
		base.Start ();
	}

	public virtual void Die(){
		endTurnEnemy = true;
		if (piege != null) {
			piege.TriggerExit ();
		}
		isDead = true;
		enabled = false;
		GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)transform.position.x, (int)transform.position.y, 1, null);
		caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x, transform.position.y));
		GameManager.instance.RemoveEnemyToList (this);
		Destroy (this.gameObject);
	}

	protected override void AttemptMove (int xDir, int yDir)
	{
		//Check if skipMove is true, if so set it to false and skip this turn.
		if(skipMove)
		{
			skipMove = false;
			endTurnEnemy = true;
			return;
		}
		
		//Call the AttemptMove function from MovingObject.
		base.AttemptMove(xDir, yDir);
		//skipMove = true;
	}

	protected override bool Move(int xDir, int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			launchMove (xDir, yDir, end);
			return true;
		} else if (hit.transform.tag == "Bullet") {
			launchMove (xDir, yDir, end);
			return true;
		}
		return false;
	}

	protected void launchMove(int xDir, int yDir, Vector2 end){
		if (piege != null) {
			piege.TriggerExit ();
		}
		caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x + xDir, transform.position.y + yDir));
		GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)end.x, (int)end.y, -1, this);
		GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)transform.position.x, (int)transform.position.y, 1,null);
		StartCoroutine(SmoothMovement(end));
	}

	protected override IEnumerator SmoothMovement(Vector3 end)
	{
		//coroutine permettant de bouger une unité d'un espace/une case 
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
		testPiege ();
	}

	protected override void testPiege(){
		manager.getCurrentBoard ().testCasePiege (this);
		if (isTrap) {
			if ((piege.gameObject.name != "Arrow") && (piege.gameObject.name != "Arrow(Clone")) {
				setIsUnderTrapEffect (true);
				piege.declencherPiege ();
				endTurnEnemy = true;
			} else {
				setIsUnderTrapEffect (true);
				piege.declencherPiege ();
			}
		} else {
			endTurnEnemy = true;
			setIsUnderTrapEffect(false);
		}
	}

	//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
	public virtual void MoveEnemy (){
		if (isDead)
			return;
		if (underTrapNewTurnEffect) {
			piege.declencherPiegeNewTurn ();
		}

		endTurnEnemy = false;
		
		int xDir = 0;
		int yDir = 0;

		BoardManager.Node currentPos = new BoardManager.Node(1, new Vector2(this.transform.position.x,this.transform.position.y));

		Vector2 nextPosition = GameManager.instance.getCurrentBoard().doPathfinding(target.GetComponent<Player>().caseExacte, currentPos);

		if ((nextPosition.x == -1f) && (nextPosition.y == -1f)) {
			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				yDir = target.position.y > transform.position.y ? 1 : -1;
			else
				xDir = target.position.x > transform.position.x ? 1 : -1;
		
		} else {
			xDir = (int)(nextPosition.x - this.transform.position.x);
			yDir = (int)(nextPosition.y - this.transform.position.y);
		}
		AttemptMove (xDir, yDir);
	}

	protected override void OnCantMove (GameObject col)
	{
		endTurnEnemy = true;
		if (isTrap) {
			isTrap = false;
		}
		if (col.gameObject.tag == "Wall") {
			return;
		} else if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<Player> ().loseHP ();
			animator.SetTrigger ("enemyAttack");
		}
	}
}
