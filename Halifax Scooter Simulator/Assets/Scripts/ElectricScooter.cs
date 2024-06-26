using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ElectricScooter : MonoBehaviour
{
    public WheelCollider frontCollider, rearCollider;

    public Transform frontWheel, backWheel;

    public Transform handle;

    public float steeringAngle = 60;

    public float motorPower = 25;

    public float steering;

    private float horizontal;

    private float vertical;

    private float currentVelocity;
    public float maxSpeed = 15;

    public float maxTorque = 10f;
    public float rotationThreshold = 10f;
    public float slantFactor = 0.3f;

    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        currentVelocity = rb.velocity.magnitude;
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
        print(currentVelocity);
        if (currentVelocity > maxSpeed - 1 && vertical > 0)
        {
            rearCollider.motorTorque = 0;
        }
        else
        {
            rearCollider.motorTorque = vertical * motorPower;
        }
    }

    void Steer()
    {
        //Steering angle should depend on current velocity
        float ratio = Math.Abs(1 - (rb.velocity.magnitude / maxSpeed));
        
        steering = steeringAngle * horizontal * ratio;
        frontCollider.steerAngle = steering;
        
        
        float slantAngle = -steering * slantFactor * (1 - ratio); // Adjust slantFactor to control the slant intensity

        // Rotate the main body around its local forward axis
        Vector3 rotation = new Vector3(slantAngle, transform.eulerAngles.y, transform.eulerAngles.z); // Rotate around the x-axis
        transform.localRotation = Quaternion.Euler(rotation);
        
        
    }

    void UpdateWheelPositions(WheelCollider wc, Transform t)
    {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;
        
        wc.GetWorldPose(out pos, out rot);
        t.position = pos;
        t.rotation = rot;
    }

    void BalanceSelf()
    {
        float currentRotation = transform.eulerAngles.x;
        print(currentRotation);
        // Calculate torque direction based on the sign of the rotation
        float torqueDirection = Mathf.Sign(currentRotation);

        // Calculate torque based on the difference between current rotation and threshold
        float torque = 0f;
        if (Mathf.Abs(currentRotation) > rotationThreshold)
        {
            torque = Mathf.Clamp((Mathf.Abs(currentRotation) - rotationThreshold) * maxTorque, 0f, maxTorque) * torqueDirection;
            if (currentRotation < 90) torque *= -1;
        }

        // Apply torque to the bike's rigidbody around the z-axis (local right direction)
        rb.AddTorque(transform.right * torque);
    }
}
