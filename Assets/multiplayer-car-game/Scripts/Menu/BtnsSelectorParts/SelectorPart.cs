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
    private int arrayMoved = 0;
    private CarPartType currentPartType;
    private void Start()
    {
        UpdateCarPartsButtons();
    }

    private void UpdateCarPartsButtons()
    {
        CarPartType carPartType = btnParent.GetComponent<BtnCarPartsType>().GetPartType();
        for (int i = 0; i < carPartType.carPartList.Count; i++)
        {
            if (i < carPartType.carPartList.Count  && carPartType.carPartList.Count != 0)
            {
                GameObject btn = Instantiate(btnCarPart, contentParent);
                btn.GetComponent<BtnPartSelector>().SetCarPart(carPartType.carPartList[i]);
            }
        }
    }
    private void Update()
    {
        MovingArray();
    }
    private void MovingArray()
    {
        currentPartType = ToolControllerCarParts.CarPartType;
        if (Input.GetKeyDown(KeyCode.D) && arrayMoved < currentPartType.carPartList.Count-1)
        {
            arrayMoved++;
        }
        if (Input.GetKeyDown(KeyCode.A) && arrayMoved > 0)
        {
            arrayMoved--;
        }
    }
}
