using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarePackageManager : MonoBehaviour
{
    public CarePackageManagerUtility utility;

    void Awake()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        utility.Initialize(this.transform);
        StartCoroutine(utility.RequestAirDropCo());

        StartCoroutine(utility.ShowDebugs(this.gameObject));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        utility.FixedTick();
    }
}
