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

	public class Grid
	{
		public int valeur = 0;

		public Grid(int x){
			this.valeur = x;
		}
		public void setValeur(int value){
			this.valeur = value;
		}

		public String toString(){
			return " [" + this.valeur.ToString () + "] ";
		}

	}

	public int columns = 15;
	public int rows = 8;

	public GameObject[] floorTiles;
	public toInstantiate[] objectTiles;

	private Transform boardHolder;
	//boardHolder permet lorsqu'on fait spawn plein de trucs de garder la hierarchie clean
	private Transform objectInLevel;

	private Grid[,] gridPositions;


	void InitialiseList(){
		gridPositions = new Grid[columns, rows];
		for (int x =0; x < columns; x++) {
			for(int y = 0; y < rows; y++){
				gridPositions[x,y] = new Grid(1);
			/*	if (x == columns - 1)
					print (gridPositions [x, y].toString () + "/n");
				else
					print (gridPositions [x, y].toString ());*/
			}
		}
	}

	public void setCellOnGrid(int x, int y , int value){
		gridPositions [x, y].setValeur(value);
	}

	void boardSetup(){
		boardHolder = new GameObject ("Board").transform;
		for (int x = 0; x < columns; x++) {
			for(int y = 0; y < rows; y++){
				GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
				/*GameObject toInstantiate;
				if (y % 2 != 0) {
					if ((x == 0) || (x % 2 == 0)) {
						toInstantiate = floorTiles [0];
					} else
						toInstantiate = floorTiles [1];
				} else {
					if ((x == 0) || (x % 2 == 0)) {
						toInstantiate = floorTiles [1];
					} else
						toInstantiate = floorTiles [0];
				}*/
				GameObject instantiate = Instantiate(toInstantiate, new Vector3(x,y,0f),Quaternion.identity ) as GameObject;
				instantiate.transform.SetParent(boardHolder);
			}
		}
	}

	void InitialiseLevelDesign(){
		objectInLevel = new GameObject ("ObjectInLevel").transform;
		for(int i = 0; i < objectTiles.Length; i++){
			Vector3 position = new Vector3 (objectTiles [i].x, objectTiles [i].y, 0f);
			GameObject instantiate = Instantiate (objectTiles [i].objectInstantiate, position, Quaternion.identity) as GameObject;
			instantiate.transform.SetParent (objectInLevel);
		}
	}

	public void SetupScene(int level){
		boardSetup ();
		InitialiseList ();
		InitialiseLevelDesign ();
	}
		
}
