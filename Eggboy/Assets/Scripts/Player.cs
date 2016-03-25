using UnityEngine;
using System.Collections;

public class Player : MovingObject {


	public float restartLevelDelay = 1f;
	
	private Animator animator;

	private int hp;
	public GameManager manager;

	protected override void Start () {
		animator = GetComponent<Animator> ();
		manager = GameManager.instance;
		hp = manager.playerhpPoints;
		base.Start ();
	}

	protected override void AttemptMove(int xDir, int yDir)
	{
		base.AttemptMove(xDir, yDir);
		//RaycastHit2D hit;
		CheckIfGameOver ();
	}
		

	protected override bool Move(int xDir, int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			StartCoroutine(SmoothMovement(end));
			manager.playersTurn = false;
			return true;
		}
		return false;
	}

	private void onDisable(){
		GameManager.instance.playerhpPoints = hp;
	}

	private void CheckIfGameOver(){
		if (hp <= 0) {
			GameManager.instance.GameOver();
		}
	}

	protected override void OnCantMove (GameObject col)
	{
		if (col.gameObject.tag == "Wall") {
			animator.SetTrigger ("Blase");
			manager.playersTurn = true;
			return;
		}
		else if(col.gameObject.tag == "Enemy"){
			manager.playersTurn = false;
			col.GetComponent<Enemy> ().Die ();
		}
	}
		
	public void loseHP(){
		hp -= 1;
		CheckIfGameOver ();
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Exit") {
			//Invoke permet de mettre un delay avant de charger une fonction, ici Restart()
			Invoke ("Restart", restartLevelDelay);
			//==desactive le joueur, je pense
			enabled = false;
		} 
	}

	void Update () {
		if (!manager.playersTurn)
		//permet de ne pas se préoccuper de update si ce n'est pas le tour du joueur
			return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int) Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0){
		// pour empecher le mouvement en diagonale
			vertical = 0;
			switch (horizontal) {
			case -1:
				animator.SetBool ("isRight", false);
				break;
			case 1:
				animator.SetBool ("isRight", true);
				break;
			}
		}
		if (horizontal != 0 || vertical != 0) {
			AttemptMove(horizontal, vertical);
		}
		else if(Input.GetKeyDown(KeyCode.Space)){
			manager.playersTurn = false;
		}
	
	}
}
