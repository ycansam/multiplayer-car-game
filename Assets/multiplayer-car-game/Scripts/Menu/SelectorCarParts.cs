using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorCarParts : MonoBehaviour
{
     [SerializeField]
    private GameObject btnCarParts;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("B");
        foreach (CarPartType carPartType in Inventory.Instance.CarParts.CarPartsTypesList)
        {
            Debug.Log("A");
            GameObject btn = Instantiate(btnCarParts, gameObject.transform);
            btn.GetComponent<BtnCarPartsType>().SetCarPartType(carPartType);
        }
    }
}
