using UnityEngine;
using System.Collections;

public class Bomb : Trap {

    private Player eggboy;
    private int turnCount = 2;

    public override void doAction()
    {
        if (turnCount > 0)
        {
            turnCount--;
        }
        else
        {
            //on explose
            Destroy(gameObject);
        }

    }

    public override void TriggerEnter(MovingObject col)
    {

    }

	public override void TriggerExit()
	{

	}


    void OnTriggerExit2D(Collider2D col)
    {

    }

    public override void declencherPiege()
    {
        isEnclenched = true;
    }

    public override void declencherPiegeNewTurn()
    {

    }
}
