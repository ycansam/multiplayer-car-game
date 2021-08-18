using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PartType{Chasis, Motor, Brakes, Wheels, Lights, Aleron}

[System.Serializable]
public class CarPartType{ // contiene la lista de cada una de las partes lista[i]lista[i]
    public List<CarPart> carPartList = new List<CarPart>();
    public PartType partType;
    public Sprite hudSprite;
}
[System.Serializable]
public class CarPartsType  // referencia  a la lista que contiene las listas lista[i]
{
    public List<CarPartType> CarPartsTypesList;
}

