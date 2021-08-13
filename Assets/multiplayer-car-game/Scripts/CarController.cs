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
    [SerializeField] bool isAutomatic = true;


    [Header("Motor")]
    [SerializeField] private float motorForce = 50;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle = 30;
    [SerializeField] private float maxSpeed;
    private float actualSpeed;
    [SerializeField] private float[] GearRatio;
    private int currentGear = 0;
    private float engineRPM;
    [SerializeField] private float maxEngineRPM;
    [SerializeField] private float minEngineRPM;
    private float maxSpeedByGear;



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
            GUILayout.Label("KPH: " + actualSpeed);
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
        maxSpeed = maxSpeed + 0.35f; // para evitar errores en el gui
        maxSpeedByGear = maxSpeed / GearRatio.Length; // maxima velocidad por marcha
    }
    private void FixedUpdate()
    {
        actualSpeed = rb.velocity.magnitude * 3.6f;
        if (IsLocalPlayer)
        {
            GetInput();
            Steer();
            Accelerate(actualSpeed);
            if (isAutomatic)
            {
                ShiftGearsAuto();
            }
            else
            {
                ShiftGearManual();
            }
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
            frontLeftWheelCollider.brakeTorque = brakeForce;
            frontRightWheelCollider.brakeTorque = brakeForce;
            rearLeftWheelCollider.brakeTorque = brakeForce;
            rearRightWheelCollider.brakeTorque = brakeForce;
        }
        else
        {
            frontLeftWheelCollider.brakeTorque = 0;
            frontRightWheelCollider.brakeTorque = 0;
            rearLeftWheelCollider.brakeTorque = 0;
            rearRightWheelCollider.brakeTorque = 0;
        }

        //  si esta en marcha y le da a frenar con la "S"
        if (rearRightWheelCollider.rpm > 0 && verticalInput < 0)
        {
            frontLeftWheelCollider.brakeTorque = brakeForce / 4;
            frontRightWheelCollider.brakeTorque = brakeForce / 4;
            rearLeftWheelCollider.brakeTorque = brakeForce / 4;
            rearRightWheelCollider.brakeTorque = brakeForce / 4;
        }
    }
    private void DisableMotor(float motorForceReduce)
    {
        // parando el motor cuando alcanza el limite
        frontLeftWheelCollider.motorTorque = motorForceReduce;
        frontRightWheelCollider.motorTorque = motorForceReduce;
        rearLeftWheelCollider.motorTorque = motorForceReduce;
        rearRightWheelCollider.motorTorque = motorForceReduce;
    }
    private void Accelerate(float actualSpeed)
    {
        if (actualSpeed < maxSpeed && !GoingBackwards())
        {
            if (traccion == Traccion.Delantera)
            {
                frontLeftWheelCollider.motorTorque = verticalInput * motorForce / CheckingCorrectGear();
                frontRightWheelCollider.motorTorque = verticalInput * motorForce / CheckingCorrectGear();

                engineRPM = GetEngineRPM(frontLeftWheelCollider, frontRightWheelCollider);
            }

            if (traccion == Traccion.Trasera)
            {
                Debug.Log(CheckingCorrectGear());
                rearLeftWheelCollider.motorTorque = verticalInput * motorForce / CheckingCorrectGear();
                rearRightWheelCollider.motorTorque = verticalInput * motorForce / CheckingCorrectGear();
                engineRPM = GetEngineRPM(rearLeftWheelCollider, rearRightWheelCollider);
            }

            if (traccion == Traccion.n4x4)
            {
                frontLeftWheelCollider.motorTorque = verticalInput * motorForce / CheckingCorrectGear() / 2;
                frontRightWheelCollider.motorTorque = verticalInput * motorForce / CheckingCorrectGear() / 2;
                rearLeftWheelCollider.motorTorque = verticalInput * motorForce / CheckingCorrectGear() / 2;
                rearRightWheelCollider.motorTorque = verticalInput * motorForce / CheckingCorrectGear() / 2;
                engineRPM = GetEngineRPM(frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider);
            }
        }
        else
        {
            DisableMotor(0f);
        }

    }
    private bool GoingBackwards()
    {
        float mediaRpm = (rearRightWheelCollider.rpm + frontRightWheelCollider.rpm + rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) / 4;
        if (actualSpeed > 50)
        {
            if (mediaRpm < 0)
            {
                DisableMotor(0f);
                return true;
            }
        }
        return false;
    }

    private float GetEngineRPM(WheelCollider left_whl, WheelCollider right_whl, WheelCollider left_whl_2 = null, WheelCollider right_whl_2 = null)
    {
        if (left_whl_2 != null)
        {
            return (((left_whl.rpm + right_whl.rpm + left_whl_2.rpm + right_whl_2.rpm) / 2) * GearRatio.Length / GearRatio[currentGear]) / (motorForce / 1000) ;
        }
        else
        {
            // se calcula las whel rpm * Cantidad de marchas / marcha actual / fuerza motor / 1000.
            return ((left_whl.rpm + right_whl.rpm) * GearRatio.Length / GearRatio[currentGear]) / (motorForce / 1000) ;
        }
    }
    private void ShiftGearsAuto()
    {
        if (engineRPM >= maxEngineRPM)
        {
            if (engineRPM * GearRatio[currentGear] > maxEngineRPM)
            {
                if (currentGear < GearRatio.Length - 1)
                {
                    DisableMotor(0f);
                    currentGear++;
                }

            }
        }
        if (engineRPM <= minEngineRPM)
        {
            if (engineRPM < minEngineRPM)
            {
                if (currentGear > 0)
                {
                    DisableMotor(0f);
                    currentGear--;
                }

            }
        }
    }
    private void ShiftGearManual()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentGear > 0)
            {
                currentGear--;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (currentGear < GearRatio.Length - 1)
            {
                currentGear++;
            }
        }
        if (actualSpeed > maxSpeedByGear * GearRatio[currentGear])
        {
            if (engineRPM >= maxEngineRPM - 400f)
            {
                DisableMotor(-500f);
            }
        }

    }
    private float CheckingCorrectGear()
    {
        // si la velocidad actual esta por encima de la velocidad permitida por marcha, reducira la velocidad dependiendo de la velocidad en la que este.
        if (actualSpeed > maxSpeedByGear * GearRatio[currentGear] && isAutomatic)
        {
            currentGear++;
            // reduccion exponencial
            return GearRatio[currentGear] * GearRatio[currentGear];
        }
        else
        {
            if(actualSpeed < maxSpeedByGear * GearRatio[currentGear] && !isAutomatic && actualSpeed >  maxSpeedByGear * GearRatio[currentGear] - maxSpeedByGear ){
                return 1;
            }else if(!isAutomatic){
                return GearRatio[currentGear] * GearRatio[currentGear];
            }
            return 1;
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
