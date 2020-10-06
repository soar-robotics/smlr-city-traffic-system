using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour
{
    public Vector3[] points;

    public void Set()
    {
        points = new Vector3[] {            //default declaretion of curve vectors.
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
    }

    public Vector3 GetPoint(float index)                            //gets points' transform datas seperately.
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], index));
    }

    //standart math used methods class used in operations
    public static class Bezier
    {

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        public static Vector3 FirstDerivative(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (point1 - point0) +
                6f * oneMinusT * t * (point2 - point1) +
                3f * t * t * (point3 - point2);
        }
       
    }

    public Vector3 GetDirection(float t)
    {
        return transform.TransformPoint(Bezier.FirstDerivative(points[0], points[1], points[2], points[3], t)) - transform.position;
    }

    public Vector3 getDirectionNormalized(float t)
    {
        return GetDirection(t).normalized;
    }
    //creation
    void Start()
    {
        points[0].x = 1f;
        points[1].x = 2f;
        points[2].x = 3f;
        points[3].x = 4f;
    }

  
}
