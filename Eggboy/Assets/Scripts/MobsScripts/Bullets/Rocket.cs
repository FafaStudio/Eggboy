using UnityEngine;
using System.Collections;

public class Rocket : MovingObject {


	protected Vector2 velocity;
	public int playerDamage;                            //The amount of food points to subtract from the player when attacking.

	protected Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
	protected Transform target;                           //Transform to attempt to move toward each turn.
	public bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.
	protected bool isDead = false;

	private EnemyRocket enemyLauncher;

	protected override void Start (){
		manager = GameManager.instance;
		skipMove = false;
		animator = GetComponent<Animator> ();
		GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1);
		base.Start ();
	}

	protected override void OnCantMove (){
		if (blockingObject.tag == "Wall" || blockingObject.tag == "Bullet") {
			isDead = true;
			blockingObject = null;
			Die ();
		} else if (blockingObject.tag == "Player") {
			blockingObject.GetComponent<Player> ().loseHP ();
			blockingObject = null;
			isDead = true;
			Die ();
		}
	}

	public void MoveBullet (){
		if (skipMove) {
			skipMove = false;
			return;
		}
		if (isDead)
			return;
		AttemptMove ((int)velocity.x, (int)velocity.y);
	}

	public void setVelocity(Vector2 newVelocity){
		velocity = newVelocity;
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<Player> ().loseHP ();
			Die ();
		} else if ((col.gameObject.tag == "Bullet")||(col.gameObject.tag == "Wall")) {
			Die ();
		}
	}

	protected override bool Move(int xDir, int yDir){
		//simule/teste le mouvement du personnage
		Vector2 end = new Vector2(transform.position.x, transform.position.y) + new Vector2 (xDir, yDir);
		if(((this.transform.position.x + xDir)<0)||((this.transform.position.x + xDir)>14)||(this.transform.position.y + yDir<0)||(this.transform.position.y + yDir>7)){
			Die ();
		}
		blockingObject = manager.getCurrentBoard ().gridPositions [(int)(this.transform.position.x + xDir), (int)(this.transform.position.y + yDir)].nodeObject;
		if(blockingObject==null){
			StartCoroutine (SmoothMovement (end));
			return true;
		}
		return false;
	}

	protected override void testPiege(){
	}

	public virtual void Die(){
		isDead = true;
		enabled = false;
		manager.RemoveRocketToList (this);
		Destroy (this.gameObject);
	}

	public void setTireur(EnemyRocket enemy){
		enemyLauncher = enemy;
	}
}
