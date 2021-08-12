using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using MLAPI;
public class CarController : NetworkBehaviour
{
    enum Traccion { Trasera, Delantera, n4x4 };
    [SerializeField] Transform cameraTransform;
    [SerializeField] Traccion traccion;
    

    [Header("Motor")]
    [SerializeField] private float motorForce = 50;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle = 30;

    [Header("Ruedas")]
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private float steeringAngle;
    private void Start()
    {
        if(!IsLocalPlayer){
            cameraTransform.GetComponent<AudioListener>().enabled = false;
            cameraTransform.GetComponent<Camera>().enabled = false;
        }else{
            rb = GetComponent<Rigidbody>();
        }
    }
    private void FixedUpdate()
    {
        
        if (IsLocalPlayer)
        {
            GetInput();
            Steer();
            Accelerate();
            UpdateWheelPoses();
        }

    }
    public void GetInput()
    {
        horizontalInput = Input.GetAxis(GameConstants.HORIZONTAL);
        verticalInput = Input.GetAxis(GameConstants.VERTICAL);
    }
    private void Steer()
    {
        steeringAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = steeringAngle;
        frontRightWheelCollider.steerAngle = steeringAngle;
    }
    private void Accelerate()
    {
        if (Traccion.Delantera == traccion)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        }
        if (Traccion.Trasera == traccion)
        {
            rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
            rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        }
        if (traccion == Traccion.n4x4)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;
            rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
            rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        }


    }
    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelPose(frontRightWheelCollider, frontRightWheeTransform);
        UpdateWheelPose(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelPose(rearRightWheelCollider, rearRightWheelTransform);
    }
    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }
}
