using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnPartSelector : MonoBehaviour
{
    private CarPart carPart;

    [SerializeField] private Image imageCarPart;

    [SerializeField] private Text textName;
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
    public void SetCarPart(CarPart dataCarPart)
    {
        carPart = dataCarPart;
        imageCarPart.sprite = carPart.hudSprite;
        textName.text = dataCarPart.partName;
    }
}
