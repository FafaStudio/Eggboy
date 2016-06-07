using UnityEngine;
using System.Collections;

public class Frog : Enemy {

	protected bool isChargingAttack;
	protected int turnToAttack = -1;
	protected int maxTurnToAttack = 1;

	private bool isPlayerOnRange = false;
	private GameObject playerDetected = null;

	protected override void Start(){
		base.Start ();
		animator = null;
		isChargingAttack = false;
		turnToAttack = -1;

	}

	protected override bool Move (int xDir, int yDir, out RaycastHit2D hit)
	{
		int[] position = testRangeAttack (xDir, yDir);

		Vector2 start = transform.position;

		Vector2 endTestAttack = start + new Vector2 (position[0], position[1]);
		Vector2 endMovement = start + new Vector2 (xDir, yDir);

		boxCollider.enabled = false;
		//RaycastHit2D hitAttack = Physics2D.Linecast (start, endTestAttack, blockingLayer);
		hit = Physics2D.Linecast (start, endMovement, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			hit = Physics2D.Linecast (start, endTestAttack, blockingLayer);
			if (hit.transform != null) {
				if (hit.transform.gameObject.tag == "Player") {
					return false;
				} else {
					GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)endMovement.x, (int)endMovement.y, -1);
					GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1);
					StartCoroutine (SmoothMovement (endMovement));
					return true;
				}
			}
		} else if (hit.transform.gameObject.tag == "Player") {
			if (turnToAttack == 0) {
				
			}
			return false;
		} else {
			hit = Physics2D.Linecast (start, endTestAttack, blockingLayer);
			if (hit.transform == null) {
				GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)endMovement.x, (int)endMovement.y, -1);
				GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1);
				StartCoroutine (SmoothMovement (endMovement));
				return true;
			} else
				return false;
		}
		return false;

	}
		
	public override void MoveEnemy ()
	{
		if (turnToAttack > 0) {
			print ("je charge");
			turnToAttack--;
			return;
		} 
		base.MoveEnemy ();
	}
	protected override void AttemptMove (int xDir, int yDir)
	{
			RaycastHit2D hit;
			bool canMove = Move(xDir, yDir, out hit);
			if (!canMove) {
				OnCantMove (hit.transform.gameObject);
			}
	}

	protected override void OnCantMove (GameObject col)
	{
		if (col.gameObject.tag == "Wall") {
			return;
		} else if (col.gameObject.tag == "Player"){
			print ("repéré");
			if (isChargingAttack) {
				print ("croc");
				playerDetected.GetComponent<Player> ().loseHP ();
				isChargingAttack = false;
				turnToAttack = -1;
			} else {
				print ("je commence à charger");
				isChargingAttack = true;
				turnToAttack = maxTurnToAttack;
			}
		}
	}
		
	protected int[] testRangeAttack(int xDir, int yDir){
		int[] position = new int[2];
		switch (xDir) {
		case -1:
			position [0] = xDir - 1;
			position [1] = yDir;
			break;
		case 1:
			position [0] = xDir + 1;
			position [1] = yDir;
			break;
		case 0:
			position [0] = xDir;
			switch (yDir){
			case -1:
				position [1] = yDir - 1;
				break;
			case 1:
				position [1] = yDir + 1;
				break;
			case 0:
				position [1] = yDir;
				break;
			}
			break;
		}
		return position;
	}
}
