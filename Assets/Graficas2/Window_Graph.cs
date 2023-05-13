using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static UnityEditor.PlayerSettings;

public class Window_Graph : MonoBehaviour
{
    [SerializeField]
    Sprite circle_sprite;
    float circle_scale = 5;

    // Puntos de la telemetria
    List<float> points;
    [SerializeField]
    LineRenderer line_renderer;

    // Puntos del diseñador
    //List<float> objetive_points = new List<float> { 10f, 30f, 0f, 12f, 12.5f, 6f, 2f };
    public List<float> objective_points;
    [SerializeField]
    LineRenderer objective_line_renderer;

    // Containers para los objetos
    [SerializeField]
    RectTransform graph_container;
    [SerializeField]
    GameObject points_container;
    [SerializeField]
    GameObject lines_container;
    [SerializeField]
    GameObject label_X_container;
    [SerializeField]
    GameObject label_Y_container;
    [SerializeField]
    RectTransform left_container;

    // Objetos que generamos
    [SerializeField]
    RectTransform label_template_X;
    [SerializeField]
    RectTransform label_template_Y;
    [SerializeField]
    RectTransform dash_template_X;
    [SerializeField]
    RectTransform dash_template_Y;

    // Configuración del gráfico
    public GraphConfig graphConfig;
    // Dimensiones del Grafico
    //public float graph_Height;
    //public float graph_Width;

    float x_size; // distancia entre puntos de X
    float y_size; // distancia entre puntos de Y
    float x_pos = 0;

    //public int x_segments = 10; // numero de separaciones que tiene el Eje X (Ademas es el numero de puntos que se representan en la grafica a la vez)
    //public int y_segments = 8; // numero de separaciones que tiene el Eje Y
    // |-------------*-
    // |-------*------- 
    // |----*-----*----
    // |-*-------------
    // +---------------
    // Los puntos se van ocultando por la izquierda <- <- <-


    float x_max; // Valor maximo que tiene el Eje X en cada momento
                 // Cada vez que se añade un punto se suma
    float y_max; // Valor maximo que puede alcanzar el Eje Y 
                 // Inicialmente lo determina el mayor valor de la grafica del diseñador
                 // Posteriormente se actualiza si aparecen valores mayores

    int acostada = 1;

    // Listas de objetos generados
    TextMeshProUGUI[] label_Y_List;
    List<TextMeshProUGUI> label_X_List;
    List<GameObject> circles;
    List<GameObject> objective_circles;
    int objective_index = 0;


    // Renderizado dentro del Viewport
    [SerializeField]
    ScrollRect render_viewport;


    public void SetConfig(GraphConfig g)
    {
        graphConfig = g;
        label_Y_List = new TextMeshProUGUI[graphConfig.y_segments + 1];

        // Altura del grid
        graphConfig.graph_Height = graph_container.sizeDelta.y;
        // Ancho del grid
        graphConfig.graph_Width = graph_container.sizeDelta.x;

        graphConfig.x_segments--;

        circles = new List<GameObject>();
        objective_circles = new List<GameObject>();
        points = new List<float>();
        label_X_List = new List<TextMeshProUGUI>();

        x_max = graphConfig.x_segments;

        List<float> objectiveLineAux = new List<float>();

        for (int i = 0; i < graphConfig.myCurve.length; ++i)
        {
            float aux = graphConfig.myCurve.Evaluate(i);
            objectiveLineAux.Add(aux);
        }
        SetObjectiveLine(objectiveLineAux);
        y_max = getMaxFromList();

        ShowGraph();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddPoint(Random.Range(0, 100));
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddPoint(acostada);
            acostada *= 2;
        }
    }

    void ShowGraph()
    {
        x_size = graphConfig.graph_Width / graphConfig.x_segments;
        y_size = graphConfig.graph_Height / graphConfig.y_segments;

        float x = 0;
        float y = x;

        // MARCADORES EJE X
        for (int i = 0; i < graphConfig.x_segments + 1; i++)
        {
            RectTransform dashX = Instantiate(dash_template_X, lines_container.transform);
            dashX.anchoredPosition = new Vector2(x, 0);
            dashX.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphConfig.graph_Height);

            RectTransform labelX = Instantiate(label_template_X, label_X_container.transform);
            labelX.anchoredPosition = new Vector2(x, 0);
            labelX.GetComponent<TextMeshProUGUI>().text = i.ToString();
            label_X_List.Add(labelX.GetComponent<TextMeshProUGUI>());

            x += x_size;
        }

        // MARCADORES EJE Y
        for (int i = 0; i < graphConfig.y_segments + 1; i++)
        {

            RectTransform dashY = Instantiate(dash_template_Y, lines_container.transform);
            dashY.anchoredPosition = new Vector2(0, y);
            dashY.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graphConfig.graph_Width);

            RectTransform labelY = Instantiate(label_template_Y, label_Y_container.transform);
            labelY.anchoredPosition = new Vector2(0, y);
            labelY.GetComponent<TextMeshProUGUI>().text = ((y_max / graphConfig.y_segments) * i).ToString();

            y += y_size;

            label_Y_List[i] = labelY.GetComponent<TextMeshProUGUI>();
        }

    }

    // Añade un nuevo punto a la grafica
    public void AddPoint(float new_y)
    {
        /// PUNTO TELEMETRIA
        // Añadimos el nuevo punto
        points.Add(new_y);
        CheckMove(new Vector2(points.Count - 1, new_y));
        Debug.Log("MAX: " + y_max + "  NEW:" + new_y);

        // Lo creamos
        float y_pos = (points[points.Count - 1] / y_max) * graphConfig.graph_Height;
        GameObject point_object = CreateCircle(new Vector2(x_pos, y_pos));
        circles.Add(point_object);


        /// PUNTO OBJETIVO
        // Añadimos el nuevo punto
        if (objective_points.Count >= points.Count)
        {
            CheckMove(new Vector2(objective_index, new_y));
            Debug.Log("MAX_O: " + y_max + "  NEW_O:" + new_y);
            Debug.Log("P: " + points.Count + "  O:" + objective_points.Count);

            // Lo creamos
            float o_y_pos = (objective_points[objective_index] / y_max) * graphConfig.graph_Height;
            GameObject o_point_object = CreateCircle(new Vector2(x_pos, o_y_pos));
            objective_circles.Add(o_point_object);
            objective_index++;
        }

        x_pos += x_size;


        // Lo unimos a la grafica
        CreateLine();
    }

    // Crea la linea que une todos los puntos
    // Solo dibuja la linea entre los puntos que se renderizan dentro del viewport
    // Ahora como esta hay que cambiarlo
    private void CreateLine()
    {
        // MUCHO BUCLE, IGUAL HAY QUE CAMBIARLO //

        /// TELEMETRIA ///
        // Creamos una lista de Posiciones dentro del Viewport
        List<Vector3> aux = new List<Vector3>();
        for (int i = 0; i < circles.Count; i++)
        {
            Transform t = circles[i].transform;

            // Comprobamos que el punto se este renderizando en el Viewport
            if (RectTransformUtility.RectangleContainsScreenPoint(render_viewport.viewport, t.position))
            {
                Vector3 v = new Vector3(t.position.x, t.position.y);
                aux.Add(v);
            }
        }
        // Volcamos la lista en un vector
        Vector3[] aux_def = new Vector3[aux.Count];
        for (int i = 0; i < aux.Count; i++)
        {
            aux_def[i] = aux[i];
        }


        // Creamos la linea 
        line_renderer.positionCount = aux.Count;
        line_renderer.SetPositions(aux_def);

        /// OBJETIVO ///
        // Creamos una lista de Posiciones dentro del Viewport
        List<Vector3> aux_o = new List<Vector3>();
        for (int i = 0; i < objective_index; i++)
        {
            Transform t = objective_circles[i].transform;

            // Comprobamos que el punto se este renderizando en el Viewport
            if (RectTransformUtility.RectangleContainsScreenPoint(render_viewport.viewport, t.transform.position))
            {
                Vector3 v = new Vector3(t.position.x, t.position.y);
                aux_o.Add(v);
            }
        }
        // Volcamos la lista en un vector
        Vector3[] aux_def_o = new Vector3[aux_o.Count];
        for (int i = 0; i < aux_o.Count; i++)
        {
            aux_def_o[i] = aux_o[i];
        }

        // Creamos la linea 
        objective_line_renderer.positionCount = aux_o.Count;
        objective_line_renderer.SetPositions(aux_def_o);
    }

    // Desplaza la Grafica a la Izquierda 1 posicion
    private void MoveLeft()
    {
        // Movemos el container
        left_container.anchoredPosition = new Vector2(left_container.anchoredPosition.x - x_size, left_container.anchoredPosition.y);

        // Añadimos el nuevo marcador abajo
        RectTransform labelX = Instantiate(label_template_X, label_X_container.transform);
        labelX.anchoredPosition = new Vector2(x_pos, 0);
        labelX.GetComponent<TextMeshProUGUI>().text = label_X_List.Count.ToString();
        label_X_List.Add(labelX.GetComponent<TextMeshProUGUI>());
    }

    private void CheckMove(Vector2 newPoint)
    {
        // Si el nuevo punto es mayor que el maximo que habia, ReEscalamos
        if (newPoint.y > y_max)
        {
            y_max = newPoint.y;
            ReScalePoints();
        }

        // Si Añadimos un nuevo punto y hay que desplazar la Grafica
        if (newPoint.x > x_max)
        {
            MoveLeft();
            x_max = newPoint.x;
        }
    }

    // Crea un circulo, pàra representar graficamente un punto
    GameObject CreateCircle(Vector2 pos)
    {
        // Creamos una Imagen
        GameObject game_Object = new GameObject("Point");
        game_Object.AddComponent<Image>();
        game_Object.transform.SetParent(points_container.transform, false);

        // Seteamos la imagen 
        game_Object.GetComponent<Image>().sprite = circle_sprite;

        // Seteamos su posicion
        RectTransform rect = game_Object.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(circle_scale, circle_scale);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);

        return game_Object;
    }

    private float getMaxFromList()
    {
        return objective_points.Max();
    }

    // Re Escalamos los puntos para que se ajusten a los nuevos valores maximos del eje Y
    private void ReScalePoints()
    {
        for (int i = 0; i < circles.Count; i++)
        {
            RectTransform rect = circles[i].GetComponent<RectTransform>();
            float y_pos = (points[i] / y_max) * graphConfig.graph_Height;
            Vector2 pos = new Vector2(rect.anchoredPosition.x, y_pos);
            rect.anchoredPosition = pos;
        }
        for (int i = 0; i < objective_circles.Count; i++)
        {
            RectTransform rect = objective_circles[i].GetComponent<RectTransform>();
            float y_pos = (objective_points[i] / y_max) * graphConfig.graph_Height;
            Vector2 pos = new Vector2(rect.anchoredPosition.x, y_pos);
            rect.anchoredPosition = pos;
        }

        for (int i = 0; i < label_Y_List.Length; i++)
        {
            label_Y_List[i].text = ((y_max / graphConfig.y_segments) * i).ToString();
        }
    }

    // Inicializa la lista de puntos del Diseñador (Llamar desde la persistencia al crear)
    // He puesto que pasais una lista, si pasais un vector pos lo cambiais jeje
    public void SetObjectiveLine(List<float> o)
    {
        objective_points = new List<float>(o);
    }
}
