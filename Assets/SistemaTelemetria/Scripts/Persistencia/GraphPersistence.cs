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
public enum Constrains { FREE_CONFIG, LEFT_TOP, LEFT_BOTTOM, LEFT_VERTICAL, RIGHT_VERTICAL }

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
    //Posici�n en X e Y
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

    public Constrains constrainsGraphs;

    //La configuracion inicial de todos los graphs desde el editor
    public GraphConfig[] graphsConfig;

    Window_Graph[] graphs;

    private string baseSaveRoute = "Trazas\\Graphs\\";
    private Dictionary<string, StreamWriter> graphWriters;

    float preset_Scale = 1f;

    private void Start()
    {
        eventsBuff = new();
        graphWriters = new Dictionary<string, StreamWriter>();

        // Crear la carpeta donde se guuardar�n los archivos que contienen los datos con los puntos de la gr�fica
        string id = Tracker.instance.getSessionId().ToString();
        string fullRoute = baseSaveRoute + id + "\\";
        Directory.CreateDirectory(fullRoute);

        // Crear un nuevo objeto Canvas
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<CanvasScaler>();              // Agregar el componente gr�fico CanvasScaler al objeto Canvas
        canvasObject.AddComponent<GraphicRaycaster>();          // Agregar el componente gr�fico GraphicRaycaster al objeto Canvas
        canvasObject.transform.SetParent(transform, false);     // Hacer que el objeto Canvas sea hijo del objeto padre
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        canvasObject.GetComponent<Canvas>().worldCamera = Camera.main;
        canvasObject.GetComponent<Canvas>().scaleFactor = 1f;  //!CUIDAO


        // ESTO ESTA SIENDO DELICADO
        CanvasScaler mivieja = canvasObject.GetComponent<CanvasScaler>();
        mivieja.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        Resolution res = Screen.currentResolution;
        mivieja.referenceResolution = new Vector2(res.width, res.height);

        //Crear tantos Graph como se han configurado y pasarles la informaci�n
        Array.Resize(ref graphs, graphsConfig.Count());
        for (int i = 0; i < graphsConfig.Count(); ++i)
        {
            // Creamos el objeto grafica
            GameObject aux = Instantiate(graphObject, parent: canvasObject.transform);

            // Rescalamos y posicionamos 
            aux.GetComponent<RectTransform>().localScale = new Vector3(preset_Scale / graphsConfig.Count(), preset_Scale / graphsConfig.Count(), preset_Scale);
            float offset = (res.width / graphsConfig.Count()) * i;
            aux.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset, graphsConfig[i].graph_Y);

            graphs[i] = aux.GetComponent<Window_Graph>();
            graphs[i].name = graphsConfig[i].name;
            graphs[i].SetConfig(graphsConfig[i]);

            // Crear el archivo en el que se guardar�n los puntos en formato de texto
            graphWriters.Add(graphsConfig[i].name, new StreamWriter(fullRoute + graphsConfig[i].name + ".csv"));
            graphWriters[graphsConfig[i].name].WriteLine(graphsConfig[i].eventX + "," + graphsConfig[i].eventY);

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            transform.GetChild(0).transform.gameObject.SetActive(!transform.GetChild(0).transform.gameObject.activeSelf);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < graphs.Length; ++i)
        {
            graphWriters[graphs[i].name].Close();
        }
    }
    public override void Send(TrackerEvent e)
    {
        //eventsBuff.Add(e);
        for (int i = 0; i < graphs.Length; ++i)
        {
            // Comprueba si la gr�fica tiene el evento y si debe mostrar un nuevo punto
            if (graphs[i].ReceiveEvent(e))
            {
                // Si muestra un nuevo punto lo escribe en archivo para guardarlo
                Vector2 pos = graphs[i].getLatestPoint();
                // Formato: X (de los dos puntos), Y (del punto de la gr�fica del jugador), Y (del punto de la gr�fica del dise�ador)
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

    void OnValidate()
    {

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //EditorUtility.SetDirty(target);
        EditorGUILayout.PropertyField(grPers);

        GraphPersistence graphPersistence = (GraphPersistence)target;

        //Obt�n los nombres de los eventos del script TrackerConfig
        TrackerConfig trackerConfig = graphPersistence.gameObject.GetComponent<TrackerConfig>();
        List<string> eventNames = new List<string>();
        foreach (TrackerConfig.EventConfig config in trackerConfig.eventConfig)
        {
            eventNames.Add(config.eventName);
        }

        //M�todo de checkeo de cambios en el editor
        EditorGUI.BeginChangeCheck();

        graphPersistence.constrainsGraphs = (Constrains)EditorGUILayout.EnumPopup("Constrains", graphPersistence.constrainsGraphs);
        EditorGUILayout.Space(10);

        // Crea un men� dropdown para cada elemento de graphs
        for (int i = 0; i < graphPersistence.graphsConfig.Length; i++)
        {
            GraphConfig actGraphConf = graphPersistence.graphsConfig[i];
            actGraphConf.name = EditorGUILayout.TextField("GraphName", actGraphConf.name);

            actGraphConf.myCurve = EditorGUILayout.CurveField(actGraphConf.myCurve);
            EditorGUILayout.Space(5);

            //Variables de escala
            actGraphConf.graphType = (GraphTypes)EditorGUILayout.EnumPopup("GraphType", actGraphConf.graphType);
            actGraphConf.scaling = (Scaling)EditorGUILayout.EnumPopup("Scaling", actGraphConf.scaling);

            // Crea un men� popup con los nombres de los eventos
            int selectedEventIndex;
            selectedEventIndex = EditorGUILayout.Popup("Select Event X", eventNames.IndexOf(actGraphConf.eventX), eventNames.ToArray());
            if (selectedEventIndex != -1 && actGraphConf.eventX != eventNames[selectedEventIndex])
                actGraphConf.eventX = eventNames[selectedEventIndex];

            selectedEventIndex = EditorGUILayout.Popup("Select Event Y", eventNames.IndexOf(actGraphConf.eventY), eventNames.ToArray());
            if (selectedEventIndex != -1 && actGraphConf.eventY != eventNames[selectedEventIndex])
                actGraphConf.eventY = eventNames[selectedEventIndex];

            //El resto de configuracion
            actGraphConf.graph_Height = EditorGUILayout.FloatField("Graph_Height", actGraphConf.graph_Height);
            actGraphConf.graph_Width = EditorGUILayout.FloatField("Graph_Width", actGraphConf.graph_Width);

            if (graphPersistence.constrainsGraphs == Constrains.FREE_CONFIG)
            {
                actGraphConf.graph_X = EditorGUILayout.IntField("X_Pos", actGraphConf.graph_X);
                actGraphConf.graph_Y = EditorGUILayout.IntField("Y_Pos", actGraphConf.graph_Y);
            }

            actGraphConf.x_segments = EditorGUILayout.IntField("X_segments", actGraphConf.x_segments);
            actGraphConf.y_segments = EditorGUILayout.IntField("Y_segments", actGraphConf.y_segments);

            EditorGUILayout.Space(20);
            graphPersistence.graphsConfig[i] = actGraphConf;
        }
        EditorGUILayout.Space();
        graphPersistence.graphObject = EditorGUILayout.ObjectField("Graph Object", graphPersistence.graphObject, typeof(GameObject), false) as GameObject;



        //Si ha habido cambios utilizamos setDirty para que unity no cambie los valores de editor y se mantengan para ejecucion

        if (EditorGUI.EndChangeCheck())

            EditorUtility.SetDirty(target);



        // Guarda los cambios realizados en el editor

        serializedObject.ApplyModifiedProperties();
    }
}
