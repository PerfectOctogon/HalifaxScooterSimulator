using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ElectricScooter : MonoBehaviour
{
    public WheelCollider frontCollider, rearCollider;

    public Transform frontWheel, backWheel;

    public float steeringAngle = 60;

    public float motorPower = 25;

    public float steering;

    private float horizontal;

    private float vertical;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputManager();
        Drive();
        Steer();
        UpdateWheelPositions(frontCollider, frontWheel);
        UpdateWheelPositions(rearCollider, backWheel);
    }

    void InputManager()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    void Drive()
    {
        rearCollider.motorTorque = vertical * motorPower;
    }

    void Steer()
    {
        steering = steeringAngle * horizontal;
        frontCollider.steerAngle = steering;
    }

    void UpdateWheelPositions(WheelCollider wc, Transform t)
    {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;
        
        wc.GetWorldPose(out pos, out rot);
        t.position = pos;
        t.rotation = rot;
    }
}
