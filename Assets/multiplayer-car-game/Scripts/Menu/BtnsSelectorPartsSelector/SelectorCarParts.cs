using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectorCarParts : MonoBehaviour
{
    [SerializeField]
    private GameObject btnCarParts;
    [SerializeField] private Inventory inventory;
    private List<GameObject> arrayButtons = new List<GameObject>();
    private int currentPart;
    private int arrayMoved = 0;
    // Start is called before the first frame update
    void Start()
    {
        ToolControllerCarParts.positionSelected = 2;
        currentPart = ToolControllerCarParts.positionSelected;
        for (int i = 0; i < 5; i++)
        {
            CarPartType carPartType = inventory.CarParts.CarPartsTypesList[i];
            GameObject btn = Instantiate(btnCarParts, gameObject.transform);
            arrayButtons.Add(btn);
            btn.GetComponent<BtnCarPartsType>().SetCarPartType(carPartType, false);
            btn.GetComponent<Button>().onClick.Invoke();
        }
        arrayButtons[ToolControllerCarParts.positionSelected].GetComponent<Button>().Select();
        arrayButtons[ToolControllerCarParts.positionSelected].GetComponent<Button>().onClick.Invoke();
    }

    private void Update()
    {
        ChangePartSelection();
    }
    private void UpdateButtons()
    {
        if (currentPart > ToolControllerCarParts.positionSelected)
        {
            for (int i = 0; i < arrayButtons.Count; i++)
            {
                if (i + arrayMoved > inventory.CarParts.CarPartsTypesList.Count - 1)
                {
                    CarPartType carPartType = inventory.CarParts.CarPartsTypesList[i + arrayMoved - inventory.CarParts.CarPartsTypesList.Count];
                    arrayButtons[i].GetComponent<BtnCarPartsType>().SetCarPartType(carPartType, true);
                }
                else
                {
                    if (i + arrayMoved > -1)
                    {
                        CarPartType carPartType = inventory.CarParts.CarPartsTypesList[i + arrayMoved];
                        arrayButtons[i].GetComponent<BtnCarPartsType>().SetCarPartType(carPartType, false);
                    }
                    
                }

                Debug.Log(i + arrayMoved);
            }
            arrayButtons[ToolControllerCarParts.positionSelected].GetComponent<Button>().Select();
            arrayButtons[ToolControllerCarParts.positionSelected].GetComponent<Button>().onClick.Invoke();
            currentPart = ToolControllerCarParts.positionSelected;
        }
        if (currentPart < ToolControllerCarParts.positionSelected)
        {
            for (int i = 0; i < arrayButtons.Count; i++)
            {

                if (i + arrayMoved < 0)
                {
                    CarPartType carPartType = inventory.CarParts.CarPartsTypesList[i + arrayMoved + inventory.CarParts.CarPartsTypesList.Count];
                    arrayButtons[i].GetComponent<BtnCarPartsType>().SetCarPartType(carPartType, true);
                }
                else
                {
                    if (i + arrayMoved != inventory.CarParts.CarPartsTypesList.Count)
                    {
                        CarPartType carPartType = inventory.CarParts.CarPartsTypesList[i + arrayMoved];
                        arrayButtons[i].GetComponent<BtnCarPartsType>().SetCarPartType(carPartType, false);
                    }

                }
                Debug.Log(i + arrayMoved);
            }
            arrayButtons[ToolControllerCarParts.positionSelected].GetComponent<Button>().Select();
            arrayButtons[ToolControllerCarParts.positionSelected].GetComponent<Button>().onClick.Invoke();
            currentPart = ToolControllerCarParts.positionSelected;
        }
    }

    private void OnChangeSelection(int pos)
    {
        arrayButtons[pos].GetComponent<Button>().Select();
        arrayButtons[pos].GetComponent<Button>().onClick.Invoke();
    }
    bool activated = false;
    private void ChangePartSelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw(GameConstants.VERTICAL) == 1 && !activated)
        {
            activated = true;
            if (currentPart > 0 && currentPart + arrayMoved > 0)
            {
                currentPart--;
                arrayMoved--;
            }
            OnChangeSelection(currentPart);
            UpdateButtons();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxisRaw(GameConstants.VERTICAL) == -1 && !activated)
        {
            activated = true;
            if (currentPart < arrayButtons.Count - 1 && currentPart + arrayMoved < inventory.CarParts.CarPartsTypesList.Count - 1)
            {
                currentPart++;
                arrayMoved++;
            }
            OnChangeSelection(currentPart);
            UpdateButtons();

        }

        if(Input.GetAxisRaw(GameConstants.VERTICAL) == 0){
            activated = false;
        }
    }
}
