using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarePackageSubHolder : MonoBehaviour
{
    [SerializeField]
    public List<CarePackage> carePackages = new List<CarePackage>();

    /// <summary>
    /// Sets up the air drop lists to hold the air drop game objects at run time
    /// </summary>
    /// <param name="_airDropList"></param>
    /// <param name="_index"></param>
    public CarePackageSubHolder(string _name)
    {
        name = _name;                                                                                                  // Set the name of the list class 
    }

    public CarePackageSubHolder(CarePackageSubHolder _subHolder)
    {
        name = _subHolder.name;
        carePackages = _subHolder.carePackages;
    }
}