using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookSelect : MonoBehaviour
{
    GameObject selection_target;
    GameObject system;
    void Start()
    {
        print(transform);
        print(transform.position);
        print(transform.forward);
        system = GameObject.Find("Particle System");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100.0f)){
            GameObject hitObject = hit.transform.gameObject;
            selection_target = hitObject;
            print(hitObject);
            selection_target.GetComponent<Renderer>().material.color += new Color(0f,1.0f/(60*10),0f,1f);
            system.transform.position = hit.transform.position;
        }
        else
        {
            //system.transform = transform;
            selection_target = null;
        }
    }
}
