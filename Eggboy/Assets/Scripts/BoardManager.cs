using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class toInstantiate
	{
		public GameObject objectInstantiate;
		public int x;
		public int y;
	}

	public int columns = 15;
	public int rows = 8;

	public GameObject[] floorTiles;
	public toInstantiate[] objectTiles;



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
				GameObject instantiate = Instantiate(toInstantiate, new Vector3(x,y,0f),Quaternion.identity ) as GameObject;
				instantiate.transform.SetParent(boardHolder);
			}
		}
	}

	void InitialiseLevelDesign(){
		for(int i = 0; i < objectTiles.Length; i++){
			Vector3 position = new Vector3 (objectTiles [i].x, objectTiles [i].y, 0f);
			Instantiate (objectTiles [i].objectInstantiate, position, Quaternion.identity);
		}
	}

	public void SetupScene(int level){
		boardSetup ();
		InitialiseList ();
		InitialiseLevelDesign ();
	}
}
