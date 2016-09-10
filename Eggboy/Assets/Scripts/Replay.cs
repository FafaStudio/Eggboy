using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Replay : MonoBehaviour {
    [HideInInspector]//si je le met private ça marche pas je sais pas pk mais bon x) du coup public mais caché
    public string specifiedFileName;
    [HideInInspector]
    public int gameSeed;
    public List<int> actionsParTours;
    [SerializeField]
    public List<int> actionsParToursActualGame;
    /*  0 = Space
     *  1 = Left
     *  2 = Right
     *  3 = Bottom
     *  4 = Top
     */
    public bool debugMod = false;


    void Awake()
    {
        if (debugMod)
        {
            read();
            gameObject.GetComponent<GameInformations>().gameSeed = gameSeed;
        }
        actionsParToursActualGame = new List<int>();
    }
    public void addAction(int actionId)
    {
        if (!debugMod)
        {
            actionsParToursActualGame.Add(actionId);
        }
    }

    public void changeAction(int actionId, int actionTurn)
    {
        if(actionsParToursActualGame.Count < actionTurn)
        {
            actionsParToursActualGame.Add(actionId);
        }
        else
        {
            actionsParToursActualGame[actionTurn] = actionId;
        }
    }
    public void save()
    {
        if (!debugMod)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Create(Application.dataPath + "/DebugData/" + System.DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss--") + gameObject.GetComponent<GameInformations>().gameSeed +".txt");
            //FileStream fs = File.Create(Application.dataPath + "/DebugData/" + System.Guid.NewGuid().ToString() + ".txt");
            bf.Serialize(fs, actionsParToursActualGame);
            fs.Close();
        }
    }

    public void read()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.OpenRead(Application.dataPath + "/DebugData/" + specifiedFileName + ".txt");
        actionsParTours = (List<int>)bf.Deserialize(fs);
        fs.Close();
    }
    void OnApplicationQuit()
    {
        save();
    }

    public void setSpecifiedFileName(string name)
    {
        this.specifiedFileName = name;
    }
}
