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

    private string baseSaveRoute = "Trazas\\Graphs\\";
    private Dictionary<string, StreamWriter> graphWriters;

    private void Start()
    {
        eventsBuff = new();
        graphWriters = new Dictionary<string, StreamWriter>();

        // Crear la carpeta donde se guuardarán los archivos que contienen los datos con los puntos de la gráfica
        string id = Tracker.instance.getSessionId().ToString();
        string fullRoute = baseSaveRoute + id + "\\";
        Directory.CreateDirectory(fullRoute);

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
            // Crear el archivo en el que se guardarán los puntos en formato de texto
            graphWriters.Add(graphsConfig[i].name, new StreamWriter(fullRoute + graphsConfig[i].name + ".csv"));
            graphWriters[graphsConfig[i].name].WriteLine(graphsConfig[i].eventX + "," + graphsConfig[i].eventY);
        }
    }

    private void Update()
    {
    }

    private void OnDestroy()
    {
        for(int i = 0; i < graphs.Length; ++i)
        {
            graphWriters[graphs[i].name].Close();
        }
    }
    public override void Send(TrackerEvent e)
    {
        //eventsBuff.Add(e);
        for (int i = 0; i < graphs.Length; ++i)
        {
            // Comprueba si la gráfica tiene el evento y si debe mostrar un nuevo punto
            if (graphs[i].ReceiveEvent(e))
            {
                // Si muestra un nuevo punto lo escribe en archivo para guardarlo
                Vector2 pos = graphs[i].getLatestPoint();
                // Formato: X (de los dos puntos), Y (del punto de la gráfica del jugador), Y (del punto de la gráfica del diseñador)
                graphWriters[graphs[i].name].WriteLine(pos.x + "," + pos.y + "," + graphs[i].getLatestObjectivePoint());
            }
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
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(grPers);

        GraphPersistence graphPersistence = (GraphPersistence)target;

        //Obtén los nombres de los eventos del script TrackerConfig
        TrackerConfig trackerConfig = graphPersistence.gameObject.GetComponent<TrackerConfig>();
        List<string> eventNames = new List<string>();
        foreach (TrackerConfig.EventConfig config in trackerConfig.eventConfig)
        {
            eventNames.Add(config.eventName);
        }

        // Crea un menú dropdown para cada elemento de graphs
        for (int i = 0; i < graphPersistence.graphsConfig.Length; i++)
        {
            string name = EditorGUILayout.TextField("GraphName", graphPersistence.graphsConfig[i].name);
            if (graphPersistence.graphsConfig[i].name != name)
            {
                graphPersistence.graphsConfig[i].name = name;
                EditorUtility.SetDirty(target);
            }

            AnimationCurve an = EditorGUILayout.CurveField(graphPersistence.graphsConfig[i].myCurve);            
            if(!graphPersistence.graphsConfig[i].myCurve.keys.SequenceEqual(an.keys))
            {
                graphPersistence.graphsConfig[i].myCurve = an;
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.Space(5);

            GraphTypes gT = (GraphTypes)EditorGUILayout.EnumPopup("GraphType", graphPersistence.graphsConfig[i].graphType);
            if (graphPersistence.graphsConfig[i].graphType != gT)
            {
                graphPersistence.graphsConfig[i].graphType = gT;
                EditorUtility.SetDirty(target);
            }

            Scaling sc = (Scaling)EditorGUILayout.EnumPopup("Scaling", graphPersistence.graphsConfig[i].scaling);
            if (graphPersistence.graphsConfig[i].scaling != sc)
            {
                graphPersistence.graphsConfig[i].scaling = sc;
                EditorUtility.SetDirty(target);
            }

            // Crea un menú popup con los nombres de los eventos
            int selectedEventIndex = eventNames.IndexOf(graphPersistence.graphsConfig[i].eventX);
            selectedEventIndex = EditorGUILayout.Popup("Select Event X", selectedEventIndex, eventNames.ToArray());
            if (selectedEventIndex != -1 && graphPersistence.graphsConfig[i].eventX != eventNames[selectedEventIndex])
            {
                graphPersistence.graphsConfig[i].eventX = eventNames[selectedEventIndex];
                EditorUtility.SetDirty(target);
            }

            selectedEventIndex = eventNames.IndexOf(graphPersistence.graphsConfig[i].eventY);
            selectedEventIndex = EditorGUILayout.Popup("Select Event Y", selectedEventIndex, eventNames.ToArray());
            if (selectedEventIndex != -1 && graphPersistence.graphsConfig[i].eventY != eventNames[selectedEventIndex])
            {
                graphPersistence.graphsConfig[i].eventY = eventNames[selectedEventIndex];
                EditorUtility.SetDirty(target);
            }

            //El resto de configuracion
            float aux;
            aux = EditorGUILayout.FloatField("Graph_Height", graphPersistence.graphsConfig[i].graph_Height);
            if (graphPersistence.graphsConfig[i].graph_Height != aux)
            {
                graphPersistence.graphsConfig[i].graph_Height = aux;
                EditorUtility.SetDirty(target);
            }

            aux = EditorGUILayout.FloatField("Graph_Width", graphPersistence.graphsConfig[i].graph_Width);
            if (graphPersistence.graphsConfig[i].graph_Width != aux)
            {
                graphPersistence.graphsConfig[i].graph_Width = aux;
                EditorUtility.SetDirty(target);
            }

            int intAux;
            intAux = EditorGUILayout.IntField("X_Pos", graphPersistence.graphsConfig[i].graph_X);
            if (graphPersistence.graphsConfig[i].graph_X != intAux)
            {
                graphPersistence.graphsConfig[i].graph_X = intAux;
                EditorUtility.SetDirty(target);
            }

            intAux = EditorGUILayout.IntField("Y_Pos", graphPersistence.graphsConfig[i].graph_Y);
            if (graphPersistence.graphsConfig[i].graph_Y != intAux)
            {
                graphPersistence.graphsConfig[i].graph_Y = intAux;
                EditorUtility.SetDirty(target);
            }

            intAux = EditorGUILayout.IntField("X_segments", graphPersistence.graphsConfig[i].x_segments);
            if (graphPersistence.graphsConfig[i].x_segments != intAux)
            {
                graphPersistence.graphsConfig[i].x_segments = intAux;
                EditorUtility.SetDirty(target);
            }

            intAux = EditorGUILayout.IntField("Y_segments", graphPersistence.graphsConfig[i].y_segments);
            if (graphPersistence.graphsConfig[i].y_segments != intAux)
            {
                graphPersistence.graphsConfig[i].y_segments = intAux;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space(20);
        }
        EditorGUILayout.Space();
        graphPersistence.graphObject = EditorGUILayout.ObjectField("Graph Object", graphPersistence.graphObject, typeof(GameObject), false) as GameObject;

        // Guarda los cambios realizados en el editor
        serializedObject.ApplyModifiedProperties();
    }
}
