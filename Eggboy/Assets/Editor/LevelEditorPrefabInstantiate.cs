using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[InitializeOnLoad]
public class LevelEditorPrefabInstantiate : Editor 
{
    static Transform m_LevelParent;
    static Transform LevelParent
    {
        get
        {
            if( m_LevelParent == null )
            {
                GameObject go = GameObject.Find( "LevelDesign" );//le GameObject ou l'on vas stocké nos gameobjects

                if( go != null )
                {
                    m_LevelParent = go.transform;
                }
            }

            return m_LevelParent;
        }
    }



    //Get or Set which Block is selected in our custom menu
    public static int SelectedBlock
    {
        get
        {
            return EditorPrefs.GetInt( "SelectedEditorBlock", 0 );
        }
        set
        {
            EditorPrefs.SetInt( "SelectedEditorBlock", value );
        }
    }

    static CategorieItemData categorieItem;
    static ItemData item;

    static LevelEditorPrefabInstantiate()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;

        //Make sure we load our block database. Notice the path here, which means the block database has to be in this specific location so we can find it
        //LoadAssetAtPath is a great way to load an asset from the project 
        categorieItem = AssetDatabase.LoadAssetAtPath<CategorieItemData>("Assets/Editor/CategoriesItemsData.asset");
        item = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Editor/ItemsData.asset");
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    static void OnSceneGUI( SceneView sceneView )
    {
        if( IsInCorrectLevel() == false )
        {
            return;
        }

        if(categorieItem == null )
        {
            return;
        }

        DrawCustomBlockButtons( sceneView );
        HandleLevelEditorPlacement();
    }

    static void HandleLevelEditorPlacement()
    {
        if( LevelEditorToolsMenu.SelectedTool == 0)
        {
            return;
        }

        if (LevelEditorToolsMenu.SelectedTool == 3)
        {
            if (Event.current.type == EventType.keyDown &&
                Event.current.keyCode == KeyCode.Escape)
            {
                LevelEditorToolsMenu.SelectedTool = 0;
            }
            return;
        }

        //This method is very similar to the one in . Only the AddBlock function is different

        //By creating a new ControlID here we can grab the mouse input to the SceneView and prevent Unitys default mouse handling from happening
        //FocusType.Passive means this control cannot receive keyboard input since we are only interested in mouse input
        int controlId = GUIUtility.GetControlID( FocusType.Passive );

        //If the left mouse is being clicked and no modifier buttons are being held
        if (Event.current.type == EventType.mouseDown &&
            Event.current.button == 1 &&
            Event.current.alt == false &&
            Event.current.shift == false &&
            Event.current.control == false)
        {
            if (LevelEditorCaseHandle.IsMouseInValidArea == true)
            {
                RemoveBlock(LevelEditorCaseHandle.CurrentHandlePosition);
            }
        }
        if ( (Event.current.type == EventType.mouseDown || Event.current.type == EventType.mouseDrag) &&
            Event.current.button == 0 &&
            Event.current.alt == false &&
            Event.current.shift == false &&
            Event.current.control == false )
        {
            if(LevelEditorCaseHandle.IsMouseInValidArea == true )
            {
                if( LevelEditorToolsMenu.SelectedTool == 1 )
                {
                    RemoveBlock( LevelEditorCaseHandle.CurrentHandlePosition );
                }

                if( LevelEditorToolsMenu.SelectedTool == 2 )
                {
                    if( SelectedBlock < item.Blocks.Count )
                    {
                        AddBlock(LevelEditorCaseHandle.CurrentHandlePosition, item.Blocks[ SelectedBlock ].Prefab );
                    }
                }
            }
        }

        //If we press escape we want to automatically deselect our own painting or erasing tools
        if( Event.current.type == EventType.keyDown &&
            Event.current.keyCode == KeyCode.Escape )
        {
            LevelEditorToolsMenu.SelectedTool = 0;
        }

        HandleUtility.AddDefaultControl( controlId );
    }

    //Draw a list of our custom blocks on the left side of the SceneView
    static void DrawCustomBlockButtons( SceneView sceneView )
    {
        if (LevelEditorToolsMenu.SelectedTool == 0)
        {
            return;
        }
        Handles.BeginGUI();
        GUI.Box( new Rect( 0, 0, 120, sceneView.position.height - 35 ), GUIContent.none, EditorStyles.textArea );
        DrawCustomCategorieButton();

        /*
        for ( int i = 0; i < categorieItem.Blocks.Count; ++i )
        {
            DrawCustomBlockButton( i, sceneView.position );
        }*/

        Handles.EndGUI();
    }


    public static int itemSelector = 0;
    public static int categorieSelector = 0;
    public static int selectedCategorie = 0;
    static void DrawCustomCategorieButton()
    {
        Item[] itemsDisplayed = item.Blocks.Where(t => t.CategorieName.ToString() == categorieItem.Blocks[categorieSelector].Name).ToArray();

        bool isActive = false;

        if (isActive == false && categorieSelector != selectedCategorie)
        {
            selectedCategorie = categorieSelector;
            itemSelector = 0;
            LevelEditorToolsMenu.SelectedTool = 2;
        }
        try
        {
            if (LevelEditorToolsMenu.SelectedTool == 2 && item.Blocks.FindIndex(a => a == itemsDisplayed[itemSelector]) == SelectedBlock)
            {
                isActive = true;
            }

        }
        catch (System.Exception)
        {
            isActive = true;
        }
        string[] categorieDisplay = new string[categorieItem.Blocks.Count];
        for (int i = 0; i < categorieItem.Blocks.Count; i++)
        {
            categorieDisplay[i] = categorieItem.Blocks[i].Name;
        }
        categorieSelector = GUI.SelectionGrid(new Rect(10, 10, 100, 20*categorieDisplay.Length), categorieSelector, categorieDisplay, 1);

        GUI.DrawTexture(new Rect(0, 20 * categorieDisplay.Length + 15, 120,2), AssetPreview.GetAssetPreview(item.Blocks[0].Prefab));

        itemsDisplayed = item.Blocks.Where(t => t.CategorieName.ToString() == categorieItem.Blocks[categorieSelector].Name).ToArray();
        GUIContent[] itemsDisplay = new GUIContent[itemsDisplayed.Count()];
        for (int j = 0; j < itemsDisplayed.Count(); j++)
        {
            itemsDisplay[j] = new GUIContent(AssetPreview.GetAssetPreview(itemsDisplayed[j].Prefab));
        }
        itemSelector = GUI.SelectionGrid(new Rect(10, 20 * categorieDisplay.Length + 20, 100, 50 * Mathf.Ceil(itemsDisplay.Length / 2f)), itemSelector, itemsDisplay, 2);



        if (isActive == false && item.Blocks.FindIndex(a => a == itemsDisplayed[itemSelector]) != SelectedBlock)
        {
            SelectedBlock = item.Blocks.FindIndex(a => a == itemsDisplayed[itemSelector]);
            LevelEditorToolsMenu.SelectedTool = 2;
        }
    }


    static void DrawCustomBlockButton( int index, Rect sceneViewRect )
    {
        /*
        bool isActive = false;

        if( LevelEditorToolsMenu.SelectedTool == 2 && index == SelectedBlock )
        {
            isActive = true;
        }
        return;*/
    //By passing a Prefab or GameObject into AssetPreview.GetAssetPreview you get a texture that shows this object
        /*Texture2D previewImage = AssetPreview.GetAssetPreview(categorieItem.Blocks[ index ].Prefab );
        GUIContent buttonContent = new GUIContent( previewImage );
        GUI.Label( new Rect( 5, index * 128 + 5, 100, 20 ), categorieItem.Blocks[ index ].Name );
        bool isToggleDown = GUI.Toggle( new Rect( 5 + (index%2)*60, index * 30 + 25 - (index % 2) * 30, 50, 50 ), isActive, buttonContent, GUI.skin.button );





        //If this button is clicked but it wasn't clicked before (ie. if the user has just pressed the button)
        if ( isToggleDown == true && isActive == false )
        {
            SelectedBlock = index;
            LevelEditorToolsMenu.SelectedTool = 2;
        }*/
    }

    public static void AddBlock( Vector3 position, GameObject prefab )
    {
        if( prefab == null )
        {
            return;
        }
        RemoveBlock(position);
        GameObject newCube = (GameObject)PrefabUtility.InstantiatePrefab( prefab );
        newCube.transform.parent = LevelParent;
        newCube.transform.position = position;

        //Make sure a proper Undo/Redo step is created. This is a special type for newly created objects
        Undo.RegisterCreatedObjectUndo( newCube, "Add " + prefab.name );

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }
    public static void RemoveBlock(Vector3 position)
    {
        for (int i = 0; i < LevelParent.childCount; ++i)
        {
            float distanceToBlock = Vector3.Distance(LevelParent.GetChild(i).transform.position, position);
            if (distanceToBlock < 0.1f)
            {
                //Use Undo.DestroyObjectImmediate to destroy the object and create a proper Undo/Redo step for it
                Undo.DestroyObjectImmediate(LevelParent.GetChild(i).gameObject);

                //Mark the scene as dirty so it is being saved the next time the user saves
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                return;
            }
        }
    }

    //I will use this type of function in many different classes. Basically this is useful to 
    //be able to draw different types of the editor only when you are in the correct scene so we
    //can have an easy to follow progression of the editor while hoping between the different scenes
    static bool IsInCorrectLevel()
    {
        return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name.Contains("Level");
    }
}
