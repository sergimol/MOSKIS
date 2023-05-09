using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour
{
    [SerializeField]
    Sprite circle_sprite;
    [SerializeField]
    float circle_scale;

    [SerializeField]
    RectTransform graph_container;
    [SerializeField]
    RectTransform label_template_X;
    [SerializeField]
    RectTransform label_template_Y;
    [SerializeField]
    RectTransform dash_template_X;
    [SerializeField]
    RectTransform dash_template_Y;

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


    // Puntos de la telemetria
    List<float> points;
    [SerializeField]
    LineRenderer line_renderer;

    // Puntos del diseñador
    List<float> objetive_points;
    [SerializeField]
    LineRenderer objetive_line_renderer;

    int y_separator = 10;

    //GameObject[] circles;
    List<GameObject> circles;

    float graph_Height;
    float graph_Width;
    float y_max = 100f;// puntos de Y
    float x_size = 50f;// distancia entre puntos de X
    float x_pos = 0;

    int visualize_points = 10; // numero de puntos que se van a representar como maximo a la vez en la grafica
    // |-------------*-
    // |-------*------- 
    // |----*-----*----
    // |-*-------------
    // +---------------
    // Los puntos se van ocultando por la izquierda <- <- <-

    [SerializeField]
    GameObject separator_line_renderer;


    // 
    float actual_y_max;
    float actual_y_min;
    float actual_x_max;


    private void Awake()
    {
        // Altura del grid
        graph_Height = graph_container.sizeDelta.y;
        // Ancho del grid
        graph_Width = graph_container.sizeDelta.x;

        visualize_points--;

        circles = new List<GameObject>();
        points = new List<float>();
        Debug.Log(circles.Count);
        Debug.Log(points.Count);

        actual_x_max = visualize_points;
        actual_y_max = y_max;
        actual_y_min = 0;

        ShowGraph();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddPoint(Random.Range(0, 100));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveDown();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveUp();
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

    void ShowGraph()
    {
        //ClearGraph();

        // Recorremos todos los puntos colocandolos
        //for (int i = 0; i < points.Count; i++)
        //{
        //    // Solo coloca los puntos
        //    AddPoint(points[i]);

        //}

        // Separador Eje X
        x_size = graph_Width / visualize_points;
        float y_size = graph_Height / visualize_points;

        float x = 0;
        float y = x;
        for (int i = 0; i < visualize_points + 1; i++)
        {
            RectTransform dashX = Instantiate(dash_template_X, lines_container.transform);
            dashX.anchoredPosition = new Vector2(x, 0);
            dashX.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graph_Height);

            RectTransform labelX = Instantiate(label_template_X, label_X_container.transform);
            labelX.anchoredPosition = new Vector2(x, 0); 
            labelX.GetComponent<TextMeshProUGUI>().text = i.ToString();

            x += x_size;


            RectTransform dashY = Instantiate(dash_template_Y, lines_container.transform);
            dashY.anchoredPosition = new Vector2(0, y); 
            dashY.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, graph_Width);

            RectTransform labelY = Instantiate(label_template_Y, label_Y_container.transform);
            labelY.anchoredPosition = new Vector2(0, y);
            labelY.GetComponent<TextMeshProUGUI>().text = i.ToString();

            y += y_size;
        }




    }

    private float GetAngle(Vector2 point1, Vector2 point2)
    {
        Vector2 direction = point2 - point1;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }

    // Crea la linea que une todos los puntos
    // Solo dibuja la linea entre los puntos que se renderizan dentro del viewport
    // Ahora como esta hay que cambiarlo
    private void CreateLine()
    {
        // Copiamos la lista de Circulos al vector de posiciones
        Vector3[] aux = new Vector3[points.Count];
        for(int i = 0; i < circles.Count; i++)
        {
            Transform t = circles[i].transform;
            Vector3 v = new Vector3(t.position.x, t.position.y);
            aux[i] = v;
        }

        // Creamos la linea 
        line_renderer.positionCount = points.Count;
        line_renderer.SetPositions(aux);
    }

    // Añade un nuevo punto a la grafica
    public void AddPoint(float new_y)
    {
        // Añadimos el nuevo punto
        points.Add(new_y);

        // Lo creamos
        float y_pos = (points[points.Count-1] / y_max) * graph_Height;
        GameObject point_object = CreateCircle(new Vector2(x_pos, y_pos));
        circles.Add(point_object);
        x_pos += x_size;

        CheckMove(new Vector2(points.Count - 1, new_y));

        // Lo unimos a la grafica
        CreateLine();
    }

    // Desplaza la Grafica a la Izquierda 1 posicion
    private void MoveLeft()
    {
        // Movemos el container
        left_container.anchoredPosition = new Vector2(left_container.anchoredPosition.x - x_size, left_container.anchoredPosition.y);

        // Redibujamos
        CreateLine();
    }
    // Desplaza la Grafica hacia Abajo 1 posicion
    private void MoveDown()
    {
        float y_size = graph_Height / visualize_points;
        // Movemos el container
        RectTransform rectY = label_Y_container.GetComponent<RectTransform>();
        left_container.anchoredPosition = new Vector2(left_container.anchoredPosition.x, left_container.anchoredPosition.y - y_size);
        rectY.anchoredPosition = new Vector2(rectY.anchoredPosition.x, rectY.anchoredPosition.y - y_size);
        // Redibujamos
        CreateLine();
    }
    private void MoveUp()
    {
        float y_size = graph_Height / visualize_points;
        // Movemos el container
        RectTransform rectY = label_Y_container.GetComponent<RectTransform>();
        left_container.anchoredPosition = new Vector2(left_container.anchoredPosition.x, left_container.anchoredPosition.y + y_size);
        rectY.anchoredPosition = new Vector2(rectY.anchoredPosition.x, rectY.anchoredPosition.y + y_size);

        // Redibujamos
        CreateLine();
    }

    private void CheckMove(Vector2 newPoint)
    {
        if(newPoint.y > actual_y_max)
        {
            MoveUp();
            actual_y_max = newPoint.y;
        }
        else if (newPoint.y < actual_y_min)
        {
            MoveDown();
            actual_y_min = newPoint.y;
        }

        if(newPoint.x > actual_x_max)
        {
            MoveLeft();
            actual_x_max = newPoint.x;
        }    
    }

}
