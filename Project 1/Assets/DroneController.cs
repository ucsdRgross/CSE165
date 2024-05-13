using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DroneController : MonoBehaviour
{
    public GameObject drone;
    public GameObject controlOrb;
    public GameObject leftAnchor;
    public GameObject rightAnchor;
    private bool leftGripping = false;
    private bool rightGripping = false;
    public bool active = false;
    public TextMeshPro timer;
    public TextMeshPro countdown;
    private float timerf;
    private float countdownf = 3.0f;
    private bool playing = false;
    public bool gameEnded = false;
    public GameObject manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update(){
        if (countdownf > 0.0f) {
            active = false;
            countdownf -= Time.deltaTime;
        }
        if (countdownf <= 0.0f) {
            countdownf = 0.0f;
            playing = true;
            active = true;
        }
        if (playing && !gameEnded) {
            timerf += Time.deltaTime;
        }
        timer.text = timerf.ToString("F2");
        countdown.text = countdownf.ToString("F2");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        controlOrb.transform.position = Vector3.Lerp(leftAnchor.transform.position, rightAnchor.transform.position, 0.5f);
        controlOrb.transform.right = rightAnchor.transform.position - leftAnchor.transform.position;
        if (active && (leftGripping || rightGripping)){
            float angle = controlOrb.transform.rotation.eulerAngles.z;
            float angle_change = Mathf.DeltaAngle(0.0f, -angle);
            transform.Rotate(0.0f, angle_change/60.0f, 0.0f);
            
            Vector3 velocity = controlOrb.transform.position - drone.transform.position;
            float distance = Vector3.Distance(drone.transform.position, controlOrb.transform.position);
            velocity *= Mathf.Pow(1 + distance, 1.5f);
            transform.position += velocity;
        }

    }

    public void leftGrip(bool gripping){
        leftGripping = gripping;
        Debug.Log(leftGripping);
    }
    public void rightGrip(bool gripping){
        rightGripping = gripping;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<checkpoint>()) {
            countdownf = 3.0f;
            transform.position = manager.GetComponent<Checkpoints>().points[manager.GetComponent<Checkpoints>().curPoint - 1];
        }
    }
}
