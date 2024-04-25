using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class CustomController : MonoBehaviour
{
    public Transform head;
    public Transform leftAnchor;
    public Transform rightAnchor;
    public GameObject leftHand;
    public GameObject rightHand;
    enum HandState {None, GrabbingAir, RotationGrabbing, GrabbingObject};
    private HandState leftHandState = HandState.None;
    private Vector3 left_og_grab_pos;
    private Vector3 left_last_grab_pos;
    private HandState rightHandState = HandState.None;
    private Vector3 right_og_grab_pos;
    private Vector3 right_last_grab_pos;
    private float last_angle;
    public float precision_distance = 0.1f;
    //public Vector3 palm_down_angle = new Vector3(0f,0f,90f);
    public float palm_down_precision = 0.1f;
    void Start()
    {

    }

    void Update()
    {
        bool left_hand_grabbing = OVRInput.Get(OVRInput.RawButton.LHandTrigger);
        bool right_hand_grabbing = OVRInput.Get(OVRInput.RawButton.RHandTrigger);
        if (!left_hand_grabbing){
            //hands become green when palm down to show that grab will become rotation
            //left hand
            if (Vector3.Dot(leftAnchor.right, Vector3.down) >= 1.0 - palm_down_precision){
                leftHand.GetComponent<Renderer>().material.color = new Color(0f,1.0f,0f,1f);
                leftHandState = HandState.RotationGrabbing;
            } else {
                leftHand.GetComponent<Renderer>().material.color = new Color(1f,1.0f,1f,1f);
                leftHandState = HandState.None;
            }
        }
        if (!right_hand_grabbing){
            //right hand
            if (Vector3.Dot(-rightAnchor.right, Vector3.down) >= 1.0 - palm_down_precision){
                rightHand.GetComponent<Renderer>().material.color = new Color(0f,1.0f,0f,1f);
                rightHandState = HandState.RotationGrabbing;
            } else {
                rightHand.GetComponent<Renderer>().material.color = new Color(1f,1.0f,1f,1f);
                rightHandState = HandState.None;
            }
        }
        //grab air to pull forwards
        if (OVRInput.GetUp(OVRInput.RawButton.LHandTrigger)){
            leftHandState = HandState.None;
        }
        if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger)){
            rightHandState = HandState.None;
        }
        if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger)){
            switch (leftHandState){
                case HandState.None:
                    leftHandState = HandState.GrabbingAir;
                    rightHandState = HandState.None;
                    break;
                case HandState.RotationGrabbing:
                        rightHandState = HandState.None;
                    break;
            }
            left_last_grab_pos = leftAnchor.position;
            left_last_grab_pos -= head.position;
            left_og_grab_pos = left_last_grab_pos;
            last_angle = 0;
        } 
        if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)){
            switch (rightHandState){
                case HandState.None:
                    rightHandState = HandState.GrabbingAir;
                    leftHandState = HandState.None;
                    break;
                case HandState.RotationGrabbing:
                    leftHandState = HandState.None;
                    break;
            }
            right_last_grab_pos = rightAnchor.position;
            right_last_grab_pos -= head.position;
            right_og_grab_pos = right_last_grab_pos;
            last_angle = 0;
        } 
        //Debug.Log(Vector3.Dot(-rightAnchor.right, Vector3.down));
        if (left_hand_grabbing){
            Vector3 current_hand_position = leftAnchor.position;
            current_hand_position -= head.position;
            Vector3 velocity = left_last_grab_pos - current_hand_position;
            switch (leftHandState){
                case HandState.None:
                    break;
                case HandState.GrabbingAir:
                    float distance_from_grab = Vector3.Distance(left_og_grab_pos, current_hand_position);
                    if (distance_from_grab > precision_distance){
                        velocity *= Mathf.Pow(distance_from_grab / precision_distance,1.5f);
                    }
                    transform.position += velocity;
                    break;
                case HandState.RotationGrabbing:
                    float angle = -Vector3.SignedAngle(left_last_grab_pos, current_hand_position, Vector3.up);
                    angle += last_angle;
                    transform.RotateAround(head.position,Vector3.up,angle);
                    last_angle = angle;
                    break;
            }
            left_last_grab_pos = current_hand_position;
        }
        if (right_hand_grabbing){
            Vector3 current_hand_position = rightAnchor.position;
            current_hand_position -= head.position;
            Vector3 velocity = right_last_grab_pos - current_hand_position;
            switch (rightHandState){
                case HandState.None:
                    break;
                case HandState.GrabbingAir:
                    float distance_from_grab = Vector3.Distance(right_og_grab_pos, current_hand_position);
                    if (distance_from_grab > precision_distance){
                        velocity *= Mathf.Pow(distance_from_grab / precision_distance,1.5f);
                    }
                    transform.position += velocity;
                    break;
                case HandState.RotationGrabbing:
                    float angle = -Vector3.SignedAngle(right_last_grab_pos, current_hand_position, Vector3.up);
                    angle += last_angle;
                    transform.RotateAround(head.position,Vector3.up,angle);
                    last_angle = angle;
                    break;
            }
            right_last_grab_pos = current_hand_position;
        }
    }
}
