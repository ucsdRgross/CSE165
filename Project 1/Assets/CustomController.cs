using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomController : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    private float step;
    // Start is called before the first frame update
    private Vector3 original_grab_position;
    private Vector3 last_grab_position;
    private bool currentGrabHandisPrimary;
    public float precision_distance = 0.1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //grab air to pull forwards
        bool left_hand_grabbing = OVRInput.Get(OVRInput.RawButton.LHandTrigger);
        bool right_hand_grabbing = OVRInput.Get(OVRInput.RawButton.RHandTrigger);
        if (left_hand_grabbing || right_hand_grabbing){
            if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger)){
                currentGrabHandisPrimary = true;
                last_grab_position = leftHand.position;
                last_grab_position -= transform.position;
                original_grab_position = last_grab_position;
            } else if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)){
                currentGrabHandisPrimary = false;
                last_grab_position = rightHand.position;
                last_grab_position -= transform.position;
                original_grab_position = last_grab_position;
            }
            Vector3 current_hand_position;
            if (currentGrabHandisPrimary) current_hand_position = leftHand.position;
            else current_hand_position = rightHand.position;
            current_hand_position -= transform.position;
            Vector3 velocity = last_grab_position - current_hand_position;
            float distance_from_grab = Vector3.Distance(original_grab_position, current_hand_position);
            if (distance_from_grab > precision_distance){
                velocity *= distance_from_grab * distance_from_grab / precision_distance;
            }
            //Debug.Log(velocity);
            transform.position += velocity;
            last_grab_position = current_hand_position;
        }
        
    }
}
