using UnityEngine;
using System.Collections;

public class Marecage : Trap {

	private Player eggboy;

	public override void doAction()
	{
		return;
	}

	void OnTriggerEnter2D(Collider2D col)// Appelé à chaque frame a partir du moment ou une collision est la
	{
		if (col.gameObject.tag == "Player") // N'agit que si le joueur a finit son tour
		{
			eggboy = col.gameObject.GetComponent<Player>();
			eggboy.setIsTrap(true);
			eggboy.piege = this;
		}
	}


	void OnTriggerExit2D(Collider2D col)
	{

		if (col.gameObject.tag == "Player")
		{
			eggboy.setIsTrap(false);
			eggboy.piege = null;
		}
	}

	public override IEnumerator declencherPiege()
	{
		yield return null;
	}
}
