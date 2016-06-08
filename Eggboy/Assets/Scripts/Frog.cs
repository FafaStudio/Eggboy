using UnityEngine;
using System.Collections;

public class Frog : Enemy {

	protected int turnToAttack = -1; 
	// 0 : je charge, -1 : repos
	protected int maxTurnToAttack = 0;

	protected int xDirAttack = 0;
	protected int yDirAttack = 0;

	protected bool isAttackingThisTurn = false;

	protected override void Start(){
		base.Start ();
		animator = null;
		turnToAttack = -1;
	}

	protected override bool Move (int xDir, int yDir, out RaycastHit2D hit){
		int[] position = testRangeAttack (xDir, yDir);

		Vector2 start = transform.position;

		Vector2 endTestAttack = start + new Vector2 (position[0], position[1]);
		Vector2 endMovement = start + new Vector2 (xDir, yDir);

		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, endMovement, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
		// si il n'y a personne au case voisine
			
			boxCollider.enabled = false;
			hit = Physics2D.Linecast (start, endTestAttack, blockingLayer);
			boxCollider.enabled = true;

			if ((hit.transform != null) && (hit.transform.gameObject.tag == "Player")) {
			// si le joueur est sur une case plus loin 
				xDirAttack = xDir;
				yDirAttack = yDir;
				return false;
			} else {
			//sinon si le joueur n'est pas sur une case plus loin
				if (isAttackingThisTurn) {
				// si la grenouille attaque ce tour on lance une attaque nulle pour juste faire l'animation et pas de déplacement
					StartCoroutine (frogAttack (null));
					return true;
				} else {
				// si elle n'est pas en train d'attaquer et qu'il n'y a aucun obstacle ni joueur, elle bouge 
					GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)endMovement.x, (int)endMovement.y, -1);
					GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1);
					StartCoroutine (SmoothMovement (endMovement));
					return true;
				}
			}
		} else if ((hit.transform.gameObject.tag == "Player")) {
		// si le joueur est juste a côté, on se prépare a attaquer
			xDirAttack = (int)endMovement.x;
			yDirAttack = (int)endMovement.y;
			return false;
		} else if ((hit.transform.gameObject.tag == "Enemy")) {
		// si un ennemis se met en face, la grenouille arrête de se préparer a attaquer va bouger a la place
			turnToAttack = -1;
			isAttackingThisTurn = false;
			MoveEnemy ();
			return true;
		}else
			return false;
	}
		
	public override void MoveEnemy (){
		if (turnToAttack == 0) {
			isAttackingThisTurn = true;
			AttemptMove (xDirAttack, yDirAttack);
		}
		else
			base.MoveEnemy ();
	}

	public IEnumerator frogAttack(GameObject col){
		print ("attack");
		if (col != null) {
			if (col.tag == "Player") {
				//Anim d'attaque ici
				col.GetComponent<Player> ().loseHP ();
				turnToAttack = -1;
				isAttackingThisTurn = false;
			}
		} else {
			//Anim d'attaque ici
			turnToAttack = -1;
			isAttackingThisTurn = false;
		}
		yield return null;
	}

	protected override void AttemptMove (int xDir, int yDir){
		RaycastHit2D hit;
		bool canMove = Move(xDir, yDir, out hit);
		if (!canMove) {
			OnCantMove (hit.transform.gameObject);
		}
	}

	protected override void OnCantMove (GameObject col){
		if (col.gameObject.tag == "Wall") {
			return;
		} else if (col.gameObject.tag == "Player") {
			if (isAttackingThisTurn) {
				StartCoroutine (frogAttack (col.gameObject));
			} else {
				print ("repéré");
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
