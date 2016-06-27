using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad]
public class LevelEditorCaseHandle : Editor {
    public static Vector2 CurrentHandlePosition = Vector2.zero;
    public static bool IsMouseInValidArea = false;

    static Vector2 m_OldHandlePosition = Vector2.zero;


    static LevelEditorCaseHandle()
    {
        //The OnSceneGUI delegate is called every time the SceneView is redrawn and allows you
        //to draw GUI elements into the SceneView to create in editor functionality
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    static void OnSceneGUI(SceneView sceneView)
    {
        bool isLevelEditorEnabled = EditorPrefs.GetBool("IsLevelEditorEnabled", true);

        //Ignore this. I am using this because when the scene GameE06 is opened we haven't yet defined any On/Off buttons
        //for the cube handles. That comes later in E07. This way we are forcing the cube handles state to On in this scene
        /*{
            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name != "GameE06") //!!! A supprimer a terme
            {
                isLevelEditorEnabled = true;
            }
        }*/

        if (isLevelEditorEnabled == false)
        {
            return;
        }

        UpdateHandlePosition();
        UpdateIsMouseInValidArea(sceneView.position);
        UpdateRepaint();

        DrawCubeDrawPreview();
    }
    static void UpdateIsMouseInValidArea(Rect sceneViewRect)
    {
        //Make sure the cube handle is only drawn when the mouse is within a position that we want
        //In this case we simply hide the cube cursor when the mouse is hovering over custom GUI elements in the lower
        //are of the sceneView which we will create in E07
        bool isInValidArea = Event.current.mousePosition.y < sceneViewRect.height - 35;//!!!! A modifier a terme

        if (isInValidArea != IsMouseInValidArea)
        {
            IsMouseInValidArea = isInValidArea;
            SceneView.RepaintAll();
        }
    }

    static void UpdateHandlePosition()
    {
        if (Event.current == null)
        {
            return;
        }

        Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, 1 << LayerMask.NameToLayer("LevelFloor"));
        if (hit == true)
        {
            Vector2 offset = Vector2.zero;
            if (EditorPrefs.GetBool("SelectBlockNextToMousePosition", true) == true)
            {
                offset = hit.normal;
            }

            CurrentHandlePosition.x = Mathf.Round(hit.point.x - hit.normal.x * 0.001f + offset.x);
            CurrentHandlePosition.y = Mathf.Round(hit.point.y - hit.normal.y * 0.001f + offset.y);
        }
    }

    static void UpdateRepaint()
    {
        //If the cube handle position has changed, repaint the scene
        if (CurrentHandlePosition != m_OldHandlePosition)
        {
            SceneView.RepaintAll();
            m_OldHandlePosition = CurrentHandlePosition;
        }
    }

    static void DrawCubeDrawPreview()
    {
        if (IsMouseInValidArea == false)
        {
            return;
        }

        Handles.color = new Color(EditorPrefs.GetFloat("CubeHandleColorR", 1f), EditorPrefs.GetFloat("CubeHandleColorG", 1f), EditorPrefs.GetFloat("CubeHandleColorB", 0f));

        DrawHandlesCube(CurrentHandlePosition);
    }

    static void DrawHandlesCube(Vector3 center)
    {
        Vector3 p1 = center + Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;
        Vector3 p2 = center + Vector3.up * 0.5f - Vector3.right * 0.5f + Vector3.forward * 0.5f;
        Vector3 p3 = center - Vector3.up * 0.5f - Vector3.right * 0.5f + Vector3.forward * 0.5f;
        Vector3 p4 = center - Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;

        //You can use Handles to draw 3d objects into the SceneView. If defined properly the
        //user can even interact with the handles. For example Unitys move tool is implemented using Handles
        //However here we simply draw a cube that the 3D position the mouse is pointing to

        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);
        Handles.DrawLine(p3, p4);
        Handles.DrawLine(p4, p1);
    }
}
