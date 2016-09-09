using UnityEngine;
using System.Collections;

public class FlameThrower : Trap {
    /*
     *Ne pas oublier de cocher isEnclenched lors de la création de la préfab 
     * 
    */
    private Player eggboy;

    public bool automate = true;
    private int TurnCount = 2;
    private int coolDownCount = 2;

    public override void doAction()
    {
        if (automate)//Ne pas oublier de caucher 
        {
            if (TurnCount > 0)//!!!!!!!!!!!!!AJOUTER LES ANIM DE FLAMMES
            {
                TurnCount--;
                //On lance des flames
            }
            else if (coolDownCount > 0)
            {
                coolDownCount--;//on attend
            }
            else//on reset
            {
                TurnCount = 2;
                coolDownCount = 2;
            }
        }
        else
        {

        }
    }

    public override void TriggerEnter(MovingObject col)
    {
        return;
    }


    void OnTriggerExit2D(Collider2D col)
    {
        return;
    }

    public override void declencherPiege()
    {
        isEnclenched = !isEnclenched;
    }

    public override void declencherPiegeNewTurn()
    {

    }
}
