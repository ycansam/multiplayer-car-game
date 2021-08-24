using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolControllerCarPart : MonoBehaviour
{
    public static Chasis chasis;
    public static int chasisPos = 0;

    public static Motor motor;
    public static int motorPos = 0;

    public static Brakes brakes;
    public static int brakesPos = 0;

    public static Wheels wheels;
    public static int wheelsPos = 0;

    public static Lights lights;
    public static int lightsPos = 0;

    public static Aleron aleron;
    public static int aleronPos = 0;


    public static event Action OnSelectCarPart;
    public static void SetCarPart(CarPart dataCarPart)
    {
        if (dataCarPart is Chasis)
            chasis = dataCarPart as Chasis;
        else if (dataCarPart is Motor)
            motor = dataCarPart as Motor;
        else if (dataCarPart is Brakes)
            brakes = dataCarPart as Brakes;
        else if (dataCarPart is Wheels)
            wheels = dataCarPart as Wheels;
        else if (dataCarPart is Lights)
            lights = dataCarPart as Lights;
        else if (dataCarPart is Aleron)
            aleron = dataCarPart as Aleron;

        OnSelectCarPart?.Invoke();
    }
}
