using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Requires Rigidbody to move around

public class CarePackageCarrier : MonoBehaviour
{
    public CarePackageCarrierUtility utility;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CarePackageFlightIndicator>() == utility.targetPosIndicator)
        {
            utility.targetPosIndicator.utility.UpdateSpawnOnTrigger(utility.targetPosIndicator.transform);
        }
    }
}