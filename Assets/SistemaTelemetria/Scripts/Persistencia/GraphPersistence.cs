using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System;
using UnityEditor;
using System.Linq;
using static UnityEngine.GraphicsBuffer;
using System.Net.Security;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEditor.PackageManager.UI;

public enum GraphTypes { ACCUMULATED, NOTACCUMULATED, AVERAGE }
public enum Scaling { X_SCALING_START, X_SCALING_OFFSET, ONLY_Y }

[Serializable]
public struct GraphConfig
{
    [HideInInspector]
    public string name;
    [HideInInspector]
    public string eventX;
    [HideInInspector]
    public string eventY;
    [HideInInspector]
    public AnimationCurve myCurve;
    [HideInInspector]
    public GameObject window_graph;
    //Config del window_graph
    [HideInInspector]
    public float graph_Height;
    [HideInInspector]
    public float graph_Width;
    //Posición en X e Y
    [HideInInspector]
    public int graph_X;
    [HideInInspector]
    public int graph_Y;
    [HideInInspector]
    public int x_segments; // numero de separaciones que tiene el Eje X (Ademas es el numero de puntos que se representan en la grafica a la vez)
    [HideInInspector]
    public int y_segments; // numero de separaciones que tiene el Eje Y
    [HideInInspector]
    public Scaling scaling;
    [HideInInspector]
    public GraphTypes graphType;
}

public class GraphPersistence : IPersistence
{
    List<TrackerEvent> eventsBuff;

    public GameObject graphObject;

    //La configuracion inicial de todos los graphs desde el editor
    public GraphConfig[] graphsConfig;

    Window_Graph[] graphs;

    private void Start()
    {
        eventsBuff = new();

        // Crear un nuevo objeto Canvas
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<CanvasScaler>();              // Agregar el componente gráfico CanvasScaler al objeto Canvas
        canvasObject.AddComponent<GraphicRaycaster>();          // Agregar el componente gráfico GraphicRaycaster al objeto Canvas
        canvasObject.transform.SetParent(transform, false);     // Hacer que el objeto Canvas sea hijo del objeto padre
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        canvasObject.GetComponent<Canvas>().worldCamera = Camera.main;
        canvasObject.GetComponent<Canvas>().scaleFactor = 0.8f;  //!CUIDAO

        //Crear tantos Graph como se han configurado y pasarles la información
        Array.Resize(ref graphs, graphsConfig.Count());
        for (int i = 0; i < graphsConfig.Count(); ++i)
        {
            GameObject aux = Instantiate(graphObject, parent: canvasObject.transform);
            aux.GetComponent<RectTransform>().offsetMax = new Vector2(graphsConfig[i].graph_X, graphsConfig[i].graph_Y);
            graphs[i] = aux.GetComponent<Window_Graph>();
            graphs[i].name = graphsConfig[i].name;
            graphs[i].SetConfig(graphsConfig[i]);
        }
    }

    private void Update()
    {
    }
    private void OnDestroy()
    {

    }
    public override void Send(TrackerEvent e)
    {
        //eventsBuff.Add(e);
        for (int i = 0; i < graphs.Length; ++i)
        {
            graphs[i].ReceiveEvent(e);
        }
    }

    public override void Flush()
    {
        //List<TrackerEvent> events = new List<TrackerEvent>(eventsBuff);
        //eventsBuff.Clear();
        //Write(events);
    }

}

[CustomEditor(typeof(GraphPersistence))]
public class GraphPersistenceEditor : Editor
{
    SerializedProperty grPers;

    void OnEnable()
    {
        grPers = serializedObject.FindProperty("graphsConfig");
    }

    void OnValidate()
    {
        
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //EditorUtility.SetDirty(target);
        EditorGUILayout.PropertyField(grPers);

        GraphPersistence graphPersistence = (GraphPersistence)target;

        //Obtén los nombres de los eventos del script TrackerConfig
        TrackerConfig trackerConfig = graphPersistence.gameObject.GetComponent<TrackerConfig>();
        List<string> eventNames = new List<string>();
        foreach (TrackerConfig.EventConfig config in trackerConfig.eventConfig)
        {
            eventNames.Add(config.eventName);
        }

        EditorGUI.BeginChangeCheck();
        // Crea un menú dropdown para cada elemento de graphs
        for (int i = 0; i < graphPersistence.graphsConfig.Length; i++)
        {
            graphPersistence.graphsConfig[i].name = EditorGUILayout.TextField("GraphName", graphPersistence.graphsConfig[i].name);

            graphPersistence.graphsConfig[i].myCurve = EditorGUILayout.CurveField(graphPersistence.graphsConfig[i].myCurve);
            EditorGUILayout.Space(5);
            
            //Variables de escala
            graphPersistence.graphsConfig[i].graphType = (GraphTypes)EditorGUILayout.EnumPopup("GraphType", graphPersistence.graphsConfig[i].graphType);
            graphPersistence.graphsConfig[i].scaling = (Scaling)EditorGUILayout.EnumPopup("Scaling", graphPersistence.graphsConfig[i].scaling);

            // Crea un menú popup con los nombres de los eventos
            int selectedEventIndex;
            selectedEventIndex = EditorGUILayout.Popup("Select Event X", eventNames.IndexOf(graphPersistence.graphsConfig[i].eventX), eventNames.ToArray());
            if (selectedEventIndex != -1 && graphPersistence.graphsConfig[i].eventX != eventNames[selectedEventIndex])
                graphPersistence.graphsConfig[i].eventX = eventNames[selectedEventIndex];
            
            selectedEventIndex = EditorGUILayout.Popup("Select Event Y", eventNames.IndexOf(graphPersistence.graphsConfig[i].eventY), eventNames.ToArray());
            if (selectedEventIndex != -1 && graphPersistence.graphsConfig[i].eventY != eventNames[selectedEventIndex])
                graphPersistence.graphsConfig[i].eventY = eventNames[selectedEventIndex];

            //El resto de configuracion
            graphPersistence.graphsConfig[i].graph_Height = EditorGUILayout.FloatField("Graph_Height", graphPersistence.graphsConfig[i].graph_Height);
            graphPersistence.graphsConfig[i].graph_Width = EditorGUILayout.FloatField("Graph_Width", graphPersistence.graphsConfig[i].graph_Width);

            graphPersistence.graphsConfig[i].graph_X = EditorGUILayout.IntField("X_Pos", graphPersistence.graphsConfig[i].graph_X);
            graphPersistence.graphsConfig[i].graph_Y = EditorGUILayout.IntField("Y_Pos", graphPersistence.graphsConfig[i].graph_Y);

            graphPersistence.graphsConfig[i].x_segments = EditorGUILayout.IntField("X_segments", graphPersistence.graphsConfig[i].x_segments);
            graphPersistence.graphsConfig[i].y_segments = EditorGUILayout.IntField("Y_segments", graphPersistence.graphsConfig[i].y_segments);

            EditorGUILayout.Space(20);
        }
        EditorGUILayout.Space();
        graphPersistence.graphObject = EditorGUILayout.ObjectField("Graph Object", graphPersistence.graphObject, typeof(GameObject), false) as GameObject;

        if (EditorGUI.EndChangeCheck())        
            EditorUtility.SetDirty(target);
        
        // Guarda los cambios realizados en el editor
        serializedObject.ApplyModifiedProperties();
    }
}
