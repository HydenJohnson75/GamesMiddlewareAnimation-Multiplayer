using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConeFieldOfView))] 
public class ConeVisionEditor : Editor
{
    private void OnSceneGUI()
    {
        ConeFieldOfView cfow = (ConeFieldOfView)target;
        Handles.color = UnityEngine.Color.white;
        Handles.DrawWireArc(cfow.transform.position, Vector3.up, Vector3.forward, 360, cfow.visionRadius);
        Vector3 viewAngleA = cfow.DirectionFromAngle(-cfow.visionAngle / 2, false);
        Vector3 viewAngleB = cfow.DirectionFromAngle(cfow.visionAngle / 2, false);

        Handles.DrawLine(cfow.transform.position, cfow.transform.position + viewAngleA * cfow.visionRadius);
        Handles.DrawLine(cfow.transform.position, cfow.transform.position + viewAngleB * cfow.visionRadius);

        Handles.color = UnityEngine.Color.red;
        foreach(Transform visibleTargets in cfow.visablesTargets) 
        { 
            Handles.DrawLine(cfow.transform.position, visibleTargets.position);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
