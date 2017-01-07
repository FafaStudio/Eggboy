using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyLaser : EnemyDistance{

	public bool hasLaunchLaserLastTurn = false;
	public List<GameObject> lasers;

	protected override void Start ()
	{
		enemyName = "laser";
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
		switch (direction)
		{
		case Direction.gauche:
			for (int i = 1; i <= this.transform.position.x; i++) {
				StartCoroutine (instantiateBullet (new Vector3 (this.transform.position.x - i, this.transform.position.y, 1)));
			}
			break;
		case Direction.droite:
			for (int i = 1; i <= 14-this.transform.position.x; i++) {
				StartCoroutine(instantiateBullet(new Vector3(this.transform.position.x+i, this.transform.position.y, 1)));
			}
			break;
		case Direction.bas:
			for (int i = 1; i <= this.transform.position.y; i++) {
				StartCoroutine(instantiateBullet(new Vector3(this.transform.position.x, this.transform.position.y-i, 1)));
			}
			break;
		case Direction.haut:
			for (int i = 1; i <= 7-this.transform.position.y; i++) {
				StartCoroutine(instantiateBullet(new Vector3(this.transform.position.x, this.transform.position.y+i, 1)));
			}							
			break;
		
		}
		cptTurnBetweenAttack = maxTurnBetweenAttack;
	}

	public override IEnumerator instantiateBullet(Vector3 position){
		GameObject laser = Instantiate(bullet, position, Quaternion.identity) as GameObject;
		laser.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));
		lasers.Add (laser);
		yield return null;
		//yield return new WaitForSeconds(0.1f);
		//Destroy (laser);
	}

	public void clearTir(){
		for (int i = 0; i < lasers.Count; i++) {
			Destroy (lasers [i].gameObject);
		}
		lasers.Clear ();
		hasLaunchLaserLastTurn = false;
	}

    public override void Die()
    {
        clearTir();
        base.Die();
    }


}
