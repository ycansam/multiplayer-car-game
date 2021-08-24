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
            c.a = selected ? 1f : 0f;
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
    public CarPart GetPartType()
    {
        return carPart;
    }

    /// <summary>
    /// Selecciona el bot�n
    /// </summary>
    public void Select()
    {
        ToolControllerCarPart.SetCarPart(carPart);
    }

    private void OnChangeCarPart()
    {
        if (carPart is Chasis)
            Selected = ToolControllerCarPart.chasis == carPart;
        else if (carPart is Motor)
            Selected = ToolControllerCarPart.motor == carPart;
        else if (carPart is Chasis)
            Selected = ToolControllerCarPart.motor == carPart;
        else if (carPart is Motor)
            Selected = ToolControllerCarPart.motor == carPart;
        else if (carPart is Brakes)
            Selected = ToolControllerCarPart.motor == carPart;
        else if (carPart is Wheels)
            Selected = ToolControllerCarPart.motor == carPart;
        else if (carPart is Lights)
            Selected = ToolControllerCarPart.motor == carPart;
        else if (carPart is Aleron)
            Selected = ToolControllerCarPart.motor == carPart;
    }

    private void OnEnable()
    {
        ToolControllerCarPart.OnSelectCarPart += OnChangeCarPart;
    }

    private void OnDisable()
    {
        ToolControllerCarPart.OnSelectCarPart -= OnChangeCarPart;
    }
}
