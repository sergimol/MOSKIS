using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Graph : MonoBehaviour
{
    public GraphConfig graphInfo;
    [SerializeField]
    GameObject window_graph_object;
    Window_Graph window_graph;

    public void SetUpWindowGraph(GameObject canvas)
    {
        // Crear un nuevo window_graph por cada grafica hijo del objeto Canvas
        GameObject window = Instantiate(window_graph_object);
        window.transform.SetParent(canvas.transform, false);
        window_graph_object = window;
        window_graph = window_graph_object.GetComponent<Window_Graph>();
    }

    public void setupWindowGraph()
    {
        window_graph.graph_Width = graphInfo.graph_Width;
        window_graph.graph_Width = graphInfo.graph_Height;
        window_graph.x_segments = graphInfo.x_segments;
        window_graph.y_segments = graphInfo.y_segments;
        SetUpObjetiveLine();
    }

    public void SetUpObjetiveLine()
    {
        List<float> objetiveLineAux = new List<float>();

        for (int i = 0; i < graphInfo.myCurve.length; ++i)
        {
            float aux = graphInfo.myCurve.Evaluate(i);
            objetiveLineAux.Add(aux);
        }
        window_graph.GetComponent<Window_Graph>().SetObjetiveLine(objetiveLineAux);
    }
}
