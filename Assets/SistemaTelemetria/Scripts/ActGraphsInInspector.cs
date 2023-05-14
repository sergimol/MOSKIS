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
        //Recorrido de todas las gr�ficas del editor
        for(int i = 0; i<grpP.graphsConfig.Length; i++)     
        {
            AnimationCurve anC = grpP.graphsConfig[i].myCurve;

            //Si no hay ning�n segmento en x se a�ade un punto inicial a la gr�fica
            if (grpP.graphsConfig[i].x_segments == 0)
            {
                Keyframe k = new Keyframe(0, 0);
                anC.AddKey(k);
                AnimationUtility.SetKeyRightTangentMode(anC, 0, TangentMode.Linear);
                AnimationUtility.SetKeyLeftTangentMode(anC, 0, TangentMode.Linear);
            }
            else
            {
                //Actualizaci�n de todos los puntos de la gr�fica
                for (int j = 0; j < grpP.graphsConfig[i].x_segments; j++)
                {
                    int y = j;
                    //Si ya hay un punto en el segmento j se coge su valor en y y se elimina
                    if (anC.length > j)
                    {
                        y = (int)anC.keys[j].value;
                        anC.RemoveKey(j);
                    }
                    //A�adimos nuevo punto con las tangentes en Linear y el valor de y = anterior o = x
                    Keyframe k = new Keyframe(j, y);
                    anC.AddKey(k);
                    AnimationUtility.SetKeyRightTangentMode(anC, j, TangentMode.Linear);
                    AnimationUtility.SetKeyLeftTangentMode(anC, j, TangentMode.Linear);
                }
            }

            anC.preWrapMode = WrapMode.Clamp;
            anC.postWrapMode = WrapMode.Clamp;
        }
    }
}
