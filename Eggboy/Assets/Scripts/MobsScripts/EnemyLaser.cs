using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyLaser : EnemyDistance{

	public bool hasLaunchLaserLastTurn = false;
	public List<GameObject> lasers;

	protected override void Start ()
	{
		base.Start ();
	}

	public override void MoveEnemy ()
	{
		/*if (hasLaunchLaserLastTurn) {
			clearTir ();
		}*/
		base.MoveEnemy ();
	}

	public override void launchTir ()
	{
		hasLaunchLaserLastTurn = true;
		switch (dir)
		{

		case Direction.horizontal:
			if (xDirAttack == -1) {
				for (int i = 1; i <= this.transform.position.x; i++) {
					StartCoroutine(instantiateLaser(new Vector3(this.transform.position.x-i, this.transform.position.y, 1)));
				}
			}else{
				for (int i = 1; i <= 14-this.transform.position.x; i++) {
					StartCoroutine(instantiateLaser(new Vector3(this.transform.position.x+i, this.transform.position.y, 1)));
				}							
			}
			break;
		case Direction.vertical:
			if (yDirAttack == -1) {
				for (int i = 1; i <= this.transform.position.y; i++) {
					StartCoroutine(instantiateLaser(new Vector3(this.transform.position.x, this.transform.position.y-i, 1)));
				}
			}else{
				for (int i = 1; i <= 7-this.transform.position.y; i++) {
					StartCoroutine(instantiateLaser(new Vector3(this.transform.position.x, this.transform.position.y+i, 1)));
				}							
			}
			break;
		
		}
		cptTurnBetweenAttack = maxTurnBetweenAttack;
	}

	public IEnumerator instantiateLaser(Vector3 position){
		GameObject laser = Instantiate(bullet, position, Quaternion.identity) as GameObject;
		laser.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));
		lasers.Add (laser);
		yield return new WaitForSeconds (0.3f);
		Destroy (laser);
	}

	public void clearTir(){
		for (int i = 0; i < lasers.Count; i++) {
			Destroy (lasers [i].gameObject);
		}
		lasers.Clear ();
		hasLaunchLaserLastTurn = false;
	}

}
