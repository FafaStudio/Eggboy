using UnityEngine;
using System.Collections;

public class Rocket : MovingObject {


	protected Vector2 velocity;
	public int playerDamage;                            //The amount of food points to subtract from the player when attacking.

	protected Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
	protected Transform target;                           //Transform to attempt to move toward each turn.
	public bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.
	protected bool isDead = false;
	public BoardManager.Node caseExacte;

	private EnemyRocket enemyLauncher;

	protected override void Start ()
	{
		skipMove = false;
		animator = GetComponent<Animator> ();
		GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, -1);
		base.Start ();
	}

	protected override void OnCantMove (GameObject col)
	{
		if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Bullet") {
			isDead = true;
			Die ();
		} else if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<Player> ().loseHP ();
			isDead = true;
			Die ();
		}
	}

	public void MoveBullet ()
	{
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
		}
	}

	protected override void testPiege(){
	}

	public virtual void Die(){
		isDead = true;
		enabled = false;
		GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1);
		enemyLauncher.roquettestirés.Remove (this);
		Destroy (this.gameObject);
	}

	public void setTireur(EnemyRocket enemy){
		enemyLauncher = enemy;
	}
}
