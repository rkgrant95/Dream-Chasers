using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))] // Requires animator with parameter "flySpeed" catering for 0, 1 (idle, flap)
[RequireComponent(typeof(Rigidbody))] // Requires Rigidbody to move around

public class AirDropRandomFlyer : MonoBehaviour
{
    public AirDropRandomFlyerUtility utility;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == utility.targetPosIndicator)
        {
            GameManager.Instance.AirDropManager.adfUtility.TestTargetPositionIndicator(utility.targetPosIndicator.transform);
           // GameManager.Instance.AirDropManager.adfUtility.TestIndicatorSpawnAirDrop();
        }
    }
}