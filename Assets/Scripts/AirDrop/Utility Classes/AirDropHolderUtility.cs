﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AirDropHolderUtility
{
    public string name;
    public List<AirDropSubHolder> airDropSubHolder = new List<AirDropSubHolder>();

    /// <summary>
    /// Initialize Air Drop Holder Utility
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_thisTransform"></param>
    public AirDropHolderUtility(string _name)
    {
        name = _name;                                                                                                // Set the air drop holder name in air drop holder class
    }


    /// <summary>
    /// Override an air drop holder utlity with another air diop holder utility
    /// </summary>
    /// <param name="_refAirDropHolderUtility"></param>
    public AirDropHolderUtility(AirDropHolderUtility _refAirDropHolderUtility)
    {
        name = _refAirDropHolderUtility.name;
        airDropSubHolder = _refAirDropHolderUtility.airDropSubHolder;
    }


}