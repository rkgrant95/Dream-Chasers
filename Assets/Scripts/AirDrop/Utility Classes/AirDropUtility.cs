using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AirDropType { Weapon, Equipment, }

[System.Serializable]
public class AirDropUtility 
{
    #region Air Drop ID
    [Header("Air Drop Identification")]

    [Tooltip("The type of air drop that will be received")]
    public AirDropType airDropType;

    [Tooltip ("The Drop ID, will be used to tell the weapon system what weapon to switch to or effect to bestow")]
    public int dropId;
    #endregion

    #region Active Time
    [Header("Air Drop Spawn Properties")]

    [Range(5, 30)]
    [Tooltip("The amount of time the air drop will stay active before despawning")]
    public float activeTime;

    [Tooltip("Randomise the amount of time the air drop will stay active before despawning?")]
    public bool useRandomActiveTime;
    #endregion


    public AirDropUtility(AirDropUtility _airDropUtility)
    {
        useRandomActiveTime = _airDropUtility.useRandomActiveTime;
        airDropType = _airDropUtility.airDropType;
        dropId = _airDropUtility.dropId;
        activeTime = _airDropUtility.activeTime;
    }

    public void AirDropCollected(AirDrop _airDrop)
    {
        GameManager.Instance.AirDropManager.airDropManager.currentAirDrops--;
        _airDrop.gameObject.SetActive(false);
    }

}





