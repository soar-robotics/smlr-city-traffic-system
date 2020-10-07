using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;           //for Array Resize

public class Splines : MonoBehaviour
{
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

    //standart methods
    public Vector3 GetPoints(float index)
    {
        int i;
        if (index >= 1f)
        {
            index = 1f;
            i = points.Length - 4;
        }
        else
        {
            index = Mathf.Clamp01(index) * CurveCount();
            i = (int)index;
            index -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], index));
    }

    public Vector3 GetDirection(float t)
    {
        int j;
        if(t >= 1f)
        {
            t = 1f;
            j = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount();
            j = (int)t;
            t -= j;
            j *= 3;
        }
        return transform.TransformPoint(Bezier.FirstDerivative(points[j], points[j + 1], points[j + 2], points[j + 3], t)) - transform.position;
    }
    public Vector3 getDirectionNormalized(float t)
    {
        return GetDirection(t).normalized;
    }
    [SerializeField]
    private BezierControlPointMode[] modes;

    public void AddCurve()
    {
        Vector3 newPoint = points[points.Length - 1];           //we create 3 new nodes that same as the last node of the curve.
        Array.Resize(ref points, points.Length + 3);            //it resizes and extents the array by 3 units.
        newPoint.x += 1f;
        points[points.Length - 3] = newPoint;
        newPoint.x += 1f;
        points[points.Length - 2] = newPoint;
        newPoint.x += 1f;
        points[points.Length - 1] = newPoint;
        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop)                                                   //if the spline is loop, when you add new curve it will remains the base shape of the curve.
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if (loop)                                                   //you know that first and last nodes are same.
        {
            if (modeIndex == 0)
            {
                modes[modes.Length - 1] = mode;
            }
            else if (modeIndex == modes.Length - 1)
            {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    public int CurveCount()
    {

        return (points.Length - 1) / 3;

    }
    public int ControlPointCount()
    {
        return points.Length;
    }
    public Vector3 GetControlPoint(int index)
    {
        return points[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = point - points[index];
            if (loop)
            {
                if (index == 0)
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1)
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else
                {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length)
                {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        EnforceMode(index);
    }
    private void EnforceMode(int index)                         //to adjust points' modes. checks whether the fixed or enforced point wraps around the array.
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
        {
            return;
        }
        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
            {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = points.Length - 2;
            }
        }
        Vector3 middle = points[middleIndex];                           //mirrored mode case.
        Vector3 enforcedTangent = middle - points[fixedIndex];          
        if (mode == BezierControlPointMode.Aligned)                     //aligned mode case.
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }
    [SerializeField]
    public bool loop;
    public bool Loop                                //for making loop, enforce the first and last points on the same position, then set control point here.
    {
        get
        {
            return loop;
        }
        set
        {
            loop = value;
            if (value == true)
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    //standart declaretion
    [SerializeField]
    public Vector3[] points;
    public void Reset()
    {
        points = new Vector3[] {new Vector3(1f,0,0), new Vector3(2f,0,0), new Vector3(3f,0,0), new Vector3(4f,0,0)};
        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }
   
}
