using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RequestAndOrderInspector : EditorWindow

{
    public Dictionary<SerfOrder, bool> foldoutState = new Dictionary<SerfOrder, bool>();
    private int tab;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/RequestsAndOrders")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(RequestAndOrderInspector));
    }

    private void OnGUI()
    {
        var go = GameObject.Find("Grass");
        if (go != null && Application.isPlaying)
        {
            var gameManager = go.GetComponent<GameManager>();

            tab = GUILayout.Toolbar(tab, new[] {"Current orders", "Completed orders", "TODO"});
            switch (tab)
            {
                case 0:
                    EditorGUILayout.LabelField("Current orders:", gameManager.GetCurrentSerfOrders().Count.ToString());
                    foreach (var serfOrder in gameManager.GetCurrentSerfOrders())
                    {
                        DrawOrder(serfOrder);
                    }
                    break;
                case 1:
                    EditorGUILayout.LabelField("Completed orders:", gameManager.CompletedOrders.Count.ToString());
                    foreach (var serfOrder in gameManager.CompletedOrders)
                    {
                        DrawOrder(serfOrder);
                    }
                    break;
                case 2:
                    break;
            }
        }
        else
        {
            GUILayout.Label("Start the game for requests info", EditorStyles.boldLabel);
        }
    }

    private void DrawOrder(SerfOrder serfOrder)
    {
        foldoutState.TryGetValue(serfOrder, out var foldout);
        foldoutState[serfOrder] = EditorGUILayout.Foldout(foldout, serfOrder.ItemType.ToString());
        if (foldout)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Status: {serfOrder.Status}");

            GUILayout.Label("From:");
            serfOrder.From.GameObject = EditorGUILayout.ObjectField(serfOrder.From.GameObject, typeof(GameObject), true) as GameObject;

            GUILayout.Label("To:");
            serfOrder.To.GameObject = EditorGUILayout.ObjectField(serfOrder.To.GameObject, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button($"serf: {serfOrder.Assignee}"))
            {
                var activeGameObject = serfOrder.Assignee;
                Selection.activeGameObject = activeGameObject.gameObject;
                SceneView.FrameLastActiveSceneView();
            }

            GUILayout.Label($"Created: {serfOrder.TimeCreated}");
            GUILayout.EndHorizontal();
        }
    }

    public void OnInspectorUpdate()
    {
        // This will only get called 10 times per second.
        Repaint();
    }
}