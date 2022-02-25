using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarePackageManager : MonoBehaviour
{

    public List<CarePackage_SO> carePackageSOList;
    public List<CarePackage> carePackages;
    // Start is called before the first frame update
    void Start()
    {
        carePackageSOList[0].spawnPoint = this.transform;
        carePackageSOList[0].carePackageHolder = this.transform;
        carePackages.Add(carePackageSOList[0].GenerateCarePackage());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            carePackages[0].cpData.ActivateCarePackage(carePackages[0]);
        }
    }
}
