using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CarePackageHolder : MonoBehaviour
{
    public string name;
    public List<CarePackageSubHolder> cpSubHolders = new List<CarePackageSubHolder>();

    /// <summary>
    /// Initialize Air Drop Holder Utility
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_thisTransform"></param>
    public CarePackageHolder(string _name)
    {
        name = _name;                                                                                                // Set the air drop holder name in air drop holder class
    }


    /// <summary>
    /// Override an air drop holder utlity with another air diop holder utility
    /// </summary>
    /// <param name="_holder"></param>
    public CarePackageHolder(CarePackageHolder _holder)
    {
        name = _holder.name;
        cpSubHolders = _holder.cpSubHolders;
    }


}