using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnCarPartsType : MonoBehaviour
{
    private CarPartType carPartType;

    [SerializeField]
    private Image imageCarPart;

    [SerializeField]
    private Text textName;

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
    public void SetCarPartType(CarPartType dataCarPartType)
    {
        carPartType = dataCarPartType;
        imageCarPart.sprite = carPartType.hudSprite;
        textName.text = dataCarPartType.partType.ToString();
    }

    /// <summary>
    /// Selecciona el bot�n
    // /// </summary>
    // public void Select()
    // {
    //     ToolController.setPlanta(planta);
    // }

    // private void OnChangePlanta()
    // {
    //     Selected = ToolController.planta == planta;
    // }

    // private void OnEnable()
    // {
    //     ToolController.OnSetPlanta += OnChangePlanta;
    // }

    // private void OnDisable()
    // {
    //     ToolController.OnSetPlanta -= OnChangePlanta;
    // }
}
