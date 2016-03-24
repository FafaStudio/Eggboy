using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count ( int min, int max ) 
		{
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 15;
	public int rows = 8;

	public GameObject[] floorTiles;
	public GameObject[] objectTiles;



	private Transform boardHolder;
	//boardHolder permet lorsqu'on fait spawn plein de trucs de garder la hierarchie clean
	private List <Vector3> gridPositions = new List<Vector3>();

	void InitialiseList(){
		gridPositions.Clear ();
		for (int x =0; x < columns; x++) {
			for(int y = 0; y < rows; y++){
				gridPositions.Add(new Vector3(x, y, 0f));
			}
		}
	}

	void boardSetup(){
		boardHolder = new GameObject ("Board").transform;
		for (int x = 0; x < columns; x++) {
			for(int y = 0; y < rows; y++){
				GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
				// on instancie un floortiles choisi en random dans la liste des floortiles
				GameObject instantiate = Instantiate(toInstantiate, new Vector3(x,y,0f),Quaternion.identity ) as GameObject;
				// on définie un gameObject qui va nous permettre d'instancier les objets choisis au préalable par toInstantiate
				instantiate.transform.SetParent(boardHolder);
				// boardHolder est parent ( dans la hierarchie ) des objets instanciés 
			}
		}
	}

	Vector3 RandomPosition(){
	// determine une position Random parmis la grille gridPosition
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum){
		int objectCount = Random.Range (minimum, maximum+1);
		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}

	public void SetupScene(int level){
		boardSetup ();
		InitialiseList ();
		int enemyCount = level;
		LayoutObjectAtRandom (objectTiles, enemyCount, enemyCount);
	}
}
