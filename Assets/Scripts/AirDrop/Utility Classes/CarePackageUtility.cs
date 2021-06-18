using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarePackageType { Weapon, Equipment, Tactical }

[System.Serializable]
public class CarePackageUtility : MonoBehaviour
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

    public CarePackageUtility()
    {

    }

    public CarePackageUtility(CarePackageUtility _airDropUtility)
    {
        dropId = _airDropUtility.dropId;
        activeTime = _airDropUtility.activeTime;
        airDropType = _airDropUtility.airDropType;
        useRandomActiveTime = _airDropUtility.useRandomActiveTime;
    }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }

    public virtual int AirDropCollected()
    {
        StartCoroutine(DespawnAirDrop(0));
        return dropId;
    }

    protected virtual IEnumerator DespawnAirDrop(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        if (gameObject.activeSelf)
        {
            GameManager.Instance.AirDropManager.utility.currentAirDrops--;
            gameObject.SetActive(false);
        }
    }

    public virtual void TriggerEnter()
    {
        switch (airDropType)
        {
            case CarePackageType.Weapon:
                GameManager.Instance.Player.GetComponentInChildren<WeaponSystem>().SetActiveWeapon(AirDropCollected());
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





