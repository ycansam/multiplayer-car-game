using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class CameraController : NetworkBehaviour
{
    [Header("Camera Conf")]
    [SerializeField] Transform[] cameraViews;
    private int currentCamera = 0;
    [SerializeField] float followSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform lookAtPoint;
    private GameObject[] viewsStartPos;


    [Header("Car Conf")]
    [SerializeField] Rigidbody carRb;

    void Start()
    {
        if (IsLocalPlayer)
        {
            viewsStartPos = new GameObject[cameraViews.Length];
            for (int i = 0; i < cameraViews.Length; i++)
            {
                viewsStartPos[i] = new GameObject();
                viewsStartPos[i].name = cameraViews[i].gameObject.name + "-StartPoint";
                viewsStartPos[i].transform.position = cameraViews[i].position;
                viewsStartPos[i].transform.rotation = cameraViews[i].rotation;
                viewsStartPos[i].gameObject.transform.parent = transform.parent;
            }
            transform.parent = null;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (IsLocalPlayer)
        {
            if (currentCamera < 2)
            {

                cameraViews[currentCamera].position = Vector3.Lerp(cameraViews[currentCamera].position, new Vector3(cameraViews[currentCamera].position.x, viewsStartPos[currentCamera].transform.position.y, cameraViews[currentCamera].position.z), followSpeed - 29 * Time.fixedDeltaTime);
                var targetRotation = Quaternion.LookRotation(lookAtPoint.position - transform.position);
                // Smoothly rotate towards the target point.
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

                cameraViews[currentCamera].RotateAround(lookAtPoint.position, new Vector3(0f, 1f, 0f), transform.InverseTransformDirection(carRb.velocity).x/2);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(cameraViews[currentCamera].rotation.x, carRb.rotation.y, cameraViews[currentCamera].rotation.z, carRb.rotation.w), rotationSpeed * Time.fixedDeltaTime);
            }
            transform.position = Vector3.Lerp(transform.position, ChangeCamView().position, followSpeed * Time.fixedDeltaTime);
        }
    }

    private Transform ChangeCamView()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentCamera <= cameraViews.Length)
            {
                currentCamera++;
                if (currentCamera == cameraViews.Length)
                {
                    currentCamera = 0;
                }
            }
        }
        return cameraViews[currentCamera];
    }
}
