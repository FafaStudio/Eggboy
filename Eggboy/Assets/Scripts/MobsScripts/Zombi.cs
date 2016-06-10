using UnityEngine;
using System.Collections;

public class Zombi : Enemy {

	protected override void OnCantMove (GameObject col)
	{
		skipMove = true;
		base.OnCantMove (col);
	}

	protected override bool Move (int xDir, int yDir, out RaycastHit2D hit)
	{
		skipMove = true;
		return base.Move (xDir, yDir, out hit);
	}
}
