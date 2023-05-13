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

[Serializable]
public struct GraphConfig
{
    public string name;
    [HideInInspector]
    public string eventX;
    [HideInInspector]
    public string eventY;
    public AnimationCurve myCurve;
    [HideInInspector]
    public GameObject window_graph;
    //Config del window_graph
    [HideInInspector]
    public float graph_Height;
    [HideInInspector]
    public float graph_Width;
    [HideInInspector]
    public int x_segments; // numero de separaciones que tiene el Eje X (Ademas es el numero de puntos que se representan en la grafica a la vez)
    [HideInInspector]
    public int y_segments; // numero de separaciones que tiene el Eje Y
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
        Keyframe k = (Keyframe)(graphsConfig[0].myCurve.keys.GetValue(0));

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
        for(int i = 0; i < graphs.Length; ++i)
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
    public override void OnInspectorGUI()
    {
        GraphPersistence graphPersistence = (GraphPersistence)target;

        // Crea un campo para el arreglo graphs en GraphPersistence
        EditorGUILayout.PropertyField(serializedObject.FindProperty("graphsConfig"), true);

        // Crea un menú dropdown para cada elemento de graphs
        for (int i = 0; i < graphPersistence.graphsConfig.Length; i++)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(graphPersistence.graphsConfig[i].name, EditorStyles.boldLabel);

            ////Editado
            //graphPersistence.graphs[i].name = EditorGUILayout.TextField("Name", graphPersistence.graphs[i].name);
            //graphPersistence.graphs[i].eventX = EditorGUILayout.TextField("Event X", graphPersistence.graphs[i].eventX);
            //graphPersistence.graphs[i].eventY = EditorGUILayout.TextField("Event Y", graphPersistence.graphs[i].eventY);

            //Obtén los nombres de los eventos del script TrackerConfig
            TrackerConfig trackerConfig = FindObjectOfType<TrackerConfig>();
            List<string> eventNames = new List<string>();
            foreach (TrackerConfig.EventConfig config in trackerConfig.eventConfig)
            {
                eventNames.Add(config.eventName);
            }

            // Crea un menú dropdown con los nombres de los eventos
            int selectedEventIndex = eventNames.IndexOf(graphPersistence.graphsConfig[i].eventX);
            selectedEventIndex = EditorGUILayout.Popup("Select Event X", selectedEventIndex, eventNames.ToArray());
            graphPersistence.graphsConfig[i].eventX = eventNames[selectedEventIndex];

            selectedEventIndex = eventNames.IndexOf(graphPersistence.graphsConfig[i].eventY);
            selectedEventIndex = EditorGUILayout.Popup("Select Event Y", selectedEventIndex, eventNames.ToArray());
            graphPersistence.graphsConfig[i].eventY = eventNames[selectedEventIndex];
            //El resto de configuracion
            graphPersistence.graphsConfig[i].graph_Height = EditorGUILayout.FloatField("graph_Height", graphPersistence.graphsConfig[i].graph_Height);
            graphPersistence.graphsConfig[i].graph_Width = EditorGUILayout.FloatField("graph_Width", graphPersistence.graphsConfig[i].graph_Width);
            graphPersistence.graphsConfig[i].x_segments = EditorGUILayout.IntField("x_segments", graphPersistence.graphsConfig[i].x_segments);
            graphPersistence.graphsConfig[i].y_segments = EditorGUILayout.IntField("y_segments", graphPersistence.graphsConfig[i].y_segments);
        }
        EditorGUILayout.Space();
        graphPersistence.graphObject = EditorGUILayout.ObjectField("Graph Object", graphPersistence.graphObject, typeof(GameObject), false) as GameObject;
        
        // Guarda los cambios realizados en el editor
        serializedObject.ApplyModifiedProperties();
    }
}
