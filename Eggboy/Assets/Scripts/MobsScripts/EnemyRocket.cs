using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyRocket : EnemyDistance {

	public List<Rocket> roquettestirés;

	protected override void Start (){
		enemyName = "rocket";
		base.Start ();
	}

	public override void MoveEnemy (){
		List<Rocket> temporaire = new List<Rocket> ();
		for (int i = 0; i < roquettestirés.Count; i++) {
			temporaire.Add(roquettestirés [i]);
		}
		for (int j = 0; j < temporaire.Count; j++) {
			temporaire [j].MoveBullet ();
		}
		base.MoveEnemy ();
	}

	public override void launchTir ()
	{
		switch (dir) {
			case Direction.horizontal:
				StartCoroutine(instantiateBullet(new Vector3(this.transform.position.x+xDirAttack, this.transform.position.y, 1)));
				break;
			case Direction.vertical:
				StartCoroutine(instantiateBullet(new Vector3(this.transform.position.x, this.transform.position.y+yDirAttack, 1)));
				break;
		}
		cptTurnBetweenAttack = maxTurnBetweenAttack;
	}

	public override IEnumerator instantiateBullet(Vector3 position){
		GameObject missile = Instantiate(bullet, position, Quaternion.identity) as GameObject;
		missile.GetComponent<Rocket> ().setVelocity (new Vector2 (xDirAttack, yDirAttack));
		missile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));
		missile.GetComponent<Rocket> ().setTireur (this);
		roquettestirés.Add (missile.GetComponent<Rocket>());
		yield return null;
	}
}
