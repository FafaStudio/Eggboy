using UnityEngine;
using System.Collections;

public class Bomb : Trap {

    private int turnCount = 1;
	private bool explosionIsLaunch;
	public GameObject explosion;

	protected override void Start(){
		base.Start ();
		isEnclenched = true;
	}

    public override void doAction()
    {
		if (turnCount > 0) {
			turnCount--;
		} else if (!explosionIsLaunch) {
			explosionIsLaunch = true;
			StartCoroutine (launchExplosion ());
		} else {
			resetAfterExplosion ();
		}

    }
		

    public override void TriggerEnter(MovingObject col){
    }

	public override void TriggerExit(){
	}
		
    public override void declencherPiege()
    {
        isEnclenched = true;
    }

    public override void declencherPiegeNewTurn(){
    }

	public IEnumerator launchExplosion(){
		GameObject toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x, this.transform.position.y, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);
		toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x+1, this.transform.position.y, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);
		toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x-1, this.transform.position.y, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);

		toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x-1, this.transform.position.y+1, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);
		toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x, this.transform.position.y+1, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);
		toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x+1, this.transform.position.y+1, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);

		toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x-1, this.transform.position.y-1, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);
		toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x, this.transform.position.y-1, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);
		toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x+1, this.transform.position.y-1, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);

		yield return new WaitForSeconds (0.5f);

		this.GetComponent<SpriteRenderer> ().sprite = null;
		yield return null;
	}

	public void resetAfterExplosion(){
		GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1, null);
		GameManager.instance.RemoveTrapToList (this);
		Destroy (this.gameObject);
	}
}
