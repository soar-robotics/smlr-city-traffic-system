using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public Splines path;
    public float speed = 5;
    public float process = 0;
    public bool direction;
    public GameObject GameObject;
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        process = process + Time.deltaTime / speed;
        
        Vector3 position = path.GetPoints(process);
        transform.localPosition = position;

        if (direction)
        {
            transform.LookAt(position + path.GetDirection(process));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Light")
        {
            Debug.Log("Vehicle goes towards traffic light.");
        }
        

       
        
    }
}
