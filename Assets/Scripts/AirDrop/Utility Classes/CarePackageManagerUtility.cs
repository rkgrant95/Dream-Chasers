using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class CarePackageManagerUtility
{
    #region Variables

    #region Air Drop Prefabs
    [Header("Air Drop Prefabs")]

    [Tooltip("Air drop holder prefab")]
    [SerializeField]    
    //[HideInInspector]
    private GameObject airDropHolderPrefab;

    [Tooltip("Air drop list prefab")]
    [SerializeField]    
    //[HideInInspector]
    private GameObject cpSubHolderPrefab;

    [Tooltip("Holder to hold child airplanes")]
    [SerializeField]    
    //[HideInInspector]
    private GameObject flightHolder;

    [Tooltip("Holder to hold airplane indicators")]
    [SerializeField]    
    //[HideInInspector]
    private GameObject indicatorHolder;

    [Tooltip("Air drop plane prefab")]
    [SerializeField]    
    //[HideInInspector]
    private GameObject airDropFlyerPrefab;

    [Tooltip("Airplane indicator indicator")]
    [SerializeField]    
    //[HideInInspector]
    private GameObject indicatorPrefab;

    [Tooltip("List of weapon air drop prefabs")]
    [SerializeField]    
    //[HideInInspector]
    private List<GameObject> weaponDropPrefabs;

    [Tooltip("List of equipment air drop prefabs")]
    [SerializeField]    
    //[HideInInspector]
    private List<GameObject> equipmentDropPrefabs;

    [Tooltip("List of tactical air drop prefabs")]
    [SerializeField]    
    //[HideInInspector]
    private List<GameObject> tacticalDropPrefabs;

    [Space(10)]
    #endregion

    #region Child List References
    [Header ("Child List Properties")]
    public List<CarePackageHolder> cpHolders;
    public List<CarePackageCarrier> airDropFlyers;
    public List<CarePackageFlightIndicator> flightIndicators;
    public List<CarePackageFlightIndicator> availableIndicators;
    #endregion

    #region Air Drop Fields & Modifiers

    [Header("Level Extents Properties")]

    [Tooltip("Collider that represents the playable area where air drops should spawn")]
    public Collider airDropExtentsCollider;
    [Tooltip("Collider that represents the non playable area where flyers will roam")]
    public Collider backDropExtentsCollider;

    [Header("Air Drop Properties")]
    [Tooltip("The type of air drop that will be received")]
    public CarePackageType nextAirDropTypeToSpawn;

    [Tooltip("The number of air drop flyers to spawn")]
    public int airDropFlyersToSpawn = 6;

    [Tooltip("The maximum number of airdrops of the same type that can be active at the same time")]
    [SerializeField]
    private int maxAirDropClones = 3;

    [Tooltip("The maximum number of active airdrops at any one time")]
    [SerializeField]
    private int maxActiveAirDrops = 5;

    [Tooltip("The current number of active airdrops")]
    //[HideInInspector]
    public int currentAirDrops = 0;

    [Space(10)]

    [Tooltip("Randomise air drop frequency?")]
    public bool useRandomAirDropFrequency;

    [Range(5, 60)]
    [Tooltip("The frequency of an air drop")]
    public int airDropFrequency;

    [Space(10)]

    [Tooltip("Randomise the amount of time the air drop will stay active before despawning?")]
    public bool useRandomActiveTime;

    [Tooltip("Override individual timers for air drop active time?")]
    public bool overrideAirDropActiveTime;

    [Range(5, 30)]
    [Tooltip("The amount of time the air drop will stay active before despawning")]
    public float airDropActiveTime;

    [Space(10)]

    [Header("Debugging Properties")]
    [SerializeField] 
    private bool canSpawn = true;      //Can this spawner spawn enemies? This is useful for testing when you want to turn a spawner off

    [SerializeField]
    private bool debugMode;
    #endregion


    #endregion

    #region Functions

    #region Air Drop spawner functions

    public IEnumerator RequestAirDropCo()
    {
        while (canSpawn)                                                                                                                // While we can spawn airDrops             
        {
            yield return new WaitForSeconds(airDropFrequency);                                                                          // Loop every 'frequency' second
            RandomizeAirDropFrequency(10, 30);

            if (currentAirDrops < maxActiveAirDrops)                                                                                    // If we have not exceeded maximum concurrent air drops
            {
                switch (nextAirDropTypeToSpawn)
                {
                    case CarePackageType.Weapon:
                        RequestAirDrop();
                        break;
                    case CarePackageType.Equipment:
                        RequestAirDrop();
                        break;
                    case CarePackageType.Tactical:
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
        availableIndicators.Clear();

        for (int i = 0; i < flightIndicators.Count; i++)
        {
            if (flightIndicators[i].utility.indicatorState == CarePackageFlightIndicatorUtility.IndicatorState.Backdrop)
                availableIndicators.Add(flightIndicators[i]);
        }


        if (availableIndicators.Count > 0)
        {
            int indicatorID = Random.Range(0, availableIndicators.Count);

            availableIndicators[indicatorID].utility.indicatorState = CarePackageFlightIndicatorUtility.IndicatorState.AirDrop;
            availableIndicators[indicatorID].utility.UpdateSpawnDefault(availableIndicators[indicatorID].transform);
        }
        else
            return;
    }

    public void SpawnAirDrop(Transform _spawnPos)
    {
        int dropID = Random.Range(0, weaponDropPrefabs.Count);                                                              // Get a random dropID to spawn a specific type of weapon drop
        RandomizeAirDropType(0, 3);                                                                                         // Determine air drop type
        int holderIndex = (int)nextAirDropTypeToSpawn;

        for (int i = 0; i < maxAirDropClones; i++)
        {
            if (!cpHolders[holderIndex].cpSubHolders[dropID].carePackages[i].gameObject.activeSelf)
            {
                cpHolders[holderIndex].cpSubHolders[dropID].carePackages[i].transform.position = _spawnPos.position + new Vector3(0, -1f, 0);
                cpHolders[holderIndex].cpSubHolders[dropID].carePackages[i].gameObject.SetActive(true);
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
            PopulateAirDropPrefabsFromAssetDatabase((CarePackageType)i);

        InitializeAirDropManager(_thisTransform);

        for (int i = 0; i < 3; i++)
            InitializeAirDropSubHolders(cpHolders[i], i);


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
        CarePackageHolder adHolder = airDropHolderPrefab.GetComponent<CarePackageHolder>();
        cpHolders.Add(GenereateNewAirDropholder(adHolder, Statics.WeaponDropHolderName, _parentTransform));
        cpHolders.Add(GenereateNewAirDropholder(adHolder, Statics.EquipmentDropHolderName, _parentTransform));
        cpHolders.Add(GenereateNewAirDropholder(adHolder, Statics.TacticalDropHolderName, _parentTransform));

    }


    /// <summary>
    /// Sets up the air drop lists to hold the air drop game objects at run time
    /// </summary>
    /// <param name="_airDropList"></param>
    /// <param name="_enumIndex"></param>
    private void InitializeAirDropSubHolders(CarePackageHolder _cpHolder, int _enumIndex)
    {
        switch ((CarePackageType)_enumIndex)
        {
            case CarePackageType.Weapon:
                for (int i = 0; i < weaponDropPrefabs.Count; i++)
                {
                    _cpHolder.cpSubHolders.Add(GenerateNewAirDropSubHolder(cpSubHolderPrefab.GetComponent<CarePackageSubHolder>(), Statics.WeaponDropSubHolderName + i, _cpHolder.transform));
                    InitializeCarePackageSubHolder(_cpHolder, i, weaponDropPrefabs);
                }
                break;
            case CarePackageType.Equipment:
                for (int i = 0; i < equipmentDropPrefabs.Count; i++)
                {
                    _cpHolder.cpSubHolders.Add(GenerateNewAirDropSubHolder(cpSubHolderPrefab.GetComponent<CarePackageSubHolder>(), Statics.EquipmentDropSubHolderName + i, _cpHolder.transform));
                    InitializeCarePackageSubHolder(_cpHolder, i, equipmentDropPrefabs);
                }
                break;
            case CarePackageType.Tactical:
                for (int i = 0; i < tacticalDropPrefabs.Count; i++)
                {
                    _cpHolder.cpSubHolders.Add(GenerateNewAirDropSubHolder(cpSubHolderPrefab.GetComponent<CarePackageSubHolder>(), Statics.TacticalDropSubHolderName + i, _cpHolder.transform));
                    InitializeCarePackageSubHolder(_cpHolder, i, tacticalDropPrefabs);
                }
                break;
            default:
                break;
        }
    }

    private void InitializeCarePackageSubHolder(CarePackageHolder _cpHolder, int _index, List<GameObject> _airDropPrefabs)
    {
        int rand = 0;

        if (useRandomActiveTime)                                                                                                // If we want to use a random value
            rand = Random.Range(5, 30);

        for (int j = 0; j < maxAirDropClones; j++)
        {
            _cpHolder.cpSubHolders[_index].carePackages.Add(GenereateNewAirDrop(_airDropPrefabs[_index].GetComponent<CarePackage>(), _cpHolder.cpSubHolders[_index].transform));         // Add the new air drop to the air drop list
            _cpHolder.cpSubHolders[_index].carePackages[j].dropId = _index;                                                               // Set the weapon drop active time to a random value

            if (overrideAirDropActiveTime)                                                                                              // If we want to override the prefab active time to the air drop manager preset
            {
                if (useRandomActiveTime)                                                                                                // If we want to use a random value
                    _cpHolder.cpSubHolders[_index].carePackages[j].activeTime = rand;                                                                  // Set the weapon drop active time to a random value
                else
                    _cpHolder.cpSubHolders[_index].carePackages[j].activeTime = airDropActiveTime;                                                                      // Else use the preset active time value
            }
        }
    }

    /// <summary>
    /// Populates the air drop prefab lists from the asset directory
    /// </summary>
    /// <param name="_airDropType"></param>
    private void PopulateAirDropPrefabsFromAssetDatabase(CarePackageType _airDropType)
    {
        System.IO.DirectoryInfo dir;                                                                                                                            // Get the file directory for the air drop folder
        int prefabCount = 0;                                                                                                                                    // How many prefabs are present in the directory                               
        GameObject adPrefab;                                                                                                                                    // Temporary game object to store the prefab in 
        string prefabPath = Statics.EmptyString;                                                                                                                // Path to locate each prefab 

        switch (_airDropType)                                                                                                                                   // Initialise variables based on air drop type
        {
            case CarePackageType.Weapon:
                weaponDropPrefabs.Clear();
                dir = new System.IO.DirectoryInfo(Statics.WeaponDropFolderPath);                                                                                    
                prefabCount = dir.GetFiles().Length / 2;                                                                                                            
                prefabPath = Statics.WeaponDropPath;
                break;
            case CarePackageType.Equipment:
                equipmentDropPrefabs.Clear();
                dir = new System.IO.DirectoryInfo(Statics.EquipmentDropFolderPath);
                prefabCount = dir.GetFiles().Length / 2;
                prefabPath = Statics.EquipmentDropPath;
                break;
            case CarePackageType.Tactical:
                tacticalDropPrefabs.Clear();
                dir = new System.IO.DirectoryInfo(Statics.TacticalDropFolderPath);
                prefabCount = dir.GetFiles().Length / 2;
                prefabPath = Statics.TacticalDropPath;
                break;
            default:
                airDropHolderPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(Statics.AirDropHolderPath, typeof(GameObject));
                cpSubHolderPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(Statics.AirDropListPath, typeof(GameObject));

                airDropFlyerPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(Statics.AirPlanePath, typeof(GameObject));
                indicatorPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(Statics.AirPlaneIndicatorPath, typeof(GameObject));
                break;
        }

        for (int i = 0; i < prefabCount; i++)                                                                                                                    // Add air drop prefabs to the list 
        {
            adPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath + i + Statics.PrefabExtension, typeof(GameObject));

            switch (_airDropType)
            {
                case CarePackageType.Weapon:
                    weaponDropPrefabs.Add(adPrefab);
                    break;
                case CarePackageType.Equipment:
                    equipmentDropPrefabs.Add(adPrefab);
                    break;
                case CarePackageType.Tactical:
                    tacticalDropPrefabs.Add(adPrefab);
                    break;
                default:
                    break;
            }
        }
    }

    #endregion

    #region Generate new instances of various air drops & air drop managers

    public CarePackageSubHolder GenerateNewAirDropSubHolder(CarePackageSubHolder _refAirDropList, string _airDropListName, Transform _parentTransform)
    {
        GameObject tempGO = GameObject.Instantiate(_refAirDropList.gameObject);
        CarePackageSubHolder retVal = tempGO.GetComponent<CarePackageSubHolder>();

        //retVal = new CarePackageSubHolder(_airDropListName);
        retVal.gameObject.name = retVal.name;
        retVal.transform.parent = _parentTransform;

        return retVal;
    }

    public CarePackageHolder GenereateNewAirDropholder(CarePackageHolder _refAirDropHolder, string _airDropHolderName, Transform _parentTransform)
    {
        GameObject tempGO = GameObject.Instantiate(_refAirDropHolder.gameObject);
        CarePackageHolder retVal = tempGO.GetComponent<CarePackageHolder>();

        //retVal = new CarePackageHolder(_airDropHolderName);
        retVal.gameObject.name = retVal.name;
        retVal.transform.parent = _parentTransform;

        return retVal;
    }

    public CarePackage GenereateNewAirDrop(CarePackage _refAirDrop, Transform _parentTransform)
    {
        GameObject tempGO = GameObject.Instantiate(_refAirDrop.gameObject);
        CarePackage retVal = tempGO.GetComponent<CarePackage>();
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
        nextAirDropTypeToSpawn = (CarePackageType)Random.Range(_lowerLimit, _upperLimit);
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
        for (int i = 0; i < airDropFlyersToSpawn; i++)
        {
            GameObject tempGO = GameObject.Instantiate(airDropFlyerPrefab.gameObject);
            CarePackageCarrier retVal = tempGO.GetComponent<CarePackageCarrier>();

            retVal.utility = new CarePackageCarrierUtility(Statics.AirDropFlyerName + " " + i);
            retVal.transform.parent = flightHolder.transform;

            retVal.transform.position = ReturnRandPosInAirdropBounds(backDropExtentsCollider, 0.75f, (int)Random.Range(15, 40));

            airDropFlyers.Add(retVal);
        }
    }

    private void InitializeRandIndicatorList()
    {
        for (int i = 0; i < airDropFlyersToSpawn; i++)
        {
            GameObject tempGO = GameObject.Instantiate(indicatorPrefab.gameObject);
            CarePackageFlightIndicator retVal = tempGO.GetComponent<CarePackageFlightIndicator>();

            retVal.utility = new CarePackageFlightIndicatorUtility(Statics.IndicatorName + " " + i);
            retVal.gameObject.name = retVal.utility.name;
            retVal.transform.parent = indicatorHolder.transform;

            retVal.transform.position = ReturnRandPosInAirdropBounds(backDropExtentsCollider, 0.75f, (int)Random.Range(15, 40));

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

