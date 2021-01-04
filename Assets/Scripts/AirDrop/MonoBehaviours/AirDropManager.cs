using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDropManager : MonoBehaviour
{
    public AirDropManagerUtility utiity;

    #region create class for flyer manager
    public Transform homeTarget;
    public Transform flyingTarget;
    #endregion
    void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        utiity.Initialize(this.transform);
        StartCoroutine(utiity.SpawnAirDrop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
