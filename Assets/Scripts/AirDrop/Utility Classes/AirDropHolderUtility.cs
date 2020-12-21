using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AirDropHolderUtility
{
    public string name;
    public GameObject gameObject;
    public List<AirDropListUtility> airDropLists = new List<AirDropListUtility>();

    /// <summary>
    /// Initialize Air Drop Holder Utility
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_thisTransform"></param>
    public AirDropHolderUtility(string _name, Transform _thisTransform)
    {
        gameObject = new GameObject();                                                                               // Create a new game object for the air drop holder
        gameObject.AddComponent<AirDropHolder>();                                                                    // Assign air drop holder monobehaviour to the new game object
        name = _name;                                                                                                // Set the air drop holder name in air drop holder class
        gameObject.name = _name;                                                                                     // Set the air drop holder name in the inspector
        gameObject.transform.parent = _thisTransform;                                                                // Set the air drop holder parent to the air drop manager
    }

    public AirDropHolderUtility(AirDropHolderUtility _refAirDropHolderUtility)
    {
        name = _refAirDropHolderUtility.name;
        gameObject = _refAirDropHolderUtility.gameObject;
        airDropLists = _refAirDropHolderUtility.airDropLists;
    }


}