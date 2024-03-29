﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Splines))]
public class SplineInspector : Editor
{
    private const int iteration = 5;
    private const float directionScale = 0.5f;

    private Splines spline;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private void OnSceneGUI()
    {
        spline = target as Splines;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowPoint(1);
        Vector3 p2 = ShowPoint(2);
        Vector3 p3 = ShowPoint(3);

        Handles.color = Color.gray;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);
        ShowDirections();
        Handles.DrawBezier(p0, p3, p1, p2, Color.red, null, 2f);

    }
    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoints(0f);
        Handles.DrawLine(point, point + spline.getDirectionNormalized(0f) * directionScale);
        for (int i = 1; i <= iteration; i++)
        {
            point = spline.GetPoints(i / (float)iteration);
            Handles.DrawLine(point, point + spline.getDirectionNormalized(i / (float)iteration) * directionScale);
        }
    }
    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline.points[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.points[index] = handleTransform.InverseTransformPoint(point);
        }
        return point;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        spline = target as Splines;
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }
}