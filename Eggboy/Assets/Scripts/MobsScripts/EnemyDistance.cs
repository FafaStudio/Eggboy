using UnityEngine;
using System.Collections;

public abstract class EnemyDistance : Enemy{

	public int maxTurnBetweenAttack = 3;
	protected int cptTurnBetweenAttack;

	public int xDirAttack;
	public int yDirAttack;

	public enum Direction{haut, bas, gauche, droite};
	public Direction direction;
	public GameObject bullet;
	protected int rotation = 0;

	protected Vector2 screenPos;

	protected override void Start (){
		screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
		cptTurnBetweenAttack = maxTurnBetweenAttack;
		base.Start ();
		animator = null;
		//xDirAttack = -1;
		initialiseShoot();
		if ((direction == Direction.droite)||(direction == Direction.gauche)) {
			rotation = 90;
		}
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
			direction = Direction.droite;
			xDirAttack = 1;
			rotation = 90;
		} else if (diffX != 0) {
			xDirAttack = -1;
			direction = Direction.gauche;
			rotation = 90;
		} else
			xDirAttack = 0;

		if (diffY > 0) {
			yDirAttack = 1;
			direction = Direction.haut;
			rotation = 0;
		} else if (diffY != 0) {
			yDirAttack = -1;
			direction = Direction.bas;
			rotation = 0;
		} else
			yDirAttack = 0;
	}

	public void initialiseShoot(){
		switch (direction) {
		case Direction.gauche:
			xDirAttack = -1;
			yDirAttack = 0;
			break;
		case Direction.bas:
			xDirAttack = 0;
			yDirAttack = -1;
			break;
		case Direction.droite:
			xDirAttack = 1;
			yDirAttack = 0;
			break;
		case Direction.haut:
			xDirAttack = 0;
			yDirAttack = 1;
			break;
		}
	}

	public abstract void launchTir ();
	public abstract IEnumerator instantiateBullet (Vector3 position);

	void OnGUI(){
		var centeredStyle = GUI.skin.GetStyle("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.TextField (new Rect (screenPos.x , (Screen.height - screenPos.y), 15, 20), (cptTurnBetweenAttack+1).ToString ());
	}

}
