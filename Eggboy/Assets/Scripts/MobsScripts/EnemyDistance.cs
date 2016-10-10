using UnityEngine;
using System.Collections;

public abstract class EnemyDistance : Enemy{

	public int maxTurnBetweenAttack = 3;
	protected int cptTurnBetweenAttack;

	protected int xDirAttack;
	protected int yDirAttack;

	public enum Direction {horizontal, vertical};
	public Direction dir;

	public GameObject bullet;
	protected int rotation = 0;

	protected Vector2 screenPos;

	protected override void Start (){
		screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
		cptTurnBetweenAttack = maxTurnBetweenAttack;
		base.Start ();
		animator = null;
		xDirAttack = -1;
		rotation = 90;
		dir = Direction.horizontal;
	}

	public override void MoveEnemy ()
	{
		if (LookForTarget ()) {
			switchDirection ();
		}
		if (cptTurnBetweenAttack > 0) {
			cptTurnBetweenAttack--;
		} else {
			launchTir ();
		}
	}

	protected bool LookForTarget(){

		bool sameX = (target.GetComponent<Player> ().caseExacte.position.x == this.transform.position.x);
		bool sameY = (target.GetComponent<Player> ().caseExacte.position.y == this.transform.position.y);

		if ((sameX)||(sameY))
			return true;
		else 
			return false;
	}

	protected void switchDirection(){
		int diffX = (int)(target.GetComponent<MovingObject> ().caseExacte.position.x - this.transform.position.x);
		int diffY = (int)(target.GetComponent<MovingObject> ().caseExacte.position.y - this.transform.position.y);

		if (diffX > 0) {
			xDirAttack = 1;
			rotation = 90;
			dir = Direction.horizontal;
		} else if (diffX != 0) {
			xDirAttack = -1;
			rotation = 90;
			dir = Direction.horizontal;
		} else
			xDirAttack = 0;

		if (diffY > 0) {
			yDirAttack = 1;
			rotation = 0;
			dir = Direction.vertical;
		} else if (diffY != 0) {
			yDirAttack = -1;
			rotation = 0;
			dir = Direction.vertical;
		} else
			yDirAttack = 0;
	}

	public abstract void launchTir ();
	public abstract IEnumerator instantiateBullet (Vector3 position);

	void OnGUI(){
		var centeredStyle = GUI.skin.GetStyle("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.TextField (new Rect (screenPos.x , (Screen.height - screenPos.y), 15, 20), (cptTurnBetweenAttack+1).ToString ());
	}

}
