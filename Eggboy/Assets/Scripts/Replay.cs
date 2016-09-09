using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Replay : MonoBehaviour {
    public bool debugMod = false;
    public string specifiedFileName;
    public List<int> actionsParTours;
    [SerializeField]
    public List<int> actionsParToursActualGame;
    /*  0 = Space
     *  1 = Left
     *  2 = Right
     *  3 = Bottom
     *  4 = Top
     */
     

    void Awake()
    {
        if (debugMod)
        {
            read();
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
    public void save()
    {
        if (!debugMod)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Create(Application.dataPath + "/DebugData/" + System.Guid.NewGuid().ToString() + ".txt");
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
}
