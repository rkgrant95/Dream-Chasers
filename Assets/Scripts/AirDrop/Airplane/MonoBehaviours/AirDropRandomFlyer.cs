using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))] // Requires animator with parameter "flySpeed" catering for 0, 1 (idle, flap)
[RequireComponent(typeof(Rigidbody))] // Requires Rigidbody to move around

public class AirDropRandomFlyer : MonoBehaviour
{
    public AirDropRandomFlyerUtility utility;

    void Start()
    {
        utility = new AirDropRandomFlyerUtility("I am a new plane");
        utility.Initialize(this.gameObject);
    }

    void FixedUpdate()
    {
        utility.FixedTick(this.gameObject);
    }
}