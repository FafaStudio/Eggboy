using UnityEngine;
using System.Collections;
using System;


public class Trigger : Trap {

    private Player eggboy;
    public Trap cible;

    public override void doAction()
    {
        return;
    }

    public override void TriggerEnter(MovingObject col)
    {
        if ((col.gameObject.tag == "Player") && (!isEnclenched)) // N'agit que si le joueur a finit son tour
        {
            isEnclenched = true;
            eggboy = col.gameObject.GetComponent<Player>();
            cible.doAction();
        }
        /*
        if ((col.gameObject.tag == "Player") && (!isEnclenched)) // N'agit que si le joueur a finit son tour
        {
            isEnclenched = true;
            eggboy = col.gameObject.GetComponent<Player>();
            eggboy.setIsTrap(true);
            eggboy.piege = this;
        }*/
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            isEnclenched = false;
        }
    }

    public override void declencherPiege()
    {
    }

    public override void declencherPiegeNewTurn()
    {
    }

    void OnDrawGizmos()
    {
        if (cible == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
            Gizmos.DrawWireCube(transform.position, new Vector3(0.97f, 0.97f, 1));
            Gizmos.DrawWireCube(transform.position, new Vector3(0.93f, 0.93f, 1));
        }

    }
    void OnDrawGizmosSelected()
    {
        if (cible != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, cible.transform.position);
        }
    }
}
