using UnityEngine;
using System.Collections;

public class Frog : Enemy {

	protected int xDirAttack = 0;
	protected int yDirAttack = 0;

	protected bool isPreparingAttack = false;
	protected bool isMoving = false;

	protected override void Start(){
		base.Start ();
		animator = null;
	}

	public override void MoveEnemy (){
		if (isPreparingAttack) {
			tryToAttack ();
			return;
		}
		base.MoveEnemy ();
	}

//MOVEMENT_______________________________________________________________________________________
// MoveEnemy() -> AttemptMove() -> Move()|-> "peut bouger " -> SmoothMovement() 
//										 |-> "ne peut pas bouger" -> OnCantMove()
// si le mouvement a marché, rappel de la boucle complète à partir de MoveEnemy() pour tenter de repérer le joueur après le mouvement


	protected override void AttemptMove (int xDir, int yDir){
		RaycastHit2D hit;
		bool canMove = Move(xDir, yDir, out hit);
		if (!canMove) {
			OnCantMove (hit.transform.gameObject);
		}
		isMoving = false;
	}

	protected override void OnCantMove (GameObject col){
		if (col.gameObject.tag == "Wall") {
			return;
		} else if (col.gameObject.tag == "Player") {
			isMoving = false;
			isPreparingAttack = true;
		}
	}

	protected override bool Move (int xDir, int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			if (PlayerIsInRange (end, xDir, yDir, out hit)) {
				xDirAttack = xDir;
				yDirAttack = yDir;
				return false;
			} else {
				if (isMoving)
					return true;
				GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)end.x, (int)end.y, -1);
				GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1);
				StartCoroutine (SmoothMovement (end));
				return true;
			}
		} else if (hit.transform.gameObject.tag == "Player") {
			xDirAttack = xDir;
			yDirAttack = yDir;
			return false;
		}
		return false;
	}

	protected IEnumerator SmoothMovement(Vector3 end){
		//coroutine permettant de bouger une unité d'un espace/une case 
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
		isMoving = true;
		MoveEnemy ();
	}

	//ATTACK_______________________________________________________________________________________

	protected bool Attack (int xDir, int yDir, out RaycastHit2D hit){
		// fonction similaire a Move() permettant de chercher le joueur sur les 2 cases devant 
		// lance l'attaque si le joueur a été repéré ou si aucun autre ennemis ne s'interpose entre le joueur et la grenouille
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			if (PlayerIsInRange (end, xDir, yDir, out hit)) {
				StartCoroutine (frogAttack (hit.transform.gameObject));
				return true;
			} else {
				StartCoroutine (frogAttack (null));
				return true;
			}
		} else if (hit.transform.gameObject.tag == "Player") {
			StartCoroutine (frogAttack (hit.transform.gameObject));
			return true;
		}
		return false;
	}

	public bool PlayerIsInRange(Vector2 start, int xDir, int yDir, out RaycastHit2D testRange){
		// teste la case "longue distance" pour y chercher le joueur
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		testRange = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if ((testRange.transform != null) && (testRange.transform.gameObject.tag == "Player"))
			return true;
		else
			return false;
	}

	public void tryToAttack(){
		// lance l'attaque ou l'action de ne pas attaquer si une condtion d'attaque n'est pas respecté
		RaycastHit2D hit;
		bool canAttack = Attack (xDirAttack, yDirAttack, out hit);
		if(!canAttack) {
			cantAttack (hit.transform.gameObject);
		}
		xDirAttack = 0;
		yDirAttack = 0;
	}

	public IEnumerator frogAttack(GameObject col){
		print ("attack");
		if (col != null) {
			if (col.tag == "Player") {
				//Anim d'attaque ici
				col.GetComponent<Player> ().loseHP ();
				isPreparingAttack = false;
			}
		} else {
			//Anim d'attaque ici
			isPreparingAttack = false;
		}
		yield return null;
	}

	public void cantAttack(GameObject col){
		isPreparingAttack = false;
		if ((col.tag == "Enemy")) {
			MoveEnemy ();
		}
	}
}
