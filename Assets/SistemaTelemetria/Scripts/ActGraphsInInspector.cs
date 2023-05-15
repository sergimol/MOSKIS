using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.AnimationUtility;

[ExecuteInEditMode]
public class ActGraphsInInspector : MonoBehaviour
{
    GraphPersistence grpP;
    // Start is called before the first frame update
    void Start()
    {
        grpP = GetComponent<GraphPersistence>();
    }

    // Update is called once per frame
    void Update()
    {
        //Recorrido de todas las gráficas del editor
        for(int i = 0; i<grpP.graphsConfig.Length; i++)     
        {
            AnimationCurve anC = grpP.graphsConfig[i].myCurve;

            //Si no hay ningún segmento en x se añade un punto inicial a la gráfica
            if (grpP.graphsConfig[i].x_segments == 0)
            {
                Keyframe k = new Keyframe(0, 0);
                anC.AddKey(k);
                AnimationUtility.SetKeyRightTangentMode(anC, 0, TangentMode.Linear);
                AnimationUtility.SetKeyLeftTangentMode(anC, 0, TangentMode.Linear);
            }
            else
            {
                //Actualización de todos los puntos de la gráfica
                for (int j = 0; j < grpP.graphsConfig[i].x_segments; j++)
                {
                    if ((j - 1 >= 0 && anC.keys[j - 1].time == j) || (j + 1 < anC.keys.Length && anC.keys[j + 1].time == j))
                        continue;
                    int y = j;
                    //Si ya hay un punto en el segmento j se coge su valor en y y se elimina
                    if (anC.length > j)
                    {
                        y = (int)Math.Round(anC.keys[j].value, 0);
                        anC.RemoveKey(j);
                    }
                    //Añadimos nuevo punto con las tangentes en Linear y el valor de y = anterior o = x
                    Keyframe k = new Keyframe(j, y);
                    anC.AddKey(k);
                    AnimationUtility.SetKeyRightTangentMode(anC, j, TangentMode.Linear);
                    AnimationUtility.SetKeyLeftTangentMode(anC, j, TangentMode.Linear);
                }
                while (anC.length > grpP.graphsConfig[i].x_segments)
                    anC.RemoveKey(anC.length - 1);
            }

            anC.preWrapMode = WrapMode.Clamp;
            anC.postWrapMode = WrapMode.Clamp;
        }
    }
}
