﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyRocket : EnemyDistance {

	//public List<Rocket> roquettestirés;

	protected override void Start (){
		enemyName = "rocket";
		base.Start ();
	}

	public override void MoveEnemy (){
		base.MoveEnemy ();
	}

	public override void launchTir ()
	{
		switch (direction) {
		case Direction.gauche:
				StartCoroutine(instantiateBullet(new Vector3(this.transform.position.x+xDirAttack, this.transform.position.y, 1)));
				break;
		case Direction.bas:
				StartCoroutine(instantiateBullet(new Vector3(this.transform.position.x, this.transform.position.y+yDirAttack, 1)));
				break;
		case Direction.droite:
			StartCoroutine(instantiateBullet(new Vector3(this.transform.position.x+xDirAttack, this.transform.position.y, 1)));
			break;
		case Direction.haut:
			StartCoroutine(instantiateBullet(new Vector3(this.transform.position.x, this.transform.position.y+yDirAttack, 1)));
			break;
		}
		cptTurnBetweenAttack = maxTurnBetweenAttack;
	}

	public override IEnumerator instantiateBullet(Vector3 position){
		if (canFire ()) {
			GameObject missile = Instantiate (bullet, position, Quaternion.identity) as GameObject;
			missile.GetComponent<Rocket> ().setVelocity (new Vector2 (xDirAttack, yDirAttack));
			missile.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, rotation));
			missile.GetComponent<Rocket> ().setTireur (this);
			manager.AddRocketToList (missile.GetComponent<Rocket> ());
		}
		yield return null;
	}

	public bool canFire(){
		if (xDirAttack == -1) {
			if (caseExacte.position.x == 0) {
				return false;
			} else if ((manager.getCurrentBoard ().gridPositions [(int)caseExacte.position.x - 1, (int)caseExacte.position.y].valeur == -1)
				&&(manager.getCurrentBoard ().gridPositions [(int)caseExacte.position.x - 1, (int)caseExacte.position.y].nodeObject.tag!="Player")) {
				return false;
			}
		} else if (xDirAttack == 1) {
			if (caseExacte.position.x == 14) {
				return false;
			}else if ((manager.getCurrentBoard ().gridPositions [(int)caseExacte.position.x + 1, (int)caseExacte.position.y].valeur == -1) 
				&&(manager.getCurrentBoard ().gridPositions [(int)caseExacte.position.x + 1, (int)caseExacte.position.y].nodeObject.tag!="Player")) {
				return false;
			}
		} else if (yDirAttack == -1) {
			if (caseExacte.position.y == 0) {
				return false;
			}else if ((manager.getCurrentBoard ().gridPositions [(int)caseExacte.position.x , (int)caseExacte.position.y-1].valeur == -1) 
				&&(manager.getCurrentBoard ().gridPositions [(int)caseExacte.position.x, (int)caseExacte.position.y-1].nodeObject.tag!="Player")) {
				return false;
			}
		} else if (yDirAttack == 1) {
			if (caseExacte.position.y == 7) {
				return false;
			}else if ((manager.getCurrentBoard ().gridPositions [(int)caseExacte.position.x , (int)caseExacte.position.y+1].valeur == -1) 
				&&(manager.getCurrentBoard ().gridPositions [(int)caseExacte.position.x, (int)caseExacte.position.y+1].nodeObject.tag!="Player")) {
				return false;
			}
		} 
		return true;
	}
}
