using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectorCarPartsv2 : MonoBehaviour
{
    [SerializeField]
    private GameObject btnCarParts;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform contentParent;
    [SerializeField] private float desplazamiento;
    private List<GameObject> arrayButtons = new List<GameObject>();
    private int currentPart;
    private int arrayMoved = 0;
    private enum Movement { NONE, UP, DOWN }
    private Movement movement;
    private float newMove;
    private float startMove;

    // Start is called before the first frame update
    void Start()
    {
        startMove =  contentParent.position.y-2f;
        ToolControllerCarParts.positionSelected = 2;
        currentPart = ToolControllerCarParts.positionSelected;

        for (int i = 0; i < inventory.CarParts.CarPartsTypesList.Count; i++)
        {
            CarPartType carPartType = inventory.CarParts.CarPartsTypesList[i];
            GameObject btn = Instantiate(btnCarParts, contentParent);
            if (i == 0)
                btn.transform.position = new Vector3(btn.transform.position.x, btn.transform.position.y + 2f, btn.transform.position.z);
            else
                btn.transform.position = new Vector3(btn.transform.position.x, btn.transform.position.y + 2f - ((float)i - 0.5f * i), btn.transform.position.z);

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
        MovingContent();
    }
    private void MovingContent()
    {
        switch (movement)
        {
            case Movement.UP:
                contentParent.position = Vector3.Lerp(contentParent.position, new Vector3(contentParent.position.x, newMove, contentParent.position.z), 5f * Time.deltaTime);
                if (contentParent.position.y == newMove)
                    movement = Movement.NONE; 
                break;
            case Movement.DOWN:
                contentParent.position = Vector3.Lerp(contentParent.position, new Vector3(contentParent.position.x, newMove, contentParent.position.z), 5f * Time.deltaTime);
                if (contentParent.position.y == newMove)
                    movement = Movement.NONE;
                break;
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
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw(GameConstants.VERTICAL) == 1 && !activated  )
        {
            activated = true;
            if (currentPart >= 0 && currentPart + arrayMoved >= 0)
            {
                currentPart--;
                arrayMoved--;
                movement = Movement.UP;
                newMove = startMove - desplazamiento*(-arrayMoved);

            }
            OnChangeSelection(currentPart);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxisRaw(GameConstants.VERTICAL) == -1 && !activated )
        {
            activated = true;
            if (currentPart <= inventory.CarParts.CarPartsTypesList.Count && currentPart + arrayMoved <= inventory.CarParts.CarPartsTypesList.Count)
            {
                currentPart++;
                arrayMoved++;
                movement = Movement.DOWN;
                newMove = startMove + desplazamiento*arrayMoved;               
            }
            OnChangeSelection(currentPart);
        }

        if (Input.GetAxisRaw(GameConstants.VERTICAL) == 0)
        {
            activated = false;
        }
    }
}
