using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class AirDropManagerUtility
{
    [Tooltip("The type of air drop that will be received")]
    public AirDropType nextAirDropTypeToSpawn;

    public List<AirDropHolder> airDropHolders = new List<AirDropHolder>(3);

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

    public AirDropHolder airDropHolderPrefab;
    public AirDropList airDropListPrefab;

    public List<GameObject> weaponDropPrefabs;
    public List<GameObject> equipmentDropPrefabs;
    public List<GameObject> tacticalDropPrefabs;

    [SerializeField]
    [Tooltip("The maximum number of active airdrops at any one time")]
    private int maxActiveAirDrops = 5;
    public int currentAirDrops = 0;

    [Tooltip("The maximum number of airdrops of the same type that can be active at the same time")]
    public int maxAirDropClones = 3;


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
        int weaponID = Random.Range(0, weaponDropPrefabs.Count);

        for (int j = 0; j < maxAirDropClones; j++)
        {
            if (!airDropHolders[0].utility.airDropLists[weaponID].utility.airDrops[j].gameObject.activeSelf)
            {
                airDropHolders[0].utility.airDropLists[weaponID].utility.airDrops[j].transform.position = GetAirDropSpawnPosition();
                airDropHolders[0].utility.airDropLists[weaponID].utility.airDrops[j].gameObject.SetActive(true);
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
        for (int i = 0; i < 3; i++)
            PopulateAirDropList((AirDropType)i);

        InitializeAirDropManager(_thisTransform);
        InitializeWeaponDrops();
        InitializeEquipmentDrops();
    }

    /// <summary>
    /// Initialize the Air Drop Manager 
    /// </summary>
    /// <param name="_thisTransform"></param>
    private void InitializeAirDropManager(Transform _parentTransform)
    {
        airDropHolders.Add(GenereateNewAirDropholder(airDropHolderPrefab, "Weapon Drop Holder", _parentTransform));
        airDropHolders.Add(GenereateNewAirDropholder(airDropHolderPrefab, "Equipment Drop Holder", _parentTransform));
        airDropHolders.Add(GenereateNewAirDropholder(airDropHolderPrefab, "Tactical Drop Holder", _parentTransform));

    }

    /// <summary>
    /// Initialize the Weapon Drops for the Air Drop Manager
    /// </summary>
    private void InitializeWeaponDrops()
    {
        if (weaponDropPrefabs.Count > 0)                                                                                                          // If we have weapon weapon drop classes in the air drop manager    
        {
            for (int i = 0; i < weaponDropPrefabs.Count; i++)                                                                                     // Loop through all weapon drops in the air drop manager
            {
                weaponDropPrefabs[i].GetComponent<AirDrop>().utility.airDropType = AirDropType.Weapon;                                                                            // Classify all weapon drops as weapon drops
                weaponDropPrefabs[i].GetComponent<AirDrop>().utility.dropId = i;                                                                                                  // Set all weapon drop ID's to their index

                InitializeDropList(airDropHolders[0], i);
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
        if (equipmentDropPrefabs.Count > 0)
        {
            for (int i = 0; i < equipmentDropPrefabs.Count; i++)
            {
                equipmentDropPrefabs[i].GetComponent<AirDrop>().utility.airDropType = AirDropType.Equipment;
                equipmentDropPrefabs[i].GetComponent<AirDrop>().utility.dropId = i;

               // equipmentDropPrefabs[i].utility.dropGO = GameObject.Instantiate(equipmentDropPrefabs[i].utility.dropGO);

                if (overrideAirDropActiveTime)
                {
                    if (useRandomActiveTime)
                        equipmentDropPrefabs[i].GetComponent<AirDrop>().utility.activeTime = Random.Range(5, 30);
                    else
                        equipmentDropPrefabs[i].GetComponent<AirDrop>().utility.activeTime = airDropActiveTime;
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
    private void InitializeDropList(AirDropHolder _airDropHolder, int _index)
    {
        _airDropHolder.utility.airDropLists.Add(GenerateNewAirDropList(airDropListPrefab, "Weapon Drop Sub-Holder " + _index, _airDropHolder.transform));

        
        int rand = Random.Range(5, 30);

        for (int j = 0; j < maxAirDropClones; j++)
        {
            _airDropHolder.utility.airDropLists[_index].utility.airDrops.Add(GenereateNewAirDrop(weaponDropPrefabs[_index].GetComponent<AirDrop>(), _airDropHolder.utility.airDropLists[_index].transform));         // Add the new air drop to the air drop list

            if (overrideAirDropActiveTime)                                                                                              // If we want to override the prefab active time to the air drop manager preset
            {
                if (useRandomActiveTime)                                                                                                // If we want to use a random value
                    _airDropHolder.utility.airDropLists[_index].utility.airDrops[j].utility.activeTime = rand;                                                                  // Set the weapon drop active time to a random value
                else
                    _airDropHolder.utility.airDropLists[_index].utility.airDrops[j].utility.activeTime = airDropActiveTime;                                                                      // Else use the preset active time value
            }

        }
    }

    /// <summary>
    /// Populates the air drop prefab lists from the asset directory
    /// </summary>
    /// <param name="_airDropType"></param>
    private void PopulateAirDropList(AirDropType _airDropType)
    {
        System.IO.DirectoryInfo dir;                                                                                                                            // Get the file directory for the air drop folder
        int prefabCount = 0;                                                                                                                                    // How many prefabs are present in the directory                               
        GameObject adPrefab;                                                                                                                                    // Temporary game object to store the prefab in 
        string prefabPath = Statics.EmptyString;                                                                                                                // Path to locate each prefab 

        switch (_airDropType)                                                                                                                                   // Initialise variables based on air drop type
        {
            case AirDropType.Weapon:
                weaponDropPrefabs.Clear();
                dir = new System.IO.DirectoryInfo(Statics.WeaponDropFolderPath);                                                                                    
                prefabCount = dir.GetFiles().Length / 2;                                                                                                            
                prefabPath = Statics.WeaponDropPath;
                break;
            case AirDropType.Equipment:
                equipmentDropPrefabs.Clear();
                dir = new System.IO.DirectoryInfo(Statics.EquipmentDropFolderPath);
                prefabCount = dir.GetFiles().Length / 2;
                prefabPath = Statics.EquipmentDropPath;
                break;
            case AirDropType.Tactical:
                tacticalDropPrefabs.Clear();
                dir = new System.IO.DirectoryInfo(Statics.TacticalDropFolderPath);
                prefabCount = dir.GetFiles().Length / 2;
                prefabPath = Statics.TacticalDropPath;
                break;
            default:
                break;
        }

        for (int i = 0; i < prefabCount; i++)                                                                                                                    // Add air drop prefabs to the list 
        {
            adPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath + i + Statics.PrefabExtension, typeof(GameObject));

            switch (_airDropType)
            {
                case AirDropType.Weapon:
                    weaponDropPrefabs.Add(adPrefab);
                    break;
                case AirDropType.Equipment:
                    equipmentDropPrefabs.Add(adPrefab);
                    break;
                case AirDropType.Tactical:
                    tacticalDropPrefabs.Add(adPrefab);
                    break;
                default:
                    break;
            }
        }
    }


    public AirDropList GenerateNewAirDropList(AirDropList _refAirDropList, string _airDropListName, Transform _parentTransform)
    {
        GameObject tempGO = GameObject.Instantiate(_refAirDropList.gameObject);
        AirDropList retVal = tempGO.GetComponent<AirDropList>();

        retVal.utility = new AirDropListUtility(_airDropListName);
        retVal.gameObject.name = retVal.utility.name;
        retVal.transform.parent = _parentTransform;

        return retVal;
    }

    public AirDropHolder GenereateNewAirDropholder(AirDropHolder _refAirDropHolder, string _airDropHolderName, Transform _parentTransform)
    {
        GameObject tempGO = GameObject.Instantiate(_refAirDropHolder.gameObject);
        AirDropHolder retVal = tempGO.GetComponent<AirDropHolder>();

        retVal.utility = new AirDropHolderUtility(_airDropHolderName);
        retVal.gameObject.name = retVal.utility.name;
        retVal.transform.parent = _parentTransform;

        return retVal;
    }

    public AirDrop GenereateNewAirDrop(AirDrop _refAirDrop, Transform _parentTransform)
    {
        GameObject tempGO = GameObject.Instantiate(_refAirDrop.gameObject);
        AirDrop retVal = tempGO.GetComponent<AirDrop>();
        retVal.utility = new AirDropUtility(_refAirDrop.utility);

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

