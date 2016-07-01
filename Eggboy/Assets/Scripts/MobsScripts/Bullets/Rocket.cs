using UnityEngine;
using System.Collections;

public class Rocket : Enemy {


	protected Vector2 velocity;

	protected override void Start ()
	{
		skipMove = true;
		base.Start ();
	}

	protected override void OnCantMove (GameObject col)
	{
		if (col.gameObject.tag == "Wall") {
			isDead = true;
			Die ();
		} else if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<Player> ().loseHP ();
			isDead = true;
			Die ();
		}
	}

	public override void MoveEnemy ()
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
}
