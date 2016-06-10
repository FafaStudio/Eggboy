using UnityEngine;
using System.Collections;

public class Necromancer : Enemy {

	public GameObject zombieRat;
	protected int cptZombie = 0;
	public int maxZombie = 3;
	private GameObject[] zombieSpawned;

	private bool hasSpawnedLastTurn = false;


	protected override void Start ()
	{
		zombieSpawned = new GameObject[maxZombie];
		animator = null;
		base.Start ();
	}

	public override void MoveEnemy ()
	{
		if (isDead)
			return;

		int xDir = 0;
		int yDir = 0;

		BoardManager.Node currentPos = new BoardManager.Node (1, new Vector2 (this.transform.position.x, this.transform.position.y));
		currentPos.distanceVO = currentPos.volDoiseau (target.GetComponent<Player>().caseExacte.position);
		if (currentPos.distanceVO < 4) {
			//	Vector2 nextPosition = GameManager.instance.getCurrentBoard().doPathfinding(target.GetComponent<Player>().caseExacte, currentPos);

			//If the difference in positions is approximately zero (Epsilon) do the following:
			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				yDir = target.position.y > transform.position.y ? -1 : 1;
			else
				xDir = target.position.x > transform.position.x ? -1 : 1;
			hasSpawnedLastTurn = false;
			AttemptMove (xDir, yDir);
		} else if (!hasSpawnedLastTurn) {
			Vector2 spawnPosition = GameManager.instance.getCurrentBoard ().doPathfinding (target.GetComponent<Player> ().caseExacte, currentPos);
			spawnZombie ((int)spawnPosition.x, (int)spawnPosition.y);
		} else
			hasSpawnedLastTurn = false;
	}

	public void spawnZombie(int xDir, int yDir){
		updateCompteurZombie ();
		if (cptZombie < zombieSpawned.Length) {
			GameObject toInstantiate = Instantiate (zombieRat, new Vector3 (xDir, yDir, 0f), Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<Enemy> ().skipMove = true;
			addZombieToSpawnedZomb(toInstantiate);
			hasSpawnedLastTurn = true;
		} else
			return;
	}

	public void addZombieToSpawnedZomb(GameObject zombie){
		for (int i = 0; i < zombieSpawned.Length; i++) {
			if (zombieSpawned [i] == null) {
				zombieSpawned [i] = zombie;
				return;
			}
		}
	}

	public void updateCompteurZombie(){
		cptZombie = zombieSpawned.Length;
		for (int i = 0; i < zombieSpawned.Length; i++) {
			if (zombieSpawned [i] == null) {
				cptZombie--;
			} else
				print (zombieSpawned [i].name);
		}
	}

	protected override void OnCantMove (GameObject col)
	{
		if (col.gameObject.tag == "Wall") {
			return;
		} else if (col.gameObject.tag == "Player") {
			return;
		}
	}
}
