using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;


public class CameraInventory : NetworkBehaviour
{
    [Header("Camera options")]
    [SerializeField] private float speed;
    [Header("Transforms")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform lookPoint;
    [SerializeField] private Transform mainCamera;
    private Transform referenceTransform;

    // Others
    private CameraController cameraController;
    private bool menuOpened;

    private void Start()
    {
        if (!IsLocalPlayer)
        {
            transform.GetComponent<Camera>().enabled = false;
        }
        else
        {
            cameraController = mainCamera.GetComponent<CameraController>();
            referenceTransform = cameraController.GetCurrentCamera();
            transform.GetComponent<Camera>().enabled = false;
        }
    }
    private void FixedUpdate()
    {
        ChangeCameraPos();
        GoingOriginalPos();
    }
    private void GoingOriginalPos()
    {
        if (referenceTransform == cameraController.GetCurrentCamera())
        {
            transform.position = Vector3.Lerp(transform.position, mainCamera.position, speed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, mainCamera.rotation, speed*2 * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, mainCamera.position) < 0.2f)
            {
                transform.GetComponent<Camera>().enabled = false;
                menuOpened = false;
            }
        }
        else
        {
            transform.LookAt(lookPoint);
        }

    }
    private void ChangeCameraPos()
    {
        if (IsLocalPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, referenceTransform.position, speed * Time.fixedDeltaTime);

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!menuOpened)
                {
                    menuOpened = true;
                    referenceTransform = cameraPosition;
                    transform.GetComponent<Camera>().enabled = true;
                }
                else
                {
                    referenceTransform = cameraController.GetCurrentCamera();
                }
            }
        }
    }



}