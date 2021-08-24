using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorPart : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject btnCarPart;
    [SerializeField] private BtnCarPartsType btnParent;
    [SerializeField] private Transform contentParent;
    [HideInInspector] public int arrayMoved = 0;
    private CarPartType scriptPartType;
    private CarPartType currentPartType;
    private bool moved = false;
    private void Start()
    {
        UpdateCarPartsButtons();
    }

    private void UpdateCarPartsButtons()
    {
        CarPartType carPartType = btnParent.GetComponent<BtnCarPartsType>().GetPartType();
        scriptPartType = carPartType;
        for (int i = 0; i < carPartType.carPartList.Count; i++)
        {
            if (i < carPartType.carPartList.Count && carPartType.carPartList.Count != 0)
            {
                GameObject btn = Instantiate(btnCarPart, contentParent);
                btn.GetComponent<BtnPartSelector>().SetCarPart(carPartType.carPartList[i]);
            }
        }
    }
    private void Update()
    {
        currentPartType = ToolControllerCarParts.CarPartType;
        if (scriptPartType.partType == currentPartType.partType)
            MovingArray();
    }
    private void SelectArrayMovedForwards()
    {

        if (currentPartType.carPartList[0] is Chasis)
        {
            arrayMoved = ToolControllerCarPart.chasisPos;
            arrayMoved++;
            ToolControllerCarPart.chasisPos = arrayMoved;
        }
        else if (currentPartType.carPartList[0] is Motor)
        {
            arrayMoved = ToolControllerCarPart.motorPos;
            arrayMoved++;
            ToolControllerCarPart.motorPos = arrayMoved;
        }
    }
    private void SelectArrayMovedBackwards()
    {

        if (currentPartType.carPartList[0] is Chasis)
        {
            arrayMoved = ToolControllerCarPart.chasisPos;
            arrayMoved--;
            ToolControllerCarPart.chasisPos = arrayMoved;
        }
        else if (currentPartType.carPartList[0] is Motor)
        {
            arrayMoved = ToolControllerCarPart.motorPos;
            arrayMoved--;
            ToolControllerCarPart.motorPos = arrayMoved;
        }
    }
    private void MovingArray()
    {
        // mueve el array a la derecha
        if (Input.GetAxisRaw(GameConstants.HORIZONTAL) == 1 && arrayMoved < currentPartType.carPartList.Count - 1 && !moved)
        {
            moved = true;
            SelectArrayMovedForwards();
            // Debug.Log(arrayMoved);
            transform.GetChild(0).GetChild(arrayMoved).GetComponent<Button>().Select();
        }
        if (Input.GetAxisRaw(GameConstants.HORIZONTAL) == -1 && arrayMoved > 0 && currentPartType.carPartList.Count != 0 && !moved)
        {
            moved = true;
            SelectArrayMovedBackwards();
            // Debug.Log(arrayMoved);
            transform.GetChild(0).GetChild(arrayMoved).GetComponent<Button>().Select();
        }
        if (Input.GetAxisRaw(GameConstants.HORIZONTAL) == 0)
        {
            moved = false;
        }
    }
}
