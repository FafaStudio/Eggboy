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
		bool canMove = Move(xDir, yDir);
		if (!canMove) {
			StartCoroutine(OnCantMove ());
		}
	}

	public override void doMove(int xDir, int yDir){
		bool canMove = SimpleMove(xDir, yDir);
		if (blockingObject == null) {
			return;
		} else if (!canMove) {
			OnCantMove ();
		}
	}

	protected override IEnumerator OnCantMove (){
		endTurnEnemy = true;
		if (blockingObject != null) {
			if (blockingObject.tag == "Wall") {
				blockingObject = null;
				yield return null;
			} else if (blockingObject.tag == "Player") {
				blockingObject = null;
				isPreparingAttack = true;
			} else {
				blockingObject = null;
			}
		}
	}

	protected bool SimpleMove (int xDir, int yDir){
		Vector2 end = caseExacte.position + new Vector2 (xDir, yDir);
		blockingObject = manager.getCurrentBoard ().gridPositions [(int)(caseExacte.position.x + xDir), (int)(caseExacte.position.y + yDir)].nodeObject;
		if(blockingObject==null){
			if (piege != null) {
				piege.TriggerExit ();
			}
			caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x + xDir, transform.position.y + yDir));
			GameManager.instance.getCurrentBoard ().setObjectOnGrid((int)end.x, (int)end.y, -1, this.gameObject);
			GameManager.instance.getCurrentBoard ().setObjectOnGrid((int)transform.position.x, (int)transform.position.y, 1,null);
			StartCoroutine (SmoothMovement (end));
			return true;
		}
		return false;
	}

	protected override bool Move (int xDir, int yDir){
		Vector2 end = caseExacte.position + new Vector2 (xDir, yDir);
		blockingObject = manager.getCurrentBoard ().gridPositions [(int)(caseExacte.position.x + xDir), (int)(caseExacte.position.y + yDir)].nodeObject;
		if(blockingObject==null){
			if (PlayerIsInRange (end, xDir, yDir)) {
				xDirAttack =xDir;
				yDirAttack = yDir;
				blockingObject = GameObject.FindGameObjectWithTag ("Player");
				return false;
			} else {
				if (piege != null) {
					piege.TriggerExit ();
				}
				caseExacte = new BoardManager.Node (1, new Vector2 (transform.position.x + xDir, transform.position.y + yDir));
				GameManager.instance.getCurrentBoard ().setObjectOnGrid((int)end.x, (int)end.y, -1, this.gameObject);
				GameManager.instance.getCurrentBoard ().setObjectOnGrid((int)transform.position.x, (int)transform.position.y, 1,null);
				StartCoroutine (SmoothMovement (end));
				return true;
			}
		} else if (blockingObject.tag == "Player") {
			xDirAttack = xDir;
			yDirAttack = yDir;
			return false;
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

	protected bool Attack (int xDir, int yDir){
		// fonction similaire a Move() permettant de chercher le joueur sur les 2 cases devant 
		// lance l'attaque si le joueur a été repéré ou si aucun autre ennemis ne s'interpose entre le joueur et la grenouille
		endTurnEnemy = true;
		Vector2 end = caseExacte.position + new Vector2 (xDir, yDir);
		GameObject testObject = manager.getCurrentBoard ().gridPositions [(int)(caseExacte.position.x + xDir), (int)(caseExacte.position.y + yDir)].nodeObject;
		if(testObject==null){
			testObject = PlayerIsInRange (end, xDir, yDir);
			if (testObject!=null) {
				StartCoroutine (frogAttack (testObject));
				return true;
			} else {
				StartCoroutine (frogAttack (null));
				return true;
			}
		} else if (testObject.tag=="Player") {
			StartCoroutine (frogAttack (testObject));
			return true;
		}
		blockingObject = testObject;
		return false;
	}

	public GameObject PlayerIsInRange(Vector2 start, int xDir, int yDir){
		// teste la case "longue distance" pour y chercher le joueur
		Vector2 end = start+new Vector2(xDir, yDir);
		if ((end.x < 0)|| (end.x > 14 )|| (end.y < 0) || (end.y > 7))
			return null;
		GameObject testPlayer = manager.getCurrentBoard ().gridPositions [(int)(end.x), (int)(end.y)].nodeObject;
		if(testPlayer!=null){
			if (testPlayer.tag == "Player")
				return testPlayer;
			else
				return null;
		}else
			return null;
	}

	public void tryToAttack(){
		// lance l'attaque ou l'action de ne pas attaquer si une condtion d'attaque n'est pas respecté
		endTurnEnemy = true;
		bool canAttack = Attack (xDirAttack, yDirAttack);
		if(!canAttack) {
			cantAttack ();
		}
		xDirAttack = 0;
		yDirAttack = 0;
	}

	public IEnumerator frogAttack(GameObject col){
		if (col != null) {
			if (col.tag == "Player") {
				//Anim d'attaque ici
				instantiateLangue();
				bool attackWin = col.GetComponent<Player> ().loseHP ();
				if ((attackWin) && (GameManager.instance.PlayerHasItem ("VFlu")))
					Die ();
			}
		} else {
			//Anim d'attaque ici
			instantiateLangue();
		}
		isPreparingAttack = false;
		endTurnEnemy = true;
		yield return null;
	}

	public void cantAttack(){
		isPreparingAttack = false;
		if ((blockingObject.tag == "Enemy")) {
			MoveEnemy ();
			blockingObject = null;
		} else {
			blockingObject = null;
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
			if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 1, (int)posY].nodeObject != null) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 1, (int)posY].nodeObject.tag == "Player")
					isPlayer = new Vector2 (1, 0);
			}
		}
		if (posX < 13) {
			if ((GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 2, (int)posY].nodeObject != null)
			    && (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 1, (int)posY].valeur != -1)) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX + 2, (int)posY].nodeObject.tag == "Player")
					isPlayer = new Vector2 (1, 0);
			}
		}
		if (posX != 0) {
			if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 1, (int)posY].nodeObject != null) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 1, (int)posY].nodeObject.tag == "Player")
					isPlayer = new Vector2 (-1, 0);
			}
		}
		if (posX > 1) {
			if ((GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 2, (int)posY].nodeObject != null)
			   && (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 1, (int)posY].valeur != -1)) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX - 2, (int)posY].nodeObject.tag == "Player")
					isPlayer = new Vector2 (-1, 0);
			}
		}
		if (posY != 7) {
			if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 1].nodeObject != null) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 1].nodeObject.tag == "Player")
					isPlayer = new Vector2 (0, 1);
			}
		}
		if (posY < 6) {
			if ((GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 2].nodeObject != null)
			  && (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 1].valeur != -1)) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY + 2].nodeObject.tag == "Player")
					isPlayer = new Vector2 (0, 1);
			}
		}
		if (posY != 0) {
			if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 1].nodeObject != null) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 1].nodeObject.tag == "Player")
					isPlayer = new Vector2 (0, -1);
			}
		}
		if (posY > 1) {
			if ((GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 2].nodeObject != null)
			  && (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 1].valeur != -1)) {
				if (GameManager.instance.getCurrentBoard ().gridPositions [(int)posX, (int)posY - 2].nodeObject.tag == "Player")
					isPlayer = new Vector2 (0, -1);
			}
		}
		return isPlayer;
	}
}
