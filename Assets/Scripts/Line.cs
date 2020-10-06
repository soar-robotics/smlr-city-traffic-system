using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public Vector3 point0, point1;
   
    // Start is called before the first frame update
    void Start()
    {
        point1.x = 1f;                          //default values for second point
        point1.y = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
