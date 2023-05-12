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
        for(int i = 0; i<grpP.graphsConfig.Length; i++)
        {
            for(int j=0; j< grpP.graphsConfig[i].x_segments; j++)
            {
                AnimationCurve anC = grpP.graphsConfig[i].myCurve;
                int y = j;
                if (anC.length > j)
                {
                    y = (int)anC.keys[j].value;
                    anC.RemoveKey(j);
                }
                Keyframe k = new Keyframe(j, y);
                anC.AddKey(k);
                AnimationUtility.SetKeyRightTangentMode(anC, j, TangentMode.Linear);
                AnimationUtility.SetKeyLeftTangentMode(anC, j, TangentMode.Linear);
            }
        }
    }
}
