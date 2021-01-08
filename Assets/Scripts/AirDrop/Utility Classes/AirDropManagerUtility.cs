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

    #region Air Drop Fields & Modifiers

    public Collider airDropExtentsCollider;

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


    #endregion

    #region Flight Manager Variables
    private GameObject flightHolder;
    private GameObject indicatorHolder;

    public GameObject airDropFlyerPrefab;
    public GameObject indicatorPrefab;

    public List<AirDropRandomFlyer> airDropFlyers;
    public List<AirDropFlightIndicator> flightIndicators;

    public int airDropFlyerCount = 6;
    public int airDropSpawnBacklog = 0;

    public bool debugMode;
    #endregion

    #region Functions

    #region Air Drop spawner functions

    public IEnumerator RequestAirDrop(int _iterationFrequency = 1)
    {
        while (canSpawn)                                                                                                                // While we can spawn airDrops             
        {
            yield return new WaitForSeconds(_iterationFrequency);                                                                                         // Loop every 1 second

            if (currentAirDrops < maxActiveAirDrops)                                                                                    // If we have not exceeded maximum concurrent air drops
            {
                yield return new WaitForSeconds(airDropFrequency - 1);                                                                  // Wait for delay to end

                airDropSpawnBacklog++;

                switch (nextAirDropTypeToSpawn)
                {
                    case AirDropType.Weapon:
                        RequestAirDrop();
                        break;
                    case AirDropType.Equipment:
                        RequestAirDrop();
                        break;
                    case AirDropType.Tactical:
                        RequestAirDrop();
                        break;
                    default:
                        break;
                }
            }
        }
    }


    private void RequestAirDrop()
    {
        List<AirDropFlightIndicator> availableIndicators = new List<AirDropFlightIndicator>();

        for (int i = 0; i < flightIndicators.Count; i++)
        {
            if (flightIndicators[i].utility.indicatorState == AirDropFlightIndicatorUtility.IndicatorState.Backdrop)
                availableIndicators.Add(flightIndicators[i]);
        }


        if (availableIndicators.Count > 0)
        {
            int indicatorID = Random.Range(0, availableIndicators.Count);
            availableIndicators[indicatorID].utility.indicatorState = AirDropFlightIndicatorUtility.IndicatorState.AirDrop;
            availableIndicators[indicatorID].utility.UpdateSpawnDefault(availableIndicators[indicatorID].transform);
        }
        else
            return;
    }

    public void SpawnAirDrop(Transform _spawnPos)
    {
        int dropID = Random.Range(0, weaponDropPrefabs.Count);                                                              // Get a random dropID to spawn a specific type of weapon drop
        RandomizeAirDropType(0, 3);                                                                                             // Determine air drop type
        RandomizeAirDropFrequency(2, 10);

        int holderIndex = (int)nextAirDropTypeToSpawn;

        for (int i = 0; i < maxAirDropClones; i++)
        {
            if (!airDropHolders[holderIndex].utility.airDropSubHolder[dropID].utility.airDrops[i].gameObject.activeSelf)
            {
                airDropHolders[holderIndex].utility.airDropSubHolder[dropID].utility.airDrops[i].transform.position = _spawnPos.position + new Vector3(0, -1f, 0);
                airDropHolders[holderIndex].utility.airDropSubHolder[dropID].utility.airDrops[i].gameObject.SetActive(true);
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


        GenerateHolders();
        InitializeHolders(_thisTransform);

        InitializeRandIndicatorList();
        InitializeFlyerList();

        InitializeFlyers();
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

    #region Flight Manager Functions

    public void FixedTick()
    {
        UpdateFlyers();
    }

    private void UpdateFlyers()
    {
        for (int i = 0; i < airDropFlyers.Count; i++)
            airDropFlyers[i].utility.FixedTick(airDropFlyers[i].gameObject);
    }

    public IEnumerator ShowDebugs(GameObject _thisGameObject)
    {
        while (_thisGameObject.activeSelf == true)
        {
            yield return new WaitForSeconds(1);

            if (debugMode)
            {
                for (int i = 0; i < flightIndicators.Count; i++)
                {
                    flightIndicators[i].GetComponent<Renderer>().enabled = true;
                }
            }
            else
            {
                for (int i = 0; i < flightIndicators.Count; i++)
                {
                    flightIndicators[i].GetComponent<Renderer>().enabled = false;
                }
            }

        }
    }

    private void InitializeFlyers()
    {
        for (int i = 0; i < airDropFlyers.Count; i++)
            airDropFlyers[i].utility.Initialize(airDropFlyers[i].gameObject, flightIndicators[i]);
    }

    private void InitializeFlyerList()
    {
        for (int i = 0; i < airDropFlyerCount; i++)
        {
            GameObject tempGO = GameObject.Instantiate(airDropFlyerPrefab.gameObject);
            AirDropRandomFlyer retVal = tempGO.GetComponent<AirDropRandomFlyer>();

            retVal.utility = new AirDropRandomFlyerUtility(Statics.AirDropFlyerName + " " + i);
            retVal.transform.parent = flightHolder.transform;

            retVal.transform.position = ReturnRandPosInAirdropBounds(GameManager.Instance.AirDropManager.backdropZone, 0.75f, (int)Random.Range(15, 40));

            airDropFlyers.Add(retVal);
        }
    }

    private void InitializeRandIndicatorList()
    {
        for (int i = 0; i < airDropFlyerCount; i++)
        {
            GameObject tempGO = GameObject.Instantiate(indicatorPrefab.gameObject);
            AirDropFlightIndicator retVal = tempGO.GetComponent<AirDropFlightIndicator>();

            retVal.utility = new AirDropFlightIndicatorUtility(Statics.IndicatorName + " " + i);
            retVal.gameObject.name = retVal.utility.name;
            retVal.transform.parent = indicatorHolder.transform;

            retVal.transform.position = ReturnRandPosInAirdropBounds(GameManager.Instance.AirDropManager.backdropZone, 0.75f, (int)Random.Range(15, 40));

            flightIndicators.Add(retVal);
        }
    }

    private void InitializeHolders(Transform _managerTransform)
    {
        flightHolder.transform.parent = _managerTransform;
        indicatorHolder.transform.parent = _managerTransform;
    }

    private void GenerateHolders()
    {
        flightHolder = new GameObject("Flight Holder");
        indicatorHolder = new GameObject("Indicator Holder");
    }

    /// <summary>
    /// Returns a random position inside a defined bounds, allows you to define the spawn height manually 
    /// </summary>
    /// <param name="_boundsCollider"></param>
    /// <param name="_boundsBuffer"></param>
    /// <param name="_ySpawnHeight"></param>
    /// <returns></returns>
    public Vector3 ReturnRandPosInBackdropBounds(Collider _boundsCollider, float _boundsBuffer, int _ySpawnHeight)
    {
        return new Vector3(Random.Range(_boundsCollider.bounds.min.x, _boundsCollider.bounds.max.x) * _boundsBuffer, _ySpawnHeight,
                           Random.Range(_boundsCollider.bounds.min.z, _boundsCollider.bounds.max.z) * _boundsBuffer);
    }

    /// <summary>
    /// Returns a random position inside a defined bounds
    /// </summary>
    /// <param name="_boundsCollider"></param>
    /// <param name="_boundsBuffer"></param>
    /// <param name="_ySpawnHeight"></param>
    /// <returns></returns>
    public Vector3 ReturnRandPosInBackdropBounds(Collider _boundsCollider, float _boundsBuffer)
    {
        return new Vector3(Random.Range(_boundsCollider.bounds.min.x, _boundsCollider.bounds.max.x),
        Random.Range(_boundsCollider.bounds.min.y, _boundsCollider.bounds.max.y),
        Random.Range(_boundsCollider.bounds.min.z, _boundsCollider.bounds.max.z));
    }

    /// <summary>
    /// Returns a random position inside a defined bounds, allows you to define spawn height manually, defaults the z spawn location to the centre
    /// </summary>
    /// <param name="_boundsCollider"></param>
    /// <param name="_boundsBuffer"></param>
    /// <param name="_ySpawnHeight"></param>
    /// <returns></returns>
    public Vector3 ReturnRandPosInAirdropBounds(Collider _boundsCollider, float _boundsBuffer, int _ySpawnHeight)
    {
        return new Vector3(Random.Range(_boundsCollider.bounds.min.x, _boundsCollider.bounds.max.x) * _boundsBuffer, _ySpawnHeight,
                           _boundsCollider.bounds.center.z);
    }
    #endregion
}

