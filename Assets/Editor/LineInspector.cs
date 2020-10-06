using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Line))]                //define Line class and use its components go to: Line.cs                
public class LineInspector : Editor         //in editor file it takes base class Editor and uses its components
{
    private void OnSceneGUI()               //drawing the visual lines
    {
        Line line = target as Line;
        Transform handleLineTransform = line.transform;                         //location,rotation and scale datas of object line.
        Vector3 p0 = handleLineTransform.TransformPoint(line.point0);
        Vector3 p1 = handleLineTransform.TransformPoint(line.point1);
        Quaternion LineRotation = Tools.pivotRotation == PivotRotation.Local ? handleLineTransform.rotation : Quaternion.identity;

        Handles.color = Color.red;
        Handles.DrawLine(p0, p1);

        EditorGUI.BeginChangeCheck();                               //it realizes the difference btw positions and assigns the new positions to points.
        p0 = Handles.DoPositionHandle(p0, LineRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point 0");                //realize the movement of movement of p0.
            EditorUtility.SetDirty(line);                           //undo movement.
            line.point0 = handleLineTransform.InverseTransformPoint(p0);
        }

        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, LineRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point 1");
            EditorUtility.SetDirty(line);
            line.point1 = handleLineTransform.InverseTransformPoint(p1);
        }
    }
}

