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

    [Tooltip("The gameobject that will represent the weapon drop in game")]
    public GameObject dropGO;

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
        airDropType = _airDropUtility.airDropType;
        dropId = _airDropUtility.dropId;
        dropGO = _airDropUtility.dropGO;
        activeTime = _airDropUtility.activeTime;
        useRandomActiveTime = _airDropUtility.useRandomActiveTime;
    }
}

[System.Serializable]
public class AirDropHolderUtility
{
    public string name;
    public GameObject gameObject;
    public List<AirDropList> airDropLists = new List<AirDropList>();
}

[System.Serializable]
public class AirDropList
{
    public string name;
    public GameObject gameObject;
    public List<AirDrop> airDrops = new List<AirDrop>();
}

[System.Serializable]
public class AirDropManagerUtility 
{
    [Tooltip("The type of air drop that will be received")]
    public AirDropType nextAirDropTypeToSpawn;

    public List <AirDropHolderUtility> airDropHolders = new List<AirDropHolderUtility>(2);

    [SerializeField] private Collider playableCollider;

    [Range(5, 60)]
    [Tooltip("The frequency of an air drop")]
    public int airDropFrequency;

    [Tooltip("Randomise air drop frequency?")]
    public bool useRandomAirDropFrequency;

    [Tooltip("Override individual timers for air drop active time?")]
    public bool overrideAirDropActiveTime;

    [Tooltip("Randomise the amount of time the air drop will stay active before despawning?")]
    public bool useRandomActiveTime;

    [Range(5, 30)]
    [Tooltip("The amount of time the air drop will stay active before despawning")]
    public float airDropActiveTime;

    public List<AirDropUtility> weaponDrops;
    public List<AirDropUtility> equipmentDrops;

    [SerializeField]
    [Tooltip("The maximum number of active airdrops at any one time")]
    private int maxAirDrops = 10;
    private int currentAirDrops = 0;


    [Header("Debugging Properties")]
    [SerializeField] private bool canSpawn = true;      //Can this spawner spawn enemies? This is useful for testing when you want to turn a spawner off
    [SerializeField] private bool showDebugs = true;

    public IEnumerator SpawnAirDrop()
    {
        while (canSpawn)
        {
            yield return new WaitForSeconds(airDropFrequency);

           // RandomizeAirDropFrequency(5, 60);
            RandomizeAirDropType(0, 1);

            switch (nextAirDropTypeToSpawn)
            {
                case AirDropType.Weapon:
                    SpawnWeaponDrop();
                    break;
                case AirDropType.Equipment:
                    SpawnEquipmentDrop();
                    break;
                default:
                    break;
            }
        }
    }

    private void SpawnWeaponDrop()
    {
        int weaponID = Random.Range(0, weaponDrops.Count);

        if (currentAirDrops < maxAirDrops)
        {
            //If the current enemy is available (not active)...
            for (int j = 0; j < 3; j++)
            {
                if (!airDropHolders[0].airDropLists[weaponID].airDrops[j].gameObject.activeSelf)
                {
                    airDropHolders[0].airDropLists[weaponID].airDrops[j].transform.position = GetAirDropSpawnPosition();
                    airDropHolders[0].airDropLists[weaponID].airDrops[j].airDrop.dropGO.SetActive(true);
                    currentAirDrops++;
                    break;
                }
            }
        }
    }

    private void SpawnEquipmentDrop()
    {

    }


    public void Initialize(Transform _thisTransform)
    {
        InitializeAirDropManager(_thisTransform);
        InitializeWeaponDrops();
        InitializeEquipmentDrops();
    }

    private void InitializeAirDropHolders(int _index, string _name, Transform _thisTransform)
    {
        airDropHolders[_index].gameObject = new GameObject();

        airDropHolders[_index].name = _name;
        airDropHolders[_index].gameObject.name = _name;
        airDropHolders[_index].gameObject.transform.parent = _thisTransform;

    }

    private void InitializeAirDropManager(Transform _thisTransform)
    {
        InitializeAirDropHolders(0, "Weapon Air Drop Holder", _thisTransform);
        InitializeAirDropHolders(1, "Equipment Air Drop Holder", _thisTransform);
    }

    private void InitializeWeaponDrops()
    {
        if (weaponDrops.Count > 0)
        {
            for (int i = 0; i < weaponDrops.Count; i++)
            {          
                weaponDrops[i].airDropType = AirDropType.Weapon;
                weaponDrops[i].dropId = i;

                if (overrideAirDropActiveTime)
                {
                    if (useRandomActiveTime)
                        weaponDrops[i].activeTime = Random.Range(5, 30);
                    else
                        weaponDrops[i].activeTime = airDropActiveTime;
                }

                SetupAirDropList(airDropHolders[0].airDropLists, i);
            }
        }
        else
        {
            Debug.LogWarning("Warning!!! Weapon air drop list is empty. Populate the list in the Air Drop Manager");
        }

    }


    private void InitializeEquipmentDrops()
    {
        if (equipmentDrops.Count > 0)
        {
            for (int i = 0; i < equipmentDrops.Count; i++)
            {
                equipmentDrops[i].airDropType = AirDropType.Equipment;
                equipmentDrops[i].dropId = i;

                equipmentDrops[i].dropGO = GameObject.Instantiate(equipmentDrops[i].dropGO);

                if (overrideAirDropActiveTime)
                {
                    if (useRandomActiveTime)
                        equipmentDrops[i].activeTime = Random.Range(5, 30);
                    else
                        equipmentDrops[i].activeTime = airDropActiveTime;
                }
            }
        }
        else
        {
            Debug.LogWarning("Warning!!! Equipment air drop list is empty. Populate the list in the Air Drop Manager");
        }
    }

    private void SetupAirDropList(List<AirDropList> _airDropList, int _index)
    {
        _airDropList.Add(new AirDropList());
        _airDropList[_index].gameObject = new GameObject();
        _airDropList[_index].name = "Weapon Holder " + _index;
        _airDropList[_index].gameObject.name = _airDropList[_index].name;
        _airDropList[_index].gameObject.transform.parent = airDropHolders[0].gameObject.transform;


        for (int j = 0; j < 3; j++)
        {
            GameObject newWeaponDrop = GameObject.Instantiate(weaponDrops[_index].dropGO);
            _airDropList[_index].airDrops.Add(newWeaponDrop.GetComponent<AirDrop>());

            _airDropList[_index].airDrops[j].airDrop.dropGO = newWeaponDrop;
            _airDropList[_index].airDrops[j].airDrop.dropGO.transform.parent = _airDropList[_index].gameObject.transform;

            _airDropList[_index].airDrops[j].airDrop.dropGO.SetActive(false);
        }
    }

    private void RandomizeAirDropFrequency(int _lowerLimit, int _upperLimit)
    {
        if (useRandomAirDropFrequency)
            airDropFrequency = Random.Range(_lowerLimit, _upperLimit);
    }

    private void RandomizeAirDropType(int _lowerLimit, int _upperLimit)
    {
        nextAirDropTypeToSpawn = (AirDropType)Random.Range(_lowerLimit, _upperLimit);
    }

    private Vector3 GetAirDropSpawnPosition()
    {
        Vector3 retVal = Vector3.zero;
        retVal = new Vector3(playableCollider.bounds.extents.x * Random.Range(-0.95f, 0.95f), 10, playableCollider.bounds.center.z);

        return retVal;
    }

}

