using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System;

public class GraphPersistence : IPersistence
{
    [Serializable]
    public struct graph{
        public string name;
        public string eventX;
        public string eventY;
        public AnimationCurve myCurve;
    }

    List<TrackerEvent> eventsBuff;

    [SerializeField]
    graph[] graphs;

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
