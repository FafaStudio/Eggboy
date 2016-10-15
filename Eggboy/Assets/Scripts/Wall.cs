using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {
	
	private BoardManager board;

	void Start(){
		board = GameManager.instance.getCurrentBoard ();
		board.setObjectOnGrid((int)this.transform.position.x, (int)this.transform.position.y, -1, this.gameObject);
		if (this.gameObject.name == "Table") {
			board.setObjectOnGrid ((int)this.transform.position.x+1, (int)this.transform.position.y, -1, this.gameObject);
			board.setObjectOnGrid ((int)this.transform.position.x, (int)this.transform.position.y+1, -1, this.gameObject);
			board.setObjectOnGrid ((int)this.transform.position.x+1, (int)this.transform.position.y+1, -1, this.gameObject);
			board.setObjectOnGrid ((int)this.transform.position.x+1, (int)this.transform.position.y+2, -1, this.gameObject);
			board.setObjectOnGrid ((int)this.transform.position.x, (int)this.transform.position.y+2, -1, this.gameObject);
		}
	}
}

