using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarChasis : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;
    public MeshFilter carMesh;
    public int carHeight;
    [SerializeField] Transform tFrontLeft, tFrontRight, tRearLeft, tRearRight;
    private PartsPositionsByChasis partsPositionsByChasis;
    private void Start()
    {
        meshFilter.mesh = carMesh.sharedMesh;
        // listener to change part
        ToolControllerCarPart.partSelected.AddListener(SetNewPart);
    }

    private void SetNewPart()
    {
        partsPositionsByChasis = ToolControllerCarPart.chasis.prefab.GetComponent<PartsPositionsByChasis>();

        // setting up mesh render
        carMesh = ToolControllerCarPart.chasis.prefab.GetComponent<MeshFilter>();
        meshFilter.mesh = carMesh.sharedMesh;
        // setting up mesh collider
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = ToolControllerCarPart.chasis.prefab.GetComponent<MeshCollider>().sharedMesh;

        // change scale to prevent errors.
        transform.localScale = ToolControllerCarPart.chasis.prefab.transform.localScale;
        SetNewPositionsWheels();
    }

    private void SetNewPositionsWheels()
    {



        tFrontLeft.localPosition = partsPositionsByChasis.Wheel_FrontLeft.position;
        tFrontRight.localPosition = partsPositionsByChasis.Wheel_FrontRight.position;
        tRearLeft.localPosition = partsPositionsByChasis.Wheel_RearLeft.position;
        tRearRight.localPosition = partsPositionsByChasis.Wheel_RearRight.position;
    }
}
