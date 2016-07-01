using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<Player> ().loseHP ();
		}
	}
}
