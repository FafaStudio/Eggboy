using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject {


	public int playerDamage;                            //The amount of food points to subtract from the player when attacking.
	
	private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
	private Transform target;                           //Transform to attempt to move toward each turn.
	private bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.
	private bool isDead = false;
	
	
	//Start overrides the virtual Start function of the base class.
	protected override void Start ()
	{
		//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
		//This allows the GameManager to issue movement commands.
	    GameManager.instance.AddEnemyToList (this);
		
		//Get and store a reference to the attached Animator component.
		animator = GetComponent<Animator> ();

		//Find the Player GameObject using it's tag and store a reference to its transform component.
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		
		//Call the start function of our base class MovingObject.
		base.Start ();
	}

	public void Die(){
		isDead = true;
		GameManager.instance.RemoveEnemyToList (this);
		Destroy (this.gameObject);
	}
	
	
	//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
	//See comments in MovingObject for more on how base AttemptMove function works.
	protected override void AttemptMove (int xDir, int yDir)
	{
		//Check if skipMove is true, if so set it to false and skip this turn.
		if(skipMove)
		{
			skipMove = false;
			return;
			
		}
		
		//Call the AttemptMove function from MovingObject.
		base.AttemptMove(xDir, yDir);
		//skipMove = true;
	}
	
	
	//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
	public virtual void MoveEnemy ()
	{
		if (isDead)
			return;
		
		int xDir = 0;
		int yDir = 0;


		/*if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
			
			//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
			yDir = target.position.y > transform.position.y ? 1 : -1;
		
		else
			//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
			xDir = target.position.x > transform.position.x ? 1 : -1;*/
		BoardManager.Grid currentPos = new BoardManager.Grid(1, new Vector2(this.transform.position.x,this.transform.position.y));

		Vector2 nextPosition = GameManager.instance.getCurrentBoard().doPathfinding(target.GetComponent<Player>().caseExacte, currentPos);

		if ((nextPosition.x == -1f) && (nextPosition.y == -1f)) {
			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
				yDir = target.position.y > transform.position.y ? 1 : -1;
			else
				//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
				xDir = target.position.x > transform.position.x ? 1 : -1;
		} else {
			xDir = (int)nextPosition.x - (int)this.transform.position.x;
			yDir = (int)nextPosition.y - (int)this.transform.position.y;
		}
		
		AttemptMove (xDir, yDir);
		GameManager.instance.getCurrentBoard ().resetDistanceGrille ();
	}
	
	
	//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
	//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
	protected override void OnCantMove (GameObject col)
	{
		if (col.gameObject.tag == "Wall") {
			return;
		}
		else if(col.gameObject.tag == "Player"){
			col.gameObject.GetComponent<Player>().loseHP();
			animator.SetTrigger ("enemyAttack");
		}
	}
}
