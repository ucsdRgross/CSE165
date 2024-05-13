using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour 
{
    
    public int order;
    public GameObject manager;
    public ParticleSystem ps;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        if (manager.GetComponent<Checkpoints>().curPoint == order){
            manager.GetComponent<Checkpoints>().curPoint++;
            ps.transform.position = transform.position;
            ps.Play();
            Destroy(gameObject);
        }
    }
}
