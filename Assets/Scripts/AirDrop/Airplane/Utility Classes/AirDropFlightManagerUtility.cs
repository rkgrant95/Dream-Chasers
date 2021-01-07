using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AirDropFlightManagerUtility
{
    private GameObject flightHolder;
    private GameObject indicatorHolder;

    public GameObject airDropFlyerPrefab;

    public List<AirDropRandomFlyer> airDropFlyers;
    public List<GameObject> flightIndicators;

    public int airDropFlyerCount = 6;
    public int airDropSpawnBacklog = 0;

    public bool debugMode;

    public string name;

    public AirDropFlightManagerUtility()
    {

    }

    public AirDropFlightManagerUtility(string _name, int _flyerCount, int _adIndicatorCount)
    {
        name = _name;
        airDropFlyerCount = _flyerCount;
    }

    public void Initialize(Transform _managerTransform)
    {
        GenerateHolders();
        InitializeHolders(_managerTransform);

        InitializeRandIndicatorList();
        InitializeFlyerList();

        InitializeFlyers();
    }

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
            airDropFlyers[i].utility.Initialize(airDropFlyers[i].gameObject, flightIndicators[i].gameObject);
    }

    private void InitializeFlyerList()
    {
        for (int i = 0; i < airDropFlyerCount; i++)
        {
            GameObject tempGO = GameObject.Instantiate(airDropFlyerPrefab.gameObject);
            AirDropRandomFlyer retVal = tempGO.GetComponent<AirDropRandomFlyer>();

            retVal.utility = new AirDropRandomFlyerUtility(Statics.AirDropFlyerName + " " + i);
            retVal.transform.parent = flightHolder.transform;
            TestTargetPositionIndicator(retVal.transform);

            airDropFlyers.Add(retVal);
        }
    }

    private void InitializeRandIndicatorList()
    {
        for (int i = 0; i < airDropFlyerCount; i++)
        {
            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            indicator.name = Statics.IndicatorName + " " + i;

            indicator.AddComponent<AirDropFlightIndicator>();
            indicator.GetComponent<BoxCollider>().isTrigger = true;
            indicator.GetComponent<BoxCollider>().size = new Vector3(2.5f, 2.5f, 2.5f);

            indicator.transform.parent = indicatorHolder.transform;
            TestTargetPositionIndicator(indicator.transform);

            flightIndicators.Add(indicator);
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

    public Vector3 ReturnRandomPositionInBounds(Collider _collider)
    {
        return new Vector3(Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
        Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
        Random.Range(_collider.bounds.min.z, _collider.bounds.max.z));
    }


    public void TestTargetPositionIndicator(Transform _objectToPosition)
    {
        Collider tempBounds = GameManager.Instance.AirDropManager.backdropZone;

     

        Vector3 randomPosInBounds = new Vector3(ReturnRandomPositionInBounds(tempBounds).x * 0.75f, (int)Random.Range(15, 40),
            ReturnRandomPositionInBounds(tempBounds).z * 0.75f);

        _objectToPosition.position = randomPosInBounds;
    }

}
