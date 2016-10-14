using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Frog : Enemy {

	protected int xDirAttack = 0;
	protected int yDirAttack = 0;

	protected bool isPreparingAttack = false;

	public GameObject grenouilleLangue;
	private List<GameObject> langues;

	protected override void Start(){
		langues = new List<GameObject> ();
		enemyName = "frog";
		base.Start ();
		animator = null;
	}

	public override void MoveEnemy (){
		if (isPreparingAttack) {
			tryToAttack ();
			return;
		}
		clearLangues ();
		base.MoveEnemy ();
	}

//MOVEMENT_______________________________________________________________________________________
// MoveEnemy() -> AttemptMove() -> Move()|-> "peut bouger " -> SmoothMovement() 
//										 |-> "ne peut pas bouger" -> OnCantMove()
// si le mouvement a marché, rappel de la boucle complète à partir de MoveEnemy() pour tenter de repérer le joueur après le mouvement


	protected override void AttemptMove (int xDir, int yDir){
		if(skipMove){
			skipMove = false;
			endTurnEnemy = true;
			return;
		}
		RaycastHit2D hit;
		bool canMove = Move(xDir, yDir, out hit);
		if (!canMove) {
			OnCantMove (hit.transform.gameObject);
		}
		;
	}

	public override void doMove(int xDir, int yDir)
	{
		RaycastHit2D hit;
		bool canMove = SimpleMove(xDir, yDir, out hit);
		if (hit.transform == null) {
			return;
		} else if (!canMove) {
			OnCantMove (hit.transform.gameObject);
		}
	}

	protected override void OnCantMove (GameObject col){
		endTurnEnemy = true;
		if (col.gameObject.tag == "Wall") {
			return;
		} else if (col.gameObject.tag == "Player") {
			isPreparingAttack = true;
		}
	}

	protected bool SimpleMove (int xDir, int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			if (piege != null) {
				piege.TriggerExit ();
			}
			caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x + xDir, transform.position.y + yDir));
			GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)end.x, (int)end.y, -1, this);
			GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)transform.position.x, (int)transform.position.y, 1,null);
			StartCoroutine (SmoothMovement (end));
			return true;
			}
		return false;
	}

	protected override bool Move (int xDir, int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			if (PlayerIsInRange (end, xDir, yDir, out hit)) {
				xDirAttack =xDir;
				yDirAttack = yDir;
				return false;
			} else {
				if (piege != null) {
					piege.TriggerExit ();
				}
				caseExacte = new BoardManager.Node (1, new Vector2 (transform.position.x + xDir, transform.position.y + yDir));
				GameManager.instance.getCurrentBoard ().setCharacterOnGrid ((int)end.x, (int)end.y, -1, this);
				GameManager.instance.getCurrentBoard ().setCharacterOnGrid ((int)transform.position.x, (int)transform.position.y, 1, null);
				StartCoroutine (SmoothMovement (end));
				return true;
			}
		} else if (hit.transform.gameObject.tag == "Player") {
			xDirAttack = xDir;
			yDirAttack = yDir;
			return false;
		} else if (hit.transform.gameObject.tag == "Bullet") {
			if (piege != null) {
				piege.TriggerExit ();
			}
			caseExacte = new BoardManager.Node (1, new Vector2 (transform.position.x + xDir, transform.position.y + yDir));
			GameManager.instance.getCurrentBoard ().setCharacterOnGrid ((int)end.x, (int)end.y, -1, this);
			GameManager.instance.getCurrentBoard ().setCharacterOnGrid ((int)transform.position.x, (int)transform.position.y, 1,null);
			StartCoroutine (SmoothMovement (end));
			return true;
		}
		return false;
	}

	protected override IEnumerator SmoothMovement(Vector3 end){
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
				Vector2 find = findPlayerRange ();
				if (find != new Vector2 (0, 0)) {
					isPreparingAttack = true;
					xDirAttack = (int)find.x;
					yDirAttack = (int)find.y;
				}
			} else {
				setIsUnderTrapEffect (true);
				piege.declencherPiege ();
			}
		} else {
			endTurnEnemy = true;
			setIsUnderTrapEffect(false);
			Vector2 find = findPlayerRange ();
			if (find != new Vector2 (0, 0)) {
				endTurnEnemy = true;
				isPreparingAttack = true;
				xDirAttack = (int)find.x;
				yDirAttack = (int)find.y;
			} else {
			}
		}
	}

	//ATTACK_______________________________________________________________________________________

	protected bool Attack (int xDir, int yDir, out RaycastHit2D hit){
		// fonction similaire a Move() permettant de chercher le joueur sur les 2 cases devant 
		// lance l'attaque si le joueur a été repéré ou si aucun autre ennemis ne s'interpose entre le joueur et la grenouille
		endTurnEnemy = true;
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
		if ((testRange.transform != null) && (testRange.transform.gameObject.tag == "Player")) {
			return true;
		}else
			return false;
	}

	public void tryToAttack(){
		// lance l'attaque ou l'action de ne pas attaquer si une condtion d'attaque n'est pas respecté
		RaycastHit2D hit;
		endTurnEnemy = true;
		bool canAttack = Attack (xDirAttack, yDirAttack, out hit);
		if(!canAttack) {
			cantAttack (hit.transform.gameObject);
		}
		xDirAttack = 0;
		yDirAttack = 0;
	}

	public IEnumerator frogAttack(GameObject col){
		if (col != null) {
			if (col.tag == "Player") {
				//Anim d'attaque ici
				instantiateLangue();
				col.GetComponent<Player> ().loseHP ();
			}
		} else {
			//Anim d'attaque ici
			instantiateLangue();
		}
		isPreparingAttack = false;
		endTurnEnemy = true;
		yield return null;
	}

	public void cantAttack(GameObject col){
		isPreparingAttack = false;
		if ((col.tag == "Enemy")) {
			MoveEnemy ();
		}
	}

	public void instantiateLangue(){
		int posX = (int)caseExacte.position.x;
		int posY = (int)caseExacte.position.y;
		if (xDirAttack == 1) {
			if (posX < 14) {
				GameObject langue = Instantiate (grenouilleLangue, new Vector3 ((caseExacte.position.x + xDirAttack), (caseExacte.position.y), 1), Quaternion.identity) as GameObject;
				langue.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 90));
				langue.transform.SetParent (this.transform);
				langues.Add (langue);
			}
			if (posX < 13) {
				GameObject langue = Instantiate (grenouilleLangue, new Vector3 ((caseExacte.position.x + xDirAttack + 1), (caseExacte.position.y), 1), Quaternion.identity) as GameObject;
				langue.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 90));
				langue.transform.SetParent (this.transform);
				langues.Add (langue);
			}
		} else if (xDirAttack == -1) {
			if (posX != 0) {
				GameObject langue = Instantiate (grenouilleLangue, new Vector3 ((caseExacte.position.x + xDirAttack), (caseExacte.position.y), 1), Quaternion.identity) as GameObject;
				langue.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 90));
				langue.transform.SetParent (this.transform);
				langues.Add (langue);
			}
			if (posX > 1) {
				GameObject langue = Instantiate (grenouilleLangue, new Vector3 ((caseExacte.position.x + xDirAttack - 1), (caseExacte.position.y), 1), Quaternion.identity) as GameObject;
				langue.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 90));
				langue.transform.SetParent (this.transform);
				langues.Add (langue);
			}
		} else if (yDirAttack == 1) {
			if (posY != 7) {
				GameObject langue = Instantiate (grenouilleLangue, new Vector3 ((caseExacte.position.x), (caseExacte.position.y+yDirAttack), 1), Quaternion.identity) as GameObject;
				langue.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
				langue.transform.SetParent (this.transform);
				langues.Add (langue);
			}
			if (posY < 6) {
				GameObject langue = Instantiate (grenouilleLangue, new Vector3 ((caseExacte.position.x), (caseExacte.position.y+yDirAttack+1), 1), Quaternion.identity) as GameObject;
				langue.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
				langue.transform.SetParent (this.transform);
				langues.Add (langue);
			}
		} else if (yDirAttack == -1) {
			if (posY != 0) {
				GameObject langue = Instantiate (grenouilleLangue, new Vector3 ((caseExacte.position.x), (caseExacte.position.y+yDirAttack), 1), Quaternion.identity) as GameObject;
				langue.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
				langue.transform.SetParent (this.transform);
				langues.Add (langue);
			}
			if (posY > 1) {
				GameObject langue = Instantiate (grenouilleLangue, new Vector3 ((caseExacte.position.x), (caseExacte.position.y+yDirAttack-1), 1), Quaternion.identity) as GameObject;
				langue.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
				langue.transform.SetParent (this.transform);
				langues.Add (langue);
			}
		}
	}

	public void clearLangues(){
		for (int i = 0; i < langues.Count; i++) {
			Destroy (langues [i].gameObject);
		}
		langues.Clear ();
	}

	public Vector2 findPlayerRange(){
		Vector2 isPlayer = new Vector2 (0, 0);
		int posX = (int)caseExacte.position.x;
		int posY = (int)caseExacte.position.y;
		if (posX != 14) {
			if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 1, (int)posY].nodeCharacter != null) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 1, (int)posY].nodeCharacter.gameObject.tag == "Player")
					isPlayer = new Vector2 (1, 0);
			}
		}
		if (posX < 13) {
			if ((GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 2, (int)posY].nodeCharacter != null)
			    && (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 1, (int)posY].valeur != -1)) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 2, (int)posY].nodeCharacter.gameObject.tag == "Player")
					isPlayer = new Vector2 (1, 0);
			}
		}
		if (posX != 0) {
			if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 1, (int)posY].nodeCharacter != null) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 1, (int)posY].nodeCharacter.gameObject.tag == "Player")
					isPlayer = new Vector2 (-1, 0);
			}
		}
		if (posX > 1) {
			if ((GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 2, (int)posY].nodeCharacter != null)
			   && (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 1, (int)posY].valeur != -1)) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 2, (int)posY].nodeCharacter.gameObject.tag == "Player")
					isPlayer = new Vector2 (-1, 0);
			}
		}
	
		if (posY != 7) {
			if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 1].nodeCharacter != null) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 1].nodeCharacter.gameObject.tag == "Player")
					isPlayer = new Vector2 (0, 1);
			}
		}
		if (posY < 6) {
			if ((GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 2].nodeCharacter != null)
			  && (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 1].valeur != -1)) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 2].nodeCharacter.gameObject.tag == "Player")
					isPlayer = new Vector2 (0, 1);
			}
		}
		if (posY != 0) {
			if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 1].nodeCharacter != null) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 1].nodeCharacter.gameObject.tag == "Player")
					isPlayer = new Vector2 (0, -1);
			}
		}
		if (posY > 1) {
			if ((GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 2].nodeCharacter != null)
			  && (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 1].valeur != -1)) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 2].nodeCharacter.gameObject.tag == "Player")
					isPlayer = new Vector2 (0, -1);
			}
		}
		return isPlayer;
	}
}
