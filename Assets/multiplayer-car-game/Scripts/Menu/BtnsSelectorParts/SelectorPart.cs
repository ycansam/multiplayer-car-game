using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorPart : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject btnCarPart;
    [SerializeField] private BtnCarPartsType btnParent;

    private void Start()
    {
        // referencia a su boton padre // motor, chasis, aleron, etc.
        CarPartType carPartType = btnParent.GetComponent<BtnCarPartsType>().GetPartType();
        Debug.Log(carPartType.carPartList.Count);
        for (int i = 0; i < 4; i++)
        {
            if ( i < carPartType.carPartList.Count-1  && carPartType.carPartList.Count != 0){
                GameObject btn = Instantiate(btnCarPart, gameObject.transform);
                btn.GetComponent<BtnPartSelector>().SetCarPart(carPartType.carPartList[i]);
            }
                
        }
    }

}
