using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System;
using UnityEditor;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class GraphPersistence : IPersistence
{
    [Serializable]
    public struct graph
    {
        public string name;
        public string eventX;
        public string eventY;
        public AnimationCurve myCurve;
    }

    List<TrackerEvent> eventsBuff;

    [SerializeField]
    public graph[] graphs;

    private void Start()
    {
        eventsBuff = new();
        Keyframe k = (Keyframe)(graphs[0].myCurve.keys.GetValue(0));
    }

    private void OnDestroy()
    {

    }
    public override void Send(TrackerEvent e)
    {
        eventsBuff.Add(e);
    }

    public override void Flush()
    {

    }
}
[CustomEditor(typeof(GraphPersistence))]
public class GraphPersistenceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GraphPersistence graphPersistence = (GraphPersistence)target;
        
        // Crea un campo para el arreglo graphs en GraphPersistence
        EditorGUILayout.PropertyField(serializedObject.FindProperty("graphs"), true);
        
        // Crea un menú dropdown para cada elemento de graphs
        for (int i = 0; i < graphPersistence.graphs.Length; i++)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(graphPersistence.graphs[i].name, EditorStyles.boldLabel);
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
            int selectedEventIndex = eventNames.IndexOf(graphPersistence.graphs[i].eventX);
            selectedEventIndex = EditorGUILayout.Popup("Select Event X", selectedEventIndex, eventNames.ToArray());
            graphPersistence.graphs[i].eventX = eventNames[selectedEventIndex];
            
            selectedEventIndex = eventNames.IndexOf(graphPersistence.graphs[i].eventY);
            selectedEventIndex = EditorGUILayout.Popup("Select Event Y", selectedEventIndex, eventNames.ToArray());
            graphPersistence.graphs[i].eventY = eventNames[selectedEventIndex];
        }
        
        // Guarda los cambios realizados en el editor
        serializedObject.ApplyModifiedProperties();
    }
}
