using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AirDropSubHolderUtility
{
    public string name;
    [SerializeField]
    public List<AirDrop> airDrops = new List<AirDrop>();

    /// <summary>
    /// Sets up the air drop lists to hold the air drop game objects at run time
    /// </summary>
    /// <param name="_airDropList"></param>
    /// <param name="_index"></param>
    public AirDropSubHolderUtility(string _name)
    {
        name = _name;                                                                                                  // Set the name of the list class 
    }

    public AirDropSubHolderUtility(AirDropSubHolderUtility _airDropUtility)
    {
        name = _airDropUtility.name;
        airDrops = _airDropUtility.airDrops;
    }
}