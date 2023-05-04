using System.Collections;
using System.Collections.Generic;
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
    List<float> points;

    int y_separator = 10;

    private void Awake()
    {
        ShowGraph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        // Altura del grid
        float graph_Height = graph_container.sizeDelta.y;
        float y_max = 100f; // puntos de Y
        float x_size = 50f; // distancia entre puntos de X

        // Recorremos todos los puntos colocandolos
        GameObject last_point_object = null;
        for(int i = 0; i < points.Count; i++)
        {
            float x_pos = (i * x_size) + x_size;
            float y_pos = (points[i] / y_max) * graph_Height;
            GameObject point_object = CreateCircle(new Vector2(x_pos, y_pos));
            if(last_point_object != null)
            {
                CreatePointConnection(last_point_object.GetComponent<RectTransform>().anchoredPosition,
                                       point_object.GetComponent<RectTransform>().anchoredPosition);
            }
            last_point_object = point_object;

            // A�adimos el marcador X
            RectTransform labelX = Instantiate(label_template_X);
            labelX.SetParent(graph_container, false);
            labelX.anchoredPosition = new Vector2(x_pos, -7f); // le casca un 7 el men, luego lo cambio
            labelX.GetComponent<Text>().text = i.ToString();

            // A�adimos el separador X
            RectTransform dashX = Instantiate(dash_template_X);
            dashX.SetParent(graph_container, false);
            dashX.anchoredPosition = new Vector2(x_pos, -7f); // le casca un 7 el men, luego lo cambio
        }

        // A�adir el marcador de la Y
        for(int i = 0; i <= y_separator; i++)
        {
            RectTransform labelX = Instantiate(label_template_Y);
            labelX.SetParent(graph_container, false);
            float n = i * (1f / y_separator);
            labelX.anchoredPosition = new Vector2(-7f, n * graph_Height); // le casca un 7 el men, luego lo cambio
            labelX.GetComponent<Text>().text = (n * y_max).ToString();

            // A�adimos el separador Y
            RectTransform dashX = Instantiate(dash_template_Y);
            dashX.SetParent(graph_container, false);
            dashX.anchoredPosition = new Vector2(-7f, n * graph_Height); // le casca un 7 el men, luego lo cambio
        }
    }

    void CreatePointConnection(Vector2 posA, Vector2 posB)
    {
        // Creamos una Imagen
        GameObject game_Object = new GameObject();
        game_Object.AddComponent<Image>();
        game_Object.transform.SetParent(graph_container.transform, false);

        // Seteamos su posicion
        RectTransform rect = game_Object.GetComponent<RectTransform>();

        // Direccion de la linea
        Vector2 dir = (posB - posA).normalized;
        float distance = Vector2.Distance(posA, posB);
        rect.localEulerAngles = new Vector3(0, 0, GetAngle(posB, posA));

        // Tama�o de la linea
        rect.sizeDelta = new Vector2(distance, 3);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);

        // Posicion, entre ambos puntos
        rect.anchoredPosition = posA + (dir * distance)/2; 


    }

    private float GetAngle(Vector2 point1, Vector2 point2)
    {
        Vector2 direction = point2 - point1;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
}
