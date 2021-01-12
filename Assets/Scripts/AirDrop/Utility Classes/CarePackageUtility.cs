using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarePackageType { Weapon, Equipment, Tactical }

[System.Serializable]
public class CarePackageUtility 
{
    #region Air Drop ID
    [Header("Air Drop Identification")]

    [Tooltip("The type of air drop that will be received")]
    public CarePackageType airDropType;

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


    public CarePackageUtility(CarePackageUtility _airDropUtility)
    {
        useRandomActiveTime = _airDropUtility.useRandomActiveTime;
        airDropType = _airDropUtility.airDropType;
        dropId = _airDropUtility.dropId;
        activeTime = _airDropUtility.activeTime;
    }

    public int AirDropCollected(CarePackage _carePackage)
    {
        GameManager.Instance.AirDropManager.utility.currentAirDrops--;
        _carePackage.gameObject.SetActive(false);
        return dropId;
    }

    public void TriggerEnter(CarePackage _carePackage)
    {
        switch (_carePackage.utility.airDropType)
        {
            case CarePackageType.Weapon:
                GameManager.Instance.Player.GetComponentInChildren<WeaponSystem>().SetActiveWeapon(_carePackage.utility.AirDropCollected(_carePackage));
                break;
            case CarePackageType.Equipment:
                break;
            case CarePackageType.Tactical:
                break;
            default:
                break;
        }

    }
}





