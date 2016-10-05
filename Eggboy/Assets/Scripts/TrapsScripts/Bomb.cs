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
		} else {
			StartCoroutine (launchExplosion ());
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

		yield return null;

		this.GetComponent<SpriteRenderer> ().sprite = null;
		explosionIsLaunch = true;
	}

	public void resetAfterExplosion(){
		GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1, null);
		GameManager.instance.RemoveTrapToList (this);
		Destroy (this.gameObject);
	}

	public bool isExplosing(){
		return explosionIsLaunch;
	}
}
