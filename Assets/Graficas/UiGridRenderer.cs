using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Este Script Dibuja el Grid de la Grafica
public class UiGridRenderer : Graphic
{
    float width;
    float height;
    [SerializeField]
    float thickness;

    [SerializeField]
    Vector2Int gridSize = new Vector2Int(1, 1);

    float cellWidth;
    float cellHeight;

    // Metodo que se usa para redefinir mallas en ejecucion
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        cellWidth = width / gridSize.x;
        cellHeight = height / gridSize.y;

        //base.OnPopulateMesh(vh);

        // VertexHelper es el objeto que permite redefinir una malla
        // Limpiamos los posibles vertices
        vh.Clear();

        // Definimos un vertice, lo reutilizamos todo el rato cambiando sus parametros
        UIVertex vertex = UIVertex.simpleVert;

        // Cuadrado
        vertex.position = new Vector2(0, 0);
        vh.AddVert(vertex);

        vertex.position = new Vector2(0, height);
        vh.AddVert(vertex);

        vertex.position = new Vector2(width, height);
        vh.AddVert(vertex);

        vertex.position = new Vector2(width, 0);
        vh.AddVert(vertex);


        // Cuadrado de dentro
        float withSquare = thickness * thickness;
        float distanceSquare = width / 2;
        float distance = Mathf.Sqrt(distanceSquare);

        vertex.position = new Vector2(distance, distance);
        vh.AddVert(vertex);

        vertex.position = new Vector2(distance, height - distance);
        vh.AddVert(vertex);

        vertex.position = new Vector2(width - distance, height - distance);
        vh.AddVert(vertex);

        vertex.position = new Vector2(width - distance, distance);
        vh.AddVert(vertex);

        // 
        vh.AddTriangle(0, 1, 5);
        vh.AddTriangle(5, 4, 0);
        // 
        vh.AddTriangle(1, 2, 6);
        vh.AddTriangle(6, 5, 1);
        // 
        vh.AddTriangle(2, 3, 7);
        vh.AddTriangle(7, 6, 2);
        // 
        vh.AddTriangle(3, 0, 4);
        vh.AddTriangle(4, 7, 3);
    }
}

