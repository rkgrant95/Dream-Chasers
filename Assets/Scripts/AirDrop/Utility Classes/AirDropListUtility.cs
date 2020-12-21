using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AirDropListUtility
{
    public string name;
    public GameObject gameObject;
    public List<AirDrop> airDrops = new List<AirDrop>();

    /// <summary>
    /// Sets up the air drop lists to hold the air drop game objects at run time
    /// </summary>
    /// <param name="_airDropList"></param>
    /// <param name="_index"></param>
    public AirDropListUtility(string _name, Transform _parentTransform)
    {
        gameObject = new GameObject();                                                                                 // Create a new game object at index in the list
        gameObject.AddComponent<AirDropList>();                                                                        // Assign air drop list monobehaviour to the new game object
        name = _name;                                                                                                  // Set the name of the list class 
        gameObject.name = name;                                                                                        // Set the name of the list game object in the inspector
        gameObject.transform.parent = _parentTransform;                                                                // Set the air drop list parent to the air drop holder
    }

    public AirDropListUtility(AirDropListUtility _airDropUtility)
    {
        name = _airDropUtility.name;
        gameObject = _airDropUtility.gameObject;
        airDrops = _airDropUtility.airDrops;
    }
}