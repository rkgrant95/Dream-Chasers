using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AirDropManagerUtility
{
    #region Variables

    #region Air Drop Prefabs
    [Header("Air Drop Prefabs")]
    [Tooltip("Air drop holder prefab")]
    private GameObject airDropHolderPrefab;

    [Tooltip("Air drop list prefab")]
    private GameObject airDropSubHolderPrefab;

    [Tooltip("List of weapon air drop prefabs")]
    [SerializeField]
    private List<GameObject> weaponDropPrefabs;

    [Tooltip("List of equipment air drop prefabs")]
    [SerializeField]
    private List<GameObject> equipmentDropPrefabs;

    [Tooltip("List of tactical air drop prefabs")]
    [SerializeField]
    private List<GameObject> tacticalDropPrefabs;

    #endregion

    #region Child References
    public List<AirDropHolder> airDropHolders = new List<AirDropHolder>();
    #endregion

    #endregion

    #region Functions

    #region Air Drop Fields & Modifiers

    [SerializeField] private Collider airDropExtentsCollider;

    [Tooltip("The type of air drop that will be received")]
    public AirDropType nextAirDropTypeToSpawn;

    [Header("Air drop modifiers & fields")]
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

    [Tooltip("The maximum number of active airdrops at any one time")]
    [SerializeField]
    private int maxActiveAirDrops = 5;
    [Tooltip("The current number of active airdrops")]
    public int currentAirDrops = 0;

    [Tooltip("The maximum number of airdrops of the same type that can be active at the same time")]
    [SerializeField]
    private int maxAirDropClones = 3;


    [Header("Debugging Properties")]
    [SerializeField] private bool canSpawn = true;      //Can this spawner spawn enemies? This is useful for testing when you want to turn a spawner off
    [SerializeField] private bool showDebugs = true;

    #endregion

    #region Air Drop spawner functions

    public IEnumerator SpawnAirDrop()
    {
        while (canSpawn)                                                                                                                // While we can spawn airDrops             
        {
            yield return new WaitForSeconds(1);                                                                                         // Loop every 1 second

            if (currentAirDrops < maxActiveAirDrops)                                                                                    // If we have not exceeded maximum concurrent air drops
            {
                yield return new WaitForSeconds(airDropFrequency - 1);                                                                  // Wait for delay to end

                // RandomizeAirDropFrequency(5, 60);
                RandomizeAirDropType(0, 3);                                                                                             // Determine air drop type

                int dropID = -1;

                switch (nextAirDropTypeToSpawn)
                {
                    case AirDropType.Weapon:
                        dropID = Random.Range(0, weaponDropPrefabs.Count);                                                              // Get a random dropID to spawn a specific type of weapon drop
                        SpawnAirDrop((int)nextAirDropTypeToSpawn, dropID);                                                              // Spawn the air drop
                        break;
                    case AirDropType.Equipment:
                        dropID = Random.Range(0, equipmentDropPrefabs.Count);
                        SpawnAirDrop((int)nextAirDropTypeToSpawn, dropID);
                        break;
                    case AirDropType.Tactical:
                        dropID = Random.Range(0, tacticalDropPrefabs.Count);
                        SpawnAirDrop((int)nextAirDropTypeToSpawn, dropID);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void SpawnAirDrop(int _index, int _dropID)
    {
        for (int i = 0; i < maxAirDropClones; i++)
        {
            if (!airDropHolders[_index].utility.airDropSubHolder[_dropID].utility.airDrops[i].gameObject.activeSelf)
            {
                airDropHolders[_index].utility.airDropSubHolder[_dropID].utility.airDrops[i].transform.position = GetAirDropSpawnPosition();
                airDropHolders[_index].utility.airDropSubHolder[_dropID].utility.airDrops[i].gameObject.SetActive(true);
                currentAirDrops++;
                break;
            }
        }
    }

    #endregion

    #region Initializers, initializes important variables, classes and lists

    /// <summary>
    /// Runs the initialization functions for Air Drop Manager, Weapon Drops & Equipment Drops
    /// </summary>
    /// <param name="_thisTransform"></param>
    public void Initialize(Transform _thisTransform)
    {
        for (int i = -1; i < 3; i++)
            PopulateAirDropPrefabsFromAssetDatabase((AirDropType)i);

        InitializeAirDropManager(_thisTransform);

        for (int i = 0; i < 3; i++)
            InitializeAirDropSubHolders(airDropHolders[i], i);
    }

    /// <summary>
    /// Initialize the Air Drop Manager 
    /// </summary>
    /// <param name="_thisTransform"></param>
    private void InitializeAirDropManager(Transform _parentTransform)
    {
        AirDropHolder adHolder = airDropHolderPrefab.GetComponent<AirDropHolder>();
        airDropHolders.Add(GenereateNewAirDropholder(adHolder, Statics.WeaponDropHolderName, _parentTransform));
        airDropHolders.Add(GenereateNewAirDropholder(adHolder, Statics.EquipmentDropHolderName, _parentTransform));
        airDropHolders.Add(GenereateNewAirDropholder(adHolder, Statics.TacticalDropHolderName, _parentTransform));

    }


    /// <summary>
    /// Sets up the air drop lists to hold the air drop game objects at run time
    /// </summary>
    /// <param name="_airDropList"></param>
    /// <param name="_enumIndex"></param>
    private void InitializeAirDropSubHolders(AirDropHolder _airDropHolder, int _enumIndex)
    {
        switch ((AirDropType)_enumIndex)
        {
            case AirDropType.Weapon:
                for (int i = 0; i < weaponDropPrefabs.Count; i++)
                {
                    _airDropHolder.utility.airDropSubHolder.Add(GenerateNewAirDropSubHolder(airDropSubHolderPrefab.GetComponent<AirDropSubHolder>(), Statics.WeaponDropSubHolderName + i, _airDropHolder.transform));
                    InitializeAirDropSubHolder(_airDropHolder, i, weaponDropPrefabs);
                }
                break;
            case AirDropType.Equipment:
                for (int i = 0; i < equipmentDropPrefabs.Count; i++)
                {
                    _airDropHolder.utility.airDropSubHolder.Add(GenerateNewAirDropSubHolder(airDropSubHolderPrefab.GetComponent<AirDropSubHolder>(), Statics.EquipmentDropSubHolderName + i, _airDropHolder.transform));
                    InitializeAirDropSubHolder(_airDropHolder, i, equipmentDropPrefabs);
                }
                break;
            case AirDropType.Tactical:
                for (int i = 0; i < tacticalDropPrefabs.Count; i++)
                {
                    _airDropHolder.utility.airDropSubHolder.Add(GenerateNewAirDropSubHolder(airDropSubHolderPrefab.GetComponent<AirDropSubHolder>(), Statics.TacticalDropSubHolderName + i, _airDropHolder.transform));
                    InitializeAirDropSubHolder(_airDropHolder, i, tacticalDropPrefabs);
                }
                break;
            default:
                break;
        }
    }

    private void InitializeAirDropSubHolder(AirDropHolder _airDropHolder, int _index, List<GameObject> _airDropPrefabs)
    {
        int rand = 0;

        if (useRandomActiveTime)                                                                                                // If we want to use a random value
            rand = Random.Range(5, 30);

        for (int j = 0; j < maxAirDropClones; j++)
        {
            _airDropHolder.utility.airDropSubHolder[_index].utility.airDrops.Add(GenereateNewAirDrop(_airDropPrefabs[_index].GetComponent<AirDrop>(), _airDropHolder.utility.airDropSubHolder[_index].transform));         // Add the new air drop to the air drop list
            _airDropHolder.utility.airDropSubHolder[_index].utility.airDrops[j].utility.dropId = _index;                                                               // Set the weapon drop active time to a random value

            if (overrideAirDropActiveTime)                                                                                              // If we want to override the prefab active time to the air drop manager preset
            {
                if (useRandomActiveTime)                                                                                                // If we want to use a random value
                    _airDropHolder.utility.airDropSubHolder[_index].utility.airDrops[j].utility.activeTime = rand;                                                                  // Set the weapon drop active time to a random value
                else
                    _airDropHolder.utility.airDropSubHolder[_index].utility.airDrops[j].utility.activeTime = airDropActiveTime;                                                                      // Else use the preset active time value
            }
        }
    }

    /// <summary>
    /// Populates the air drop prefab lists from the asset directory
    /// </summary>
    /// <param name="_airDropType"></param>
    private void PopulateAirDropPrefabsFromAssetDatabase(AirDropType _airDropType)
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
                airDropHolderPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(Statics.AirDropHolderPath, typeof(GameObject));
                airDropSubHolderPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(Statics.AirDropListPath, typeof(GameObject));

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

    #endregion

    #region Generate new instances of various air drops & air drop managers

    public AirDropSubHolder GenerateNewAirDropSubHolder(AirDropSubHolder _refAirDropList, string _airDropListName, Transform _parentTransform)
    {
        GameObject tempGO = GameObject.Instantiate(_refAirDropList.gameObject);
        AirDropSubHolder retVal = tempGO.GetComponent<AirDropSubHolder>();

        retVal.utility = new AirDropSubHolderUtility(_airDropListName);
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

    #endregion

    #region Utility Functions
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
        retVal = new Vector3(airDropExtentsCollider.bounds.extents.x * Random.Range(-0.95f, 0.95f), 10, airDropExtentsCollider.bounds.center.z);

        return retVal;
    }

    #endregion

    #endregion
}

