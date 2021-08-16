using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

[HideInInspector] public enum Traccion { Trasera, Delantera, n4x4 };

[RequireComponent(typeof(CarController))]
public class WheelsController : NetworkBehaviour
{
    [Header("Opciones")]
    public float frictionMultiplier = 3f;
    public float maxStiffnes = 0.6f;
    public float maxSteerAngle = 30;
    public bool canDrift;
    public Traccion traccion;
    public float handBrakeFrictionMultiplier = 2;
    private float handBrakeFriction = 0.05f;

    [Header("Ruedas")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;
    [HideInInspector] public WheelCollider[] wheelColliders = new WheelCollider[4];

    private WheelFrictionCurve forwardFriction, sidewaysFriction;

    // other
    private CarController carController;
    private float tempo;
    private float horizontalInput;
    private Rigidbody rb;

    private void Awake()
    {

        wheelColliders[0] = frontLeftWheelCollider;
        wheelColliders[1] = frontRightWheelCollider;
        wheelColliders[2] = rearLeftWheelCollider;
        wheelColliders[3] = rearRightWheelCollider;

    }
    private void Start()
    {
        if (IsLocalPlayer)
        {
            rb = GetComponent<Rigidbody>();
            carController = GetComponent<CarController>();
            SetUpWheelColliders();
        }
    }
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            AnimateWheels();
            CheckDrifting();
            GetInput();
        }


    }
    private void GetInput(){
        horizontalInput = Input.GetAxis(GameConstants.HORIZONTAL);
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

    private void SetUpWheelColliders()
    {
        if (canDrift)
        {
            for (int i = 0; i < 4; i++)
            {
                JointSpring susSpring = wheelColliders[i].suspensionSpring;
                susSpring.spring = 96600f;
                susSpring.damper = 9000f;
                wheelColliders[i].suspensionSpring = susSpring;

                forwardFriction = wheelColliders[i].forwardFriction;
                sidewaysFriction = wheelColliders[i].sidewaysFriction;

                // forward friction
                forwardFriction.asymptoteSlip = 0.8f;
                forwardFriction.asymptoteValue = 0.5f;
                forwardFriction.stiffness = 1f;


                // sideway Friction
                sidewaysFriction.asymptoteSlip = 0.5f;
                sidewaysFriction.asymptoteValue = 0.75f;
                sidewaysFriction.stiffness = 1f;

                wheelColliders[i].forwardFriction = forwardFriction;
                wheelColliders[i].sidewaysFriction = sidewaysFriction;

                if (i < 2)
                {
                    forwardFriction = wheelColliders[i].forwardFriction;
                    sidewaysFriction = wheelColliders[i].sidewaysFriction;

                    forwardFriction.extremumSlip = 1f;
                    sidewaysFriction.extremumSlip = 1f;

                    wheelColliders[i].forwardFriction = forwardFriction;
                    wheelColliders[i].sidewaysFriction = sidewaysFriction;
                }
                else
                {
                    forwardFriction = wheelColliders[i].forwardFriction;
                    sidewaysFriction = wheelColliders[i].sidewaysFriction;

                    forwardFriction.extremumSlip = 1f;
                    sidewaysFriction.extremumSlip = 0.5f;

                    wheelColliders[i].forwardFriction = forwardFriction;
                    wheelColliders[i].sidewaysFriction = sidewaysFriction;
                }

            }

        }

        if (!canDrift)
        {
            for (int i = 0; i < 4; i++)
            {

                JointSpring susSpring = wheelColliders[i].suspensionSpring;
                susSpring.spring = 120000f;
                susSpring.damper = 16000f;
                wheelColliders[i].suspensionSpring = susSpring;

                forwardFriction = wheelColliders[i].forwardFriction;
                sidewaysFriction = wheelColliders[i].sidewaysFriction;

                // forward friction
                forwardFriction.extremumSlip = 0.4f;
                forwardFriction.asymptoteSlip = 1.67f;
                forwardFriction.asymptoteValue = 0.55f;
                forwardFriction.stiffness = 1f;

                // sideway Friction

                sidewaysFriction.extremumSlip = 0.25f;
                sidewaysFriction.asymptoteSlip = 0.32f;
                sidewaysFriction.asymptoteValue = 0.45f;
                sidewaysFriction.stiffness = 1f;

                wheelColliders[i].forwardFriction = forwardFriction;
                wheelColliders[i].sidewaysFriction = sidewaysFriction;
            }
        }
    }


    private void CheckDrifting()
    {
        if (canDrift)
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


        float currentSpeed = carController.actualSpeed;
        if (!canDrift)
        {
            sidewaysFriction = wheelColliders[0].sidewaysFriction;
            forwardFriction = wheelColliders[0].forwardFriction;

            for (int i = 0; i < 4; i++)
            {
                forwardFriction = wheelColliders[i].forwardFriction;
                forwardFriction.extremumValue = 1.3f + currentSpeed / carController.maxSpeed * 1.16f;
                wheelColliders[i].forwardFriction = forwardFriction;

                sidewaysFriction = wheelColliders[i].sidewaysFriction;
                sidewaysFriction.extremumValue = 1 + currentSpeed / carController.maxSpeed * 0.6f;
                wheelColliders[i].sidewaysFriction = sidewaysFriction;

            }
        }
    }
}
