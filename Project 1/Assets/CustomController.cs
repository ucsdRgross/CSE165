using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Oculus.Interaction.Input;

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
    public GameObject leftPivot;
    public GameObject rightPivot;
    enum HandState {None, GrabbingAir, RotationGrabbing, GrabbingObject, ScalingObject};
    private HandState leftHandState = HandState.None;
    private Vector3 left_og_grab_pos;
    private Vector3 left_last_grab_pos;
    private HandState rightHandState = HandState.None;
    private Vector3 right_og_grab_pos;
    private Vector3 right_last_grab_pos;
    private float last_angle;
    public float precision_distance = 0.1f;
    public float palm_down_precision = 0.1f;
    [SerializeField] LineRenderer leftLine;
    [SerializeField] LineRenderer rightLine;
    private RaycastHit leftHit;
    private RaycastHit rightHit;
    private GameObject leftSelected;
    private Vector3 leftSelectedOffset;
    private Vector3 leftSelectedPoint;
    private Vector3 leftLocalOffset;
    private float leftInitialScale;
    private Vector3 leftSelectedScale;
    private GameObject rightSelected;
    private Vector3 rightSelectedOffset;
    private Vector3 rightSelectedPoint;
    private Vector3 rightLocalOffset;
    private float rightInitialScale;
    private Vector3 rightSelectedScale;
    
    void Start()
    {

    }

    void Update()
    {
        bool left_hand_grabbing = OVRInput.Get(OVRInput.RawButton.LHandTrigger);
        bool right_hand_grabbing = OVRInput.Get(OVRInput.RawButton.RHandTrigger);
        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger)){
            if (leftSelected && leftHandState == HandState.GrabbingObject && rightHandState != HandState.ScalingObject){
                leftPivot.transform.localPosition = leftLocalOffset;
            }
        }
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger)){
            if (rightSelected && rightHandState == HandState.GrabbingObject && leftHandState != HandState.ScalingObject){
                rightPivot.transform.localPosition = rightLocalOffset;
            }
        }
        if (!left_hand_grabbing){
            //hands become green when palm down to show that grab will become rotation
            //left hand
            if (Vector3.Dot(leftAnchor.right, Vector3.down) >= 1.0 - palm_down_precision){
                leftHand.GetComponent<Renderer>().material.color = Color.green;
                leftHandState = HandState.RotationGrabbing;
                leftLine.enabled=false;
            } else {
                leftHand.GetComponent<Renderer>().material.color = Color.white;
                leftHandState = HandState.None;
                leftLine.enabled = true;
                leftLine.SetPosition(0, leftAnchor.position);
                leftLine.SetPosition(1, leftAnchor.position + leftAnchor.forward * 10);
                if (Physics.Raycast(leftAnchor.position, leftAnchor.forward, out leftHit, 10, 1<<6)){
                    leftLine.SetPosition(1, leftHit.point);
                    leftLine.startColor=Color.blue;
                    leftLine.endColor=Color.blue;
                    if (leftSelected && leftSelected != leftHit.transform.gameObject){
                        leftSelected.GetComponent<Renderer>().material.color = Color.white;
                        leftSelected.GetComponent<Rigidbody>().isKinematic = false;
                    }
                    leftSelected = leftHit.transform.gameObject;
                    leftSelected.GetComponent<Renderer>().material.color = Color.yellow;
                    leftSelectedOffset = leftSelected.transform.position - leftHit.point;
                    leftSelectedPoint = leftHit.point;
                } else {
                    leftLine.startColor=Color.magenta;
                    leftLine.endColor=Color.magenta;
                    if (leftSelected){
                        leftSelected.GetComponent<Renderer>().material.color = Color.white;
                        leftSelected.GetComponent<Rigidbody>().isKinematic = false;
                        leftSelected = null;
                    }
                }
                
            }
        } else {
            leftLine.enabled=false;
        }
        if (!right_hand_grabbing){
            //right hand
            if (Vector3.Dot(-rightAnchor.right, Vector3.down) >= 1.0 - palm_down_precision){
                rightHand.GetComponent<Renderer>().material.color = Color.green;
                rightHandState = HandState.RotationGrabbing;
                rightLine.enabled=false;
            } else {
                rightHand.GetComponent<Renderer>().material.color = Color.white;
                rightHandState = HandState.None;
                rightLine.enabled = true;
                rightLine.SetPosition(0, rightAnchor.position);
                rightLine.SetPosition(1, rightAnchor.position + rightAnchor.forward * 10);
                if (Physics.Raycast(rightAnchor.position, rightAnchor.forward, out rightHit, 10, 1<<6)){
                    rightLine.SetPosition(1, rightHit.point);
                    rightLine.startColor=Color.blue;
                    rightLine.endColor=Color.blue;
                    if (rightSelected && rightSelected != rightHit.transform.gameObject){
                        rightSelected.GetComponent<Renderer>().material.color = Color.white;
                        rightSelected.GetComponent<Rigidbody>().isKinematic = false;
                    }
                    rightSelected = rightHit.transform.gameObject;
                    rightSelected.GetComponent<Renderer>().material.color = Color.yellow;
                    rightSelectedOffset = rightSelected.transform.position - rightHit.point;
                    rightSelectedPoint = rightHit.point;
                } else {
                    rightLine.startColor=Color.magenta;
                    rightLine.endColor=Color.magenta;
                    if (rightSelected){
                        rightSelected.GetComponent<Renderer>().material.color = Color.white;
                        rightSelected.GetComponent<Rigidbody>().isKinematic = false;
                        rightSelected = null;
                    }
                }
            }
        } else {
            rightLine.enabled=false;
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
                    if (leftSelected) {
                        leftPivot.transform.position = leftSelectedOffset + leftAnchor.position;
                        leftLocalOffset = leftPivot.transform.localPosition;
                        leftPivot.transform.position = leftSelectedOffset + leftSelectedPoint;
                        leftPivot.transform.rotation = leftSelected.transform.rotation;
                        if (rightSelected && rightSelected == leftSelected && rightHandState == HandState.GrabbingObject){
                            leftHandState = HandState.ScalingObject;
                            leftInitialScale = Vector3.Distance(leftAnchor.transform.position, rightAnchor.transform.position);
                            leftSelectedScale = leftSelected.transform.localScale;
                        } else {
                            leftHandState = HandState.GrabbingObject;
                            leftSelected.GetComponent<Rigidbody>().isKinematic = true;
                            leftLine.enabled = false;
                        }
                        
                    } else {
                        leftHandState = HandState.GrabbingAir;
                        rightHand.GetComponent<Renderer>().material.color = new Color(1f,1.0f,1f,1f);
                        if (rightHandState != HandState.GrabbingObject){
                            rightHandState = HandState.None;
                        }
                    }
                    break;
                case HandState.RotationGrabbing:
                    if (rightHandState != HandState.GrabbingObject){
                        rightHandState = HandState.None;
                    }
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
                    if (rightSelected) {
                        rightPivot.transform.position = rightSelectedOffset + rightAnchor.position;
                        rightLocalOffset = rightPivot.transform.localPosition;
                        rightPivot.transform.position = rightSelectedOffset + rightSelectedPoint;
                        rightPivot.transform.rotation = rightSelected.transform.rotation;
                        if (leftSelected && rightSelected == leftSelected && leftHandState == HandState.GrabbingObject){
                            rightHandState = HandState.ScalingObject;
                            rightInitialScale = Vector3.Distance(leftAnchor.transform.position, rightAnchor.transform.position);
                            rightSelectedScale = rightSelected.transform.localScale;
                        } else {
                            rightHandState = HandState.GrabbingObject;
                            rightSelected.GetComponent<Rigidbody>().isKinematic = true;
                            rightLine.enabled = false;
                        }
                    } else {
                        rightHandState = HandState.GrabbingAir;
                        leftHand.GetComponent<Renderer>().material.color = new Color(1f,1.0f,1f,1f);
                        if (leftHandState != HandState.GrabbingObject){
                            leftHandState = HandState.None;
                        }
                    }
                    break;
                case HandState.RotationGrabbing:
                    if (leftHandState != HandState.GrabbingObject){
                        leftHandState = HandState.None;
                    }
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
                    float angle = Vector3.SignedAngle(left_last_grab_pos, current_hand_position, Vector3.up);
                    angle -= last_angle;
                    transform.RotateAround(head.position,Vector3.up,angle);
                    last_angle = angle;
                    break;
                case HandState.GrabbingObject:
                    leftSelected.GetComponent<Renderer>().material.color = Color.red;
                    leftSelected.transform.position = leftPivot.transform.position;
                    leftSelected.transform.rotation = leftPivot.transform.rotation;
                    break;
                case HandState.ScalingObject:
                    float scaleDistance = Vector3.Distance(leftAnchor.transform.position, rightAnchor.transform.position);
                    float scaleFactor = scaleDistance / leftInitialScale;
                    leftSelected.transform.localScale = scaleFactor * leftSelectedScale;
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
                    float angle = Vector3.SignedAngle(right_last_grab_pos, current_hand_position, Vector3.up);
                    angle -= last_angle;
                    transform.RotateAround(head.position,Vector3.up,angle);
                    last_angle = angle;
                    break;
                case HandState.GrabbingObject:
                    rightSelected.GetComponent<Renderer>().material.color = Color.red;
                    rightSelected.transform.position = rightPivot.transform.position;
                    rightSelected.transform.rotation = rightPivot.transform.rotation;
                    break;
                case HandState.ScalingObject:
                    float scaleDistance = Vector3.Distance(leftAnchor.transform.position, rightAnchor.transform.position);
                    float scaleFactor = scaleDistance / rightInitialScale;
                    rightSelected.transform.localScale = scaleFactor * rightSelectedScale;
                    break;
            }
            right_last_grab_pos = current_hand_position;
        }
    }
}
