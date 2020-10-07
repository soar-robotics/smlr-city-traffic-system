using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class traffic_lights : MonoBehaviour
{
    public GameObject greenlight;
    public GameObject yellowlight;
    public GameObject redlight;
    public Material green;
    public Material yellow;
    public Material red;
    public bool Switch = true;

    // Start is called before the first frame update
    void Start()
    {
        green = greenlight.GetComponent<Renderer>().material;
        yellow = yellowlight.GetComponent<Renderer>().material;
        red = redlight.GetComponent<Renderer>().material;

        StartCoroutine("cycle");
        Debug.Log("started.");
    }
    IEnumerator cycle()
    {
        while (true)
        {
            if(Switch == true)
            {
                green.EnableKeyword("_EMISSION");
                yield return new WaitForSeconds(15);   //Wait
                Debug.Log("green enable.");
                Switch = false;
                green.DisableKeyword("_EMISSION");
                yellow.EnableKeyword("_EMISSION");
                Debug.Log("Yellow Enable.");
                yield return new WaitForSeconds(4);   //Wait
                yellow.DisableKeyword("_EMISSION");
            }
            else if(Switch == false){
                Debug.Log("red enable.");
                red.EnableKeyword("_EMISSION");
                yield return new WaitForSeconds(30);   //Wait
                Switch = true;
                red.DisableKeyword("_EMISSION");
                yellow.EnableKeyword("_EMISSION");
                yield return new WaitForSeconds(4);   //Wait
                yellow.DisableKeyword("_EMISSION");
            }
        }
    }
    
}
