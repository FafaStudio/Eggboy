using UnityEngine;
using System.Collections;

public class BombExplosion : MonoBehaviour {

	public void checkExplosion(){
		GameObject perso = GameManager.instance.getCurrentBoard ().testCaseCharacterPiege ((int)this.transform.position.x, (int)this.transform.position.y);
		if (perso == null) {
			return;
		}
		if (perso.tag == "Enemy") {
			perso.GetComponent<Enemy> ().Die ();
		}else if(perso.tag == "Player") {
			perso.GetComponent<Player> ().loseHP ();
		}
	}
}
