using UnityEngine;
using System.Collections;

public class Camuflaje : Trap {

    public GameObject piegeCamoufle;

	// Use this for initialization
	protected override void Start () {
        base.Start();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void TriggerEnter(MovingObject col)
    {
        if ((col.gameObject.GetComponent<MovingObject>() != null) && (!isEnclenched)) // N'agit que si le joueur a finit son tour
        {
            isEnclenched = true;
            character = col;
            character.setIsTrap(true);
            character.piege = this;
        }
    }

    public override void declencherPiege()
    {
        GameObject toInstantiate = Instantiate(piegeCamoufle, new Vector3((int)this.transform.position.x, (int)this.transform.position.y, 0), Quaternion.identity) as GameObject;
        TriggerExit();
        toInstantiate.GetComponent<Trap>().TriggerEnter(character);
        GameManager.instance.RemoveTrapToList(this);
        Destroy(this.gameObject);


    }

    public override void doAction()
    {
        return;
    }

    public override void TriggerExit()
    {
        isEnclenched = false;
        if (character != null)
        {
            character.setIsTrap(false);
            character.setIsUnderTrapEffect(false);
            character.piege = null;
        }
    }

    public override void declencherPiegeNewTurn(){}

	public override void boutonDeclenchement (){}
}
