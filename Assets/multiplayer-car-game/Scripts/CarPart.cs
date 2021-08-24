using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarModel { BMW, TOYOTA, VOLKSWAGEN}
public class CarPart : ScriptableObject{
    public PartType partType;
    public CarModel carModelPart;
    public string partName;
    public Sprite hudSprite;
    public GameObject prefab;
}