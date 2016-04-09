using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {
	
	private BoardManager board;
	
	
	void Awake ()
	{
	}

	void Start(){
		board = GameManager.instance.getCurrentBoard ();
		board.setCellOnGrid ((int)this.transform.position.x, (int)this.transform.position.y, -1);
	}
}

