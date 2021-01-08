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
        if (other.GetComponent<AirDropFlightIndicator>() == utility.targetPosIndicator)
        {
            //utility.targetPosIndicator.transform.position = utility.targetPosIndicator.utility.ReturnRandPosInAirdropBounds(GameManager.Instance.AirDropManager.backdropZone, 0.75f, (int)Random.Range(15, 40));
            utility.targetPosIndicator.utility.UpdateSpawnOnTrigger(utility.targetPosIndicator.transform);
        }
    }
}