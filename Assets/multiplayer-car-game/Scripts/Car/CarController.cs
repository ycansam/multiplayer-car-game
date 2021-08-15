using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using MLAPI;
[RequireComponent(typeof(WheelsController))]
[RequireComponent(typeof(Rigidbody))]
public class CarController : NetworkBehaviour
{
    
    [SerializeField] Transform cameraTransform;
    [SerializeField] bool isAutomatic = true;

    [Range(5, 20)] [SerializeField] private float downForceValue;
    private float downforce;
    [Header("Motor")]
    [SerializeField] private float motorForce = 50;
    [SerializeField] private float brakeForce;
    private float maxSteerAngle = 30;
    public float maxSpeed;
    [HideInInspector] public float actualSpeed;
    [SerializeField] private float[] GearRatio;
    private int currentGear = 0;
    private float engineRPM;
    [SerializeField] private float maxEngineRPM;
    [SerializeField] private float minEngineRPM;
    private float maxSpeedByGear;


    [Header("Ruedas")]
    Traccion traccion;
    private WheelsController wheelsController;
    private WheelCollider[] wheelColliders = new WheelCollider[4];
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
    private void Awake() {
        
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
            wheelsController = GetComponent<WheelsController>();
            rb = GetComponent<Rigidbody>();


            maxSteerAngle = wheelsController.maxSteerAngle;
            maxSpeed = maxSpeed + 0.35f; // para evitar errores en el gui
            maxSpeedByGear = maxSpeed / GearRatio.Length; // maxima velocidad por marcha
            wheelColliders = wheelsController.wheelColliders;
            traccion = wheelsController.traccion;
        }
    }
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            addDownForce();
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
            if (!wheelsController.canDrift)
            {
                Brakes();
            }
            checkWheelSpin();
        }
    }
    private void addDownForce()
    {
        downforce = Mathf.Abs(downForceValue * rb.velocity.magnitude);
        downforce = actualSpeed > 60 ? downforce : 0;
        rb.AddForce(-transform.up * downforce);

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
        wheelColliders[0].steerAngle = steeringAngle;
        wheelColliders[1].steerAngle = steeringAngle;

        // Endereza el coche
        rb.angularDrag = (actualSpeed > 100) ? actualSpeed / 100 : 0;
        rb.drag = 0.02f + (actualSpeed / 40000);
    }
    private void Brakes()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].brakeTorque = brakeForce;
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].brakeTorque = 0;
            }
        }

        //  si esta en marcha y le da a frenar con la "S"
        if (wheelColliders[2].rpm > 0 && verticalInput < 0 || wheelColliders[0].rpm > 0 && verticalInput < 0)
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].brakeTorque = brakeForce / 4; ;
            }
        }
    }
    private void DisableMotor(float motorForceReduce)
    {
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = motorForceReduce;
        }
        // parando el motor cuando alcanza el limite

    }
    private void Accelerate(float actualSpeed)
    {
        if (actualSpeed < maxSpeed && !GoingBackwards())
        {
            if (traccion == Traccion.Delantera)
            {
                wheelColliders[0].motorTorque = verticalInput * motorForce / CheckingCorrectGear();
                wheelColliders[1].motorTorque = verticalInput * motorForce / CheckingCorrectGear();

                engineRPM = GetEngineRPM(wheelColliders[0], wheelColliders[1]);
            }

            if (traccion == Traccion.Trasera)
            {
                wheelColliders[2].motorTorque = verticalInput * motorForce / CheckingCorrectGear();
                wheelColliders[3].motorTorque = verticalInput * motorForce / CheckingCorrectGear();
                engineRPM = GetEngineRPM(wheelColliders[2], wheelColliders[3]);
            }

            if (traccion == Traccion.n4x4)
            {
                for (int i = 0; i < 4; i++)
                {
                    wheelColliders[i].motorTorque = verticalInput * motorForce / CheckingCorrectGear() / 2;;
                }
                engineRPM = GetEngineRPM( wheelColliders[0],  wheelColliders[1],  wheelColliders[2],  wheelColliders[3]);
            }
        }
        else
        {
            DisableMotor(0f);
        }

    }
    private bool GoingBackwards()
    {
        float mediaRpm = (wheelColliders[0].rpm + wheelColliders[1].rpm + wheelColliders[2].rpm + wheelColliders[3].rpm) / 4;
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

    void checkWheelSpin()
    {
        
    }
}
