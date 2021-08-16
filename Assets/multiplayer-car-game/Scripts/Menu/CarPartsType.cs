using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType{Chasis, Motor, Brakes}

[System.Serializable]
public class CarPartType{
    public List<GameObject> carPartList = new List<GameObject>();
    public PartType partType;
}
[System.Serializable]
public class CarPartsType 
{
    public List<CarPartType> CarPartsTypesList;
}
