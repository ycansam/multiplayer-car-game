using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PartType{Chasis, Motor, Brakes}

[System.Serializable]
public class CarPartType{
    public List<GameObject> carPartList = new List<GameObject>();
    public PartType partType;
    public Sprite hudSprite;
}
[System.Serializable]
public class CarPartsType 
{
    public List<CarPartType> CarPartsTypesList;
}
