using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnCarPartsType : MonoBehaviour
{
    private CarPartType carPartType;
    [SerializeField] private Image imageCarPart; // indicar en el inspector
    [SerializeField] private Text textName; // indicar en el inspector que textos cambia
    private bool selected = false;

    /// <summary>
    /// Verdadero si el bot�n est� seleccionado.
    /// </summary>
    public bool Selected
    {
        get => selected; 
        private set
        {
            if (selected == value) return;
            selected = value;
            Color c = imageCarPart.color;
            c.a = selected ? 1f : .5f;
            imageCarPart.color = c;
        }
    }

    /// <summary>
    /// Asigna una lista de partes al bot�n
    /// </summary>
    /// <param name="datosCarPart">Objeto con los datos de las partes</param>
    public void SetCarPartType(CarPartType dataCarPartType, bool disableText)
    {
        carPartType = dataCarPartType;
        imageCarPart.sprite = carPartType.hudSprite;
        textName.text = dataCarPartType.partType.ToString();
        if(disableText)
            textName.text = "";
    }
    public CarPartType GetPartType(){
        return carPartType;
    }

    /// <summary>
    /// Selecciona el bot�n
    /// </summary>
    public void Select()
    {
        ToolControllerCarParts.SetCarPart(carPartType);
    }

    private void OnChangeCarPartTypes()
    {
        Selected = ToolControllerCarParts.CarPartType == carPartType;
    }

    private void OnEnable()
    {
        ToolControllerCarParts.OnSelectCarPartType += OnChangeCarPartTypes;
    }

    private void OnDisable()
    {
        ToolControllerCarParts.OnSelectCarPartType -= OnChangeCarPartTypes;
    }
}
