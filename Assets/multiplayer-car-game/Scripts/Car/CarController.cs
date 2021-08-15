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
    public float frictionMultiplier = 3f;
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    private WheelCollider[] wheelColliders = new WheelCollider[4];
    [SerializeField] bool canDrift;


    Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private float steeringAngle;
    private float tempo;
    public float handBrakeFrictionMultiplier = 2;
    private float handBrakeFriction = 0.05f;
    private WheelFrictionCurve forwardFriction, sidewaysFriction;
    [SerializeField] float maxStiffnes = 0.6f;

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
            maxSpeed = maxSpeed + 0.35f; // para evitar errores en el gui
            maxSpeedByGear = maxSpeed / GearRatio.Length; // maxima velocidad por marcha

            wheelColliders[0] = frontLeftWheelCollider;
            wheelColliders[1] = frontRightWheelCollider;
            wheelColliders[2] = rearLeftWheelCollider;
            wheelColliders[3] = rearRightWheelCollider;
            SetUpWheelColliders();
        }
    }
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            actualSpeed = rb.velocity.magnitude * 3.6f;

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
            AnimateWheels();
            if(!canDrift){
                Brakes();
            }else{
                checkWheelSpin();
            }
        }
    }
    public void GetInput()
    {
        horizontalInput = Input.GetAxis(GameConstants.HORIZONTAL);
        verticalInput = Input.GetAxis(GameConstants.VERTICAL);
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, 0f);
        }
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
            return (((left_whl.rpm + right_whl.rpm + left_whl_2.rpm + right_whl_2.rpm) / 2) * GearRatio.Length / GearRatio[currentGear]) / (motorForce / 1000);
        }
        else
        {
            // se calcula las whel rpm * Cantidad de marchas / marcha actual / fuerza motor / 1000.
            return ((left_whl.rpm + right_whl.rpm) * GearRatio.Length / GearRatio[currentGear]) / (motorForce / 1000);
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
                    // DisableMotor(-20f);
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
            if (actualSpeed < maxSpeedByGear * GearRatio[currentGear] && !isAutomatic && actualSpeed > maxSpeedByGear * GearRatio[currentGear] - maxSpeedByGear)
            {
                return 1;
            }
            else if (!isAutomatic)
            {
                return GearRatio[currentGear] * GearRatio[currentGear];
            }
            return 1;
        }


    }



    // wheels positions
    private void AnimateWheels()
    {
        AnimateWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        AnimateWheel(frontRightWheelCollider, frontRightWheeTransform);
        AnimateWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        AnimateWheel(rearRightWheelCollider, rearRightWheelTransform);
    }
    private void AnimateWheel(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }
    void checkWheelSpin()
    {
        float velocity = 0;

        if (!Input.GetKeyDown(KeyCode.Space))
        {
            forwardFriction = wheelColliders[0].forwardFriction;
            sidewaysFriction = wheelColliders[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = ((rb.velocity.magnitude * frictionMultiplier) / 300) + 1;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = ((rb.velocity.magnitude * frictionMultiplier) / 300) + 1;

            for (int i = 0; i < 2; i++)
            {
                wheelColliders[i].forwardFriction = forwardFriction;
                wheelColliders[i].sidewaysFriction = sidewaysFriction;

            }
        }
        else
        {
            sidewaysFriction = wheelColliders[0].sidewaysFriction;
            forwardFriction = wheelColliders[0].forwardFriction;

            velocity = 0;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = Mathf.SmoothDamp(sidewaysFriction.asymptoteValue, handBrakeFriction, ref velocity, 0.05f * Time.deltaTime);
            forwardFriction.extremumValue = forwardFriction.asymptoteValue = Mathf.SmoothDamp(forwardFriction.asymptoteValue, handBrakeFriction, ref velocity, 0.05f * Time.deltaTime);
            for (int i = 2; i < 4; i++)
            {
                wheelColliders[i].sidewaysFriction = sidewaysFriction;
                wheelColliders[i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = 1.5f;
            forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.5f;

            for (int i = 0; i < 2; i++)
            {
                wheelColliders[i].sidewaysFriction = sidewaysFriction;
                wheelColliders[i].forwardFriction = forwardFriction;
            }
        }


        for (int i = 2; i < 4; i++)
        {
            WheelHit wheelHit;

            wheelColliders[i].GetGroundHit(out wheelHit);
            if (wheelHit.sidewaysSlip < 0)
            {
                tempo = (1 + -horizontalInput) * Mathf.Abs(wheelHit.sidewaysSlip * handBrakeFrictionMultiplier);

                if (wheelHit.sidewaysSlip > -maxStiffnes)
                {
                    sidewaysFriction = wheelColliders[i].sidewaysFriction;
                    sidewaysFriction.stiffness = 1f - (-wheelHit.sidewaysSlip) / 2;
                    wheelColliders[i].sidewaysFriction = sidewaysFriction;
                }

            }
            if (tempo < 0.5) tempo = 0.5f;
            if (wheelHit.sidewaysSlip > 0)
            {
                tempo = (1 + horizontalInput) * Mathf.Abs(wheelHit.sidewaysSlip * handBrakeFrictionMultiplier);

                if (wheelHit.sidewaysSlip < maxStiffnes)
                {
                    sidewaysFriction = wheelColliders[i].sidewaysFriction;
                    sidewaysFriction.stiffness = 1f - wheelHit.sidewaysSlip / 2;
                    wheelColliders[i].sidewaysFriction = sidewaysFriction;
                }

            }
            if (tempo < 0.5) tempo = 0.5f;
        }
    }

    private void SetUpWheelColliders()
    {
        if (traccion == Traccion.n4x4 && canDrift)
        {
            SetUpWheelCollider(frontLeftWheelCollider, 1, 1);
            SetUpWheelCollider(frontRightWheelCollider, 1, 1);
            SetUpWheelCollider(rearLeftWheelCollider, 1, 0.5f);
            SetUpWheelCollider(rearRightWheelCollider, 1, 0.5f);
        }

        if(!canDrift){
            
        }
    }
    private void SetUpWheelCollider(WheelCollider wheelCo, float slipForward, float slipSideway)
    {

        forwardFriction = wheelCo.forwardFriction;
        sidewaysFriction = wheelCo.sidewaysFriction;

        forwardFriction.extremumSlip = slipForward;
        sidewaysFriction.extremumSlip = slipSideway;

        wheelCo.forwardFriction = forwardFriction;
        wheelCo.sidewaysFriction = sidewaysFriction;
    }
}
