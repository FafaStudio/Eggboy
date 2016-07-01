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

        /*if (isActioning)
        {
            //anim.SetBool("isActioning", false);
            isActioning = false;
            if (isPlayer)
            {
                isEnclenched = true;
            }
        }

        else if (isEnclenched)
        {
            //anim.SetBool("isActioning", true);
            isActioning = true;
            isEnclenched = false;
            if (isPlayer)
            {
                print("AI");
                isEnclenched = true;
            }
        }*/


    }

    void OnTriggerEnter2D(Collider2D col)// Appelé à chaque frame a partir du moment ou une collision est la
    {
        if (col.gameObject.tag == "Player") // N'agit que si le joueur a finit son tour
        {
            eggboy = col.gameObject.GetComponent<Player>();
            eggboy.setIsTrap(true);
            eggboy.piege = this;
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        
        if (col.gameObject.tag == "Player")
        {
            eggboy.setIsTrap(false);
            eggboy.piege = null;
        }
    }


    // Update is called once per frame
    void Update () {
	
	}

    public override IEnumerator declencherPiege()
    {
        eggboy.enabled = false;
        switch (dir)
        {

            case Direction.Nord: eggboy.doMove(0, 1);
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
            
            
            
        yield return null;
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
}
