using UnityEngine;
using System.Collections;

public class Arrow : Trap {

    private Animator anim;
    private Player eggboy;

    public enum Direction {Nord, Est, Sud, Ouest};
    public Direction dir;

    public Sprite[] sprites;

    private GameManager manager;

    // Use this for initialization
    void Start () {
        base.Start();
        manager = GameManager.instance;
        setSprite();
    }

    public override void doAction()
    {
        return;
    }

	public override void TriggerEnter(MovingObject col){
		if ((col.gameObject.tag == "Player")&&(!isEnclenched)) // N'agit que si le joueur a finit son tour
        {
			isEnclenched = true;
            eggboy = col.gameObject.GetComponent<Player>();
            eggboy.setIsTrap(true);
            eggboy.piege = this;
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
			isEnclenched = false;
            eggboy.setIsTrap(false);
            eggboy.piege = null;
        }
    }
		
    public override void declencherPiege()
    {
        switch (dir)
        {
            case Direction.Nord: 
				eggboy.doMove(0, 1);
                break;
            case Direction.Est:
                eggboy.doMove(1, 0);
                break;
            case Direction.Sud:
                eggboy.doMove(0, -1);
                break;
            case Direction.Ouest:
                eggboy.doMove(-1, 0);
                break;
        }
        //eggboy.setIsUnderTrapEffect(false);
        //yield return null;
    }

    public void setSprite()
    {
        switch (dir)
        {
            case Direction.Nord:
                this.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case Direction.Est:
                this.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case Direction.Sud:
                this.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case Direction.Ouest:
                this.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
        }
    }
    public override void declencherPiegeNewTurn()
    {
    }
}
