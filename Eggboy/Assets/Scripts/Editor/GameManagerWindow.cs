using UnityEngine;
using UnityEditor;
using System.Collections;

public class GameManagerWindow : EditorWindow
{

    Achiever achieverScript;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("FafaStudio/GameManager")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one. 
        EditorWindow.GetWindow(typeof(GameManagerWindow));
    }
    Texture circle;
    GameManager gameManager;

    private void setColor(bool boolean)
    {
        if (boolean)
        {
            GUI.color = Color.green;
        }
        else
        {
            GUI.color = Color.red;
        }
    }


    bool pauseplayerTurn;
    bool pausetrapActioning;
    bool pauseenemiesMoving;
    bool pauserocketsMoving;

    bool lastplayersTurn;
    bool lasttrapActioning;
    bool lastenemiesMoving;
    bool lastrocketsMoving;

    void OnGUI()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            return;
        }
        EditorGUIUtility.labelWidth = 100;

        circle = AssetDatabase.LoadAssetAtPath("Assets/ScoreLib/Sprites/Voyant.png", typeof(Texture)) as Texture;
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("playersTurn", EditorStyles.boldLabel, GUILayout.Width(110));
        setColor(gameManager.playersTurn);
        Rect rectTexture1 = new Rect(new Vector2(115, 0 * 18 + 2), new Vector2(15, 15));
        GUI.DrawTexture(rectTexture1, circle, ScaleMode.ScaleToFit, true);//52 est parfait je suppose 50+1+1 donc height+margehaute+margebasse
        GUI.color = Color.white;
        GUILayout.Space(20);
        pauseplayerTurn = EditorGUILayout.Toggle("pauseOnChange", pauseplayerTurn);
        if (pauseplayerTurn && gameManager.playersTurn != lastplayersTurn)
        {
            Debug.Break();
        }
        lastplayersTurn = gameManager.playersTurn;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("trapActioning", EditorStyles.boldLabel, GUILayout.Width(110));
        setColor(gameManager.trapActioning);
        Rect rectTexture2 = new Rect(new Vector2(115, 1 * 18 + 2), new Vector2(15, 15));
        GUI.DrawTexture(rectTexture2, circle, ScaleMode.ScaleToFit, true);//52 est parfait je suppose 50+1+1 donc height+margehaute+margebasse
        GUI.color = Color.white;
        GUILayout.Space(20);
        pausetrapActioning = EditorGUILayout.Toggle("pauseOnChange", pausetrapActioning);
        if (pausetrapActioning && gameManager.trapActioning != lasttrapActioning)
        {
            Debug.Break();
        }
        lasttrapActioning = gameManager.trapActioning;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("enemiesMoving", EditorStyles.boldLabel, GUILayout.Width(110));
        setColor(gameManager.enemiesMoving);
        Rect rectTexture3 = new Rect(new Vector2(115, 2 * 18 + 2), new Vector2(15, 15));
        GUI.DrawTexture(rectTexture3, circle, ScaleMode.ScaleToFit, true);//52 est parfait je suppose 50+1+1 donc height+margehaute+margebasse
        GUI.color = Color.white;
        GUILayout.Space(20);
        pauseenemiesMoving = EditorGUILayout.Toggle("pauseOnChange", pauseenemiesMoving);
        if (pauseenemiesMoving && gameManager.enemiesMoving != lastenemiesMoving)
        {
            Debug.Break();
        }
        lastenemiesMoving = gameManager.enemiesMoving;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("rocketsMoving", EditorStyles.boldLabel, GUILayout.Width(110));
        setColor(gameManager.rocketsMoving);
        Rect rectTexture4 = new Rect(new Vector2(115, 3 * 18 + 2), new Vector2(15, 15));
        GUI.DrawTexture(rectTexture4, circle, ScaleMode.ScaleToFit, true);//52 est parfait je suppose 50+1+1 donc height+margehaute+margebasse
        GUI.color = Color.white;
        GUILayout.Space(20);
        pauserocketsMoving = EditorGUILayout.Toggle("pauseOnChange", pauserocketsMoving);
        if (pauserocketsMoving && gameManager.rocketsMoving != lastrocketsMoving)
        {
            Debug.Break();
        }
        lastrocketsMoving = gameManager.rocketsMoving;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    void awake()
    {
        pauseplayerTurn = false;
        pausetrapActioning = false;
        pauseenemiesMoving = false;
        pauserocketsMoving = false;
    }

    void Update()
    {
        Repaint();
    }
}
