using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class ToolControllerCarParts
{
    public static int positionSelected = 2;
    public static CarPartType CarPartType;

    public static event Action OnSelectCarPartType;
    
    public static void SetCarPart(CarPartType dataCarPartType)
    {
        CarPartType = dataCarPartType;
        OnSelectCarPartType?.Invoke();
    }
}
