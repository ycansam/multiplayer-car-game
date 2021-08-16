using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public CarPartsType CarParts; // lista de partes de coche
    private static Inventory instance;
    public static Inventory Instance { get => instance; }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    private void Start() {
        Debug.Log(CarParts.CarPartsTypesList.Count);
    }
}
