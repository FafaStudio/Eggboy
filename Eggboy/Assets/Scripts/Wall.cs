using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {
	
	private BoardManager board;

	void Start(){
		board = GameManager.instance.getCurrentBoard ();
		board.setNodeOnGrid ((int)this.transform.position.x, (int)this.transform.position.y, -1);
		if (this.gameObject.name == "Table") {
			board.setNodeOnGrid ((int)this.transform.position.x+1, (int)this.transform.position.y, -1);
			board.setNodeOnGrid ((int)this.transform.position.x, (int)this.transform.position.y+1, -1);
			board.setNodeOnGrid ((int)this.transform.position.x+1, (int)this.transform.position.y+1, -1);
			board.setNodeOnGrid ((int)this.transform.position.x+1, (int)this.transform.position.y+2, -1);
			board.setNodeOnGrid ((int)this.transform.position.x, (int)this.transform.position.y+2, -1);
		}
	}
}

