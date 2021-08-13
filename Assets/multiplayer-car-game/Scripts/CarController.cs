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

    [SerializeField] private float[] GearRatio;
    private int currentGear = 0;
    private float engineRPM;
    [SerializeField] private float maxEngineRPM;
    [SerializeField] private float minEngineRPM;
    [SerializeField] private float maxSpeed;






    [Header("Ruedas")]
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;





    Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private float steeringAngle;

    private void OnGUI()
    {

        if (IsLocalPlayer)
        {
            GUILayout.Label("KPH: " + rb.velocity.magnitude * 3.6f);
            GUILayout.Label("RPM: " + engineRPM);
            GUILayout.Label("GEAR: " + GearRatio[currentGear]);
        }
    }
    private void Start()
    {
        if (!IsLocalPlayer)
        {
            cameraTransform.GetComponent<AudioListener>().enabled = false;
            cameraTransform.GetComponent<Camera>().enabled = false;
        }
        else
        {
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
            Brakes();
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
    private void Brakes()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            frontLeftWheelCollider.brakeTorque = breakForce;
            frontRightWheelCollider.brakeTorque = breakForce;
            rearLeftWheelCollider.brakeTorque = breakForce;
            rearRightWheelCollider.brakeTorque = breakForce;
        }
        else
        {
            frontLeftWheelCollider.brakeTorque = 0;
            frontRightWheelCollider.brakeTorque = 0;
            rearLeftWheelCollider.brakeTorque = 0;
            rearRightWheelCollider.brakeTorque = 0;
        }

    }
    private void Accelerate()
    {
        if (traccion == Traccion.Delantera)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;


            engineRPM = GetEngineRPM(frontLeftWheelCollider, frontRightWheelCollider);
            ShiftGears();
        }
        if (traccion == Traccion.Trasera)
        {
            rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
            rearRightWheelCollider.motorTorque = verticalInput * motorForce;


            engineRPM = GetEngineRPM(rearLeftWheelCollider, rearRightWheelCollider);
            ShiftGears();
        }
        if (traccion == Traccion.n4x4)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;
            rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
            rearRightWheelCollider.motorTorque = verticalInput * motorForce;
            engineRPM = GetEngineRPM(frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider);
            ShiftGears();
        }
    }

    private float GetEngineRPM(WheelCollider left_whl, WheelCollider right_whl, WheelCollider left_whl_2 = null, WheelCollider right_whl_2 = null)
    {
        if (left_whl_2 != null)
        {
            return (left_whl.rpm + right_whl.rpm + left_whl_2.rpm + right_whl_2.rpm) / 4 * GearRatio[currentGear];
        }
        else
        {
            return (left_whl.rpm + right_whl.rpm) * GearRatio.Length / GearRatio[currentGear];
        }
    }
    private void ShiftGears()
    {
        if (engineRPM >= maxEngineRPM)
        {
            if (engineRPM * GearRatio[currentGear] > maxEngineRPM)
            {
                if (currentGear < GearRatio.Length - 1)
                {
                    currentGear++;
                    engineRPM = minEngineRPM;
                }

            }
        }

        if (engineRPM <= minEngineRPM)
        {
            if (engineRPM * GearRatio[currentGear] < minEngineRPM)
            {
                if (currentGear > 1)
                {
                    currentGear--;
                    engineRPM = maxEngineRPM;
                }

            }
        }
    }



    // wheels positions
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
