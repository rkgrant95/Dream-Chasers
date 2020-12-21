using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AirDropManagerUtility
{
    [Tooltip("The type of air drop that will be received")]
    public AirDropType nextAirDropTypeToSpawn;

    public List<AirDropHolderUtility> airDropHolders = new List<AirDropHolderUtility>(2);

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
    private int maxActiveAirDrops = 5;
    public int currentAirDrops = 0;


    [Header("Debugging Properties")]
    [SerializeField] private bool canSpawn = true;      //Can this spawner spawn enemies? This is useful for testing when you want to turn a spawner off
    [SerializeField] private bool showDebugs = true;

    public IEnumerator SpawnAirDrop()
    {
        while (canSpawn)                                                                                                                // While we can spawn airDrops             
        {
            yield return new WaitForSeconds(1);                                                                                         // Loop every 1 second

            if (currentAirDrops < maxActiveAirDrops)                                                                                    // If we have not exceeded maximum concurrent air drops
            {
                yield return new WaitForSeconds(airDropFrequency - 1);                                                                  // Wait for delay to end

                // RandomizeAirDropFrequency(5, 60);
                RandomizeAirDropType(0, 1);                                                                                             // Determine air drop type

                switch (nextAirDropTypeToSpawn)                                                                                         // Switch statement depending on air drop type
                {
                    case AirDropType.Weapon:
                        SpawnWeaponDrop();                                                                                              // Spawn a new weapon drop
                        break;
                    case AirDropType.Equipment:
                        SpawnEquipmentDrop();                                                                                           // Spawn a new equipment drop
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void SpawnWeaponDrop()
    {
        int weaponID = Random.Range(0, weaponDrops.Count);


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

    private void SpawnEquipmentDrop()
    {

    }


    /// <summary>
    /// Runs the initialization functions for Air Drop Manager, Weapon Drops & Equipment Drops
    /// </summary>
    /// <param name="_thisTransform"></param>
    public void Initialize(Transform _thisTransform)
    {
        InitializeAirDropManager(_thisTransform);
        InitializeWeaponDrops();
        InitializeEquipmentDrops();
    }

    /// <summary>
    /// Initialize the Air Drop Manager 
    /// </summary>
    /// <param name="_thisTransform"></param>
    private void InitializeAirDropManager(Transform _thisTransform)
    {
        airDropHolders.Add(new AirDropHolderUtility("Weapon Air Drop Holder", _thisTransform));
        airDropHolders.Add(new AirDropHolderUtility("Equipment Air Drop Holder", _thisTransform));

        
        for (int i = 0; i < airDropHolders.Count; i++)                                                                                      // Loop through available air drop holders
            airDropHolders[i].gameObject.GetComponent<AirDropHolder>().airDropHolder = new AirDropHolderUtility(airDropHolders[i]);         // Assign utility values to the monobehaviour 
    }

    /// <summary>
    /// Initialize the Weapon Drops for the Air Drop Manager
    /// </summary>
    private void InitializeWeaponDrops()
    {
        if (weaponDrops.Count > 0)                                                                                                          // If we have weapon weapon drop classes in the air drop manager    
        {
            for (int i = 0; i < weaponDrops.Count; i++)                                                                                     // Loop through all weapon drops in the air drop manager
            {
                weaponDrops[i].airDropType = AirDropType.Weapon;                                                                            // Classify all weapon drops as weapon drops
                weaponDrops[i].dropId = i;                                                                                                  // Set all weapon drop ID's to their index

                if (overrideAirDropActiveTime)                                                                                              // If we want to override the prefab active time to the air drop manager preset
                {
                    if (useRandomActiveTime)                                                                                                // If we want to use a random value
                        weaponDrops[i].activeTime = Random.Range(5, 30);                                                                    // Set the weapon drop active time to a random value
                    else
                        weaponDrops[i].activeTime = airDropActiveTime;                                                                      // Else use the preset active time value
                }

                SetupAirDropList(airDropHolders[0].airDropLists, i);
            }
        }
        else
        {
            Debug.LogWarning("Warning!!! Weapon air drop list is empty. Populate the list in the Air Drop Manager");
        }

    }

    /// <summary>
    /// Initialize the Equipment Drops for the Air Drop Manager
    /// </summary>
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

    /// <summary>
    /// Sets up the air drop lists to hold the air drop game objects at run time
    /// </summary>
    /// <param name="_airDropList"></param>
    /// <param name="_index"></param>
    private void SetupAirDropList(List<AirDropListUtility> _airDropList, int _index)
    {
         _airDropList.Add(new AirDropListUtility("Weapon Drop Holder" + _index, airDropHolders[0].gameObject.transform));                           // Add a new list to the air drop list

        if (_airDropList.Count > 0)
        {
            //Debug.Log(_airDropList[_index].gameObject.GetComponent<AirDropList>().airDropList.name);
            _airDropList[_index].gameObject.GetComponent<AirDropList>().airDropList = new AirDropListUtility(_airDropList[_index]);                 // Assign utility values to the monobehaviour 

        }


        for (int j = 0; j < 3; j++)                                                                                                     // Loop 3 times
            _airDropList[_index].airDrops.Add(GenereateNewAirDrop(weaponDrops[_index], _airDropList[_index].gameObject.transform));         // Add the new air drop to the air drop list
    }

    public AirDrop GenereateNewAirDrop(AirDropUtility _refAirDrop, Transform _parentTransform)
    {
        GameObject tempGO = GameObject.Instantiate(_refAirDrop.dropGO);
        AirDrop retVal = tempGO.GetComponent<AirDrop>();
        retVal.airDrop = new AirDropUtility(_refAirDrop);

        retVal.airDrop.dropGO = retVal.gameObject;
        retVal.transform.parent = _parentTransform;
        retVal.gameObject.SetActive(false);

        return retVal;
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

