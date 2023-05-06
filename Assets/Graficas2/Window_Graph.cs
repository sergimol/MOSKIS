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


    // Puntos de la telemetria
    [SerializeField]
    List<float> points;
    [SerializeField]
    LineRenderer line_renderer;

    // Puntos del diseñador
    [SerializeField]
    List<float> objetive_points;
    [SerializeField]
    LineRenderer objetive_line_renderer;

    int y_separator = 10;

    //GameObject[] circles;
    List<GameObject> circles;

    float graph_Height;
    float y_max = 100f;// puntos de Y
    float x_size = 50f;// distancia entre puntos de X
    float x_pos = 0;

    private void Awake()
    {
        // Altura del grid
        graph_Height = graph_container.sizeDelta.y;

        line_renderer.positionCount = points.Count;
        circles = new List<GameObject>();
        points = new List<float>();
        Debug.Log(circles.Count);
        Debug.Log(points.Count);
        ShowGraph();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            AddPoint(Random.Range(0, 100));
        }
    }

    // Crea un circulo, pàra representar graficamente un punto
    GameObject CreateCircle(Vector2 pos)
    {
        // Creamos una Imagen
        GameObject game_Object = new GameObject();
        game_Object.AddComponent<Image>();
        game_Object.transform.SetParent(graph_container.transform, false);

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

        // Lo unimos a la grafica
        CreateLine();
    }


    //// Añadimos el marcador X
    //RectTransform labelX = Instantiate(label_template_X);
    //labelX.SetParent(graph_container, false);
    //labelX.anchoredPosition = new Vector2(x_pos, -7f); // le casca un 7 el men, luego lo cambio
    //labelX.GetComponent<TextMeshProUGUI>().text = i.ToString();

    //// Añadimos el separador X
    //RectTransform dashX = Instantiate(dash_template_X);
    //dashX.SetParent(graph_container, false);
    //dashX.anchoredPosition = new Vector2(x_pos, -7f); // le casca un 7 el men, luego lo cambio

    // Añadir el marcador de la Y
    //for(int i = 0; i <= y_separator; i++)
    //{
    //    RectTransform labelX = Instantiate(label_template_Y);
    //    labelX.SetParent(graph_container, false);
    //    float n = i * (1f / y_separator);
    //    labelX.anchoredPosition = new Vector2(-1000f, n * graph_Height); // le casca un 7 el men, luego lo cambio
    //    labelX.GetComponent<TextMeshProUGUI>().text = (n * y_max).ToString();

    //    // Añadimos el separador Y
    //    RectTransform dashX = Instantiate(dash_template_Y);
    //    dashX.SetParent(graph_container, false);
    //    dashX.anchoredPosition = new Vector2(-7f, n * graph_Height); // le casca un 7 el men, luego lo cambio
    //}
}
