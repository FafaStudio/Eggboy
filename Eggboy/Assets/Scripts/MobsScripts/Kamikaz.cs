using UnityEngine;
using System.Collections;

public class Kamikaz : Enemy {
	
	public int turnBeforeExplosion;
	public GameObject bombExplosion;

	UICompteur compteur;

	protected override void Start (){
		compteur = this.GetComponent<UICompteur> ();
		base.Start ();
	}

	public override void MoveEnemy ()
	{
		if (isDead)
			return;
		if (turnBeforeExplosion > 0)
			turnBeforeExplosion--;
		else {
			launchExplosion ();
			return;
		}
		base.MoveEnemy ();
		if (isPlayerInRange ())
			turnBeforeExplosion = 0;
	}

	public void launchExplosion(){
		bombExplosion = Instantiate (bombExplosion, this.transform.parent, true) as GameObject;
		bombExplosion.transform.position = this.transform.position;
		bombExplosion.GetComponent<Bomb> ().setCompteur (0);
		bombExplosion.GetComponent<Bomb> ().launchExplosion ();
	}

	protected override IEnumerator OnCantMove (){
		endTurnEnemy = true;
		if (isTrap) {
			isTrap = false;
		}
		if (blockingObject.tag == "Wall") {
			blockingObject = null;
			yield return null;
		} else if (blockingObject.tag == "Player") {
			yield return null;
		} else {
			blockingObject = null;
		}
	}

	public bool isPlayerInRange(){
		if (this.caseExacte.volDoiseau (target.position) == 1)
			return true;
		else
			return false;
	}

	void Update(){
		if (!GameManager.instance.isInfoUI) {
			compteur.disactiveUI ();
			compteur.setInformation ("0");
		}
		else {
			compteur.activeUI ();
			compteur.setInformation(((turnBeforeExplosion+1).ToString ()));
		}
	}
}
