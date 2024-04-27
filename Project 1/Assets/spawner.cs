using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject spawn;
    private GameObject spawned;
    // Start is called before the first frame update
    void Start()
    {
        spawn.GetComponent<Renderer>().material.color = Color.cyan;
        spawnObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == spawned){
            spawnObject();
        }
    }

    void spawnObject(){
        spawned = Instantiate(spawn, transform);
        spawned.SetActive(true);
        print(spawned);
    }
}
