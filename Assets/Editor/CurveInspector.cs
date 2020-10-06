using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Curve))]
public class BezierCurveInspector : Editor
{

    private Curve curve;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int iteration = 5;
    private const float directionScale = 0.5f;

    private void OnSceneGUI()
    {
        curve = target as Curve;
        handleTransform = curve.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowPoint(1);
        Vector3 p2 = ShowPoint(2);
        Vector3 p3 = ShowPoint(3);

        Handles.color = Color.blue;                     //simply draws the line between two points.
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);

        ShowDirections();
        Handles.DrawBezier(p0, p3, p1, p2, Color.cyan, null, 2f);
        /*Handles.color = Color.cyan;                     //block#1
        Vector3 startLine = curve.GetPoint(0f);

        Vector3 tangentLine = curve.GetPoint(0f);                   //starting positions for guide points of the curve.
        Handles.color = Color.green;
        Handles.DrawLine(tangentLine, tangentLine + curve.GetDirection(0f));
        for (int i = 0; i <= iteration; i++)                                        this for loop draws the line (cyan) to between interpolated points and also draws tangent lines (green) to that points.
            {
            Vector3 endLine = curve.GetPoint(i / (float)iteration);
            Handles.color = Color.cyan;
            Handles.DrawLine(startLine, endLine);
            Handles.color = Color.green;
            Handles.DrawLine(endLine, endLine + curve.GetDirection(i / (float)iteration));
            startLine = endLine;
        }*/

    }


    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(curve.points[index]);        //takes related points of the target curve.
        EditorGUI.BeginChangeCheck();                                                  //identifies the position difference of the points.
        point = Handles.DoPositionHandle(point, handleRotation);                    //transform points
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curve, "Move Point");                                 //if difference is set, apply the new transforn
            EditorUtility.SetDirty(curve);                                          //ctrl + z option
            curve.points[index] = handleTransform.InverseTransformPoint(point);     //apply transform difference to curve.
        }
        return point;
    }
   private void ShowDirections()                                                          //with usage of this function and Handles.DrawBrezier method you can get rid of the code block called block#1
    {
        Handles.color = Color.green;
        Vector3 tangent = curve.GetPoint(0f);
        Handles.DrawLine(tangent, tangent + curve.getDirectionNormalized(0f) * directionScale);
        for (int i = 1; i <= iteration; i++)
        {
            tangent = curve.GetPoint(i / (float)iteration);
            Handles.DrawLine(tangent, tangent + curve.getDirectionNormalized(i / (float)iteration) * directionScale);
        }
    }
}