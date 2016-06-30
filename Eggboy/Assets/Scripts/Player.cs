﻿using UnityEngine;
using System.Collections;

public class Player : MovingObject {


	public float restartLevelDelay = 1f;
	
	private Animator animator;

	public float timeBetweenTurn = 0.2f;
	private const float MAX_TIME_BETWEEN_TURN = 0.2f;

	private int hp;
	private GameManager manager;

	private UIPlayer uiManager;

	private CameraManager camera;

	public BoardManager.Node caseExacte;

    private bool isTrap = false;

    [HideInInspector] public Trap piege;

	protected override void Start () {
		animator = GetComponent<Animator> ();
		manager = GameManager.instance;
		hp = manager.playerhpPoints;

		uiManager = GameObject.Find ("PlayerPanel").GetComponent<UIPlayer>();
		uiManager.updateLife ();

		camera = GameObject.Find ("Main Camera").GetComponent<CameraManager> ();
		caseExacte = new BoardManager.Node (1, new Vector2 (transform.position.x, transform.position.y));

		base.Start ();
	}

	protected override bool Move(int xDir, int yDir, out RaycastHit2D hit){
		print ("Eggboy ____________");
		print (caseExacte.position.ToString ());
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			animator.SetTrigger ("isRunning");
			caseExacte = new BoardManager.Node (1, new Vector2 (transform.position.x + xDir, transform.position.y + yDir));
			print (caseExacte.position.ToString ());
			print ("____________");
			StartCoroutine(SmoothMovement(end));
           
            
            return true;
		}
		return false;
	}

	/*private void onDisable(){
		GameManager.instance.playerhpPoints = hp;
	}*/

	private bool CheckIfGameOver(){
		if (hp <= 0) {
			animator.SetTrigger ("isDead");
			GameManager.instance.GameOver();
			return true;
		}
		else
			return false;
	}

	protected override IEnumerator SmoothMovement (Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}

        if (!isTrap)
        {
            manager.playersTurn = false;
            enabled = true;

        }
        else
        {
            StartCoroutine(piege.declencherPiege());

        }
        
    }

	protected override void OnCantMove (GameObject col)
	{
		//caseExacte = new BoardManager.Node (1, new Vector2 (transform.position.x, transform.position.y ));
		if (col.gameObject.tag == "Wall") {
			animator.SetTrigger ("Blase");
			manager.playersTurn = true;
			return;
		}
		else if(col.gameObject.tag == "Enemy"){
			manager.playersTurn = false;
			col.GetComponent<Enemy> ().Die ();
			return;
		}
	}
		
	public void loseHP(){
		camera.setShake (0.6f);
		animator.SetTrigger ("isDamaged");
		manager.playerhpPoints -= 1;
		hp = manager.playerhpPoints;
		uiManager.updateLife ();
		//CheckIfGameOver ();
	}

	public int getHp(){
		return hp;
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Exit") {
			//Invoke permet de mettre un delay avant de charger une fonction, ici Restart()
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} 
	}

	void Update () {
		if (!manager.playersTurn)
		//permet de ne pas se préoccuper de update si ce n'est pas le tour du joueur
			return;

		//permet de tempo le jeu notamment pour l'activation des pièges
		if (timeBetweenTurn > 0f) {
			timeBetweenTurn -= Time.deltaTime;
			return;
		}
		if (CheckIfGameOver ()) {
			this.enabled = false;
			return;
		}

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
			timeBetweenTurn = MAX_TIME_BETWEEN_TURN;
			AttemptMove(horizontal, vertical);
		}
		else if(Input.GetKeyDown(KeyCode.Space)){
			//print (caseExacte.position.ToString ());
			timeBetweenTurn = MAX_TIME_BETWEEN_TURN;
			manager.playersTurn = false;
		}
	}

    public void doMove(int xDir, int yDir)
    {
        AttemptMove(xDir, yDir);

    }
     
    public bool getisTrap()
    {

        return this.isTrap;
    }

    public void setIsTrap(bool b)
    {

        this.isTrap = b;
    }
}
