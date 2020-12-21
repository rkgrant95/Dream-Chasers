using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDropManager : MonoBehaviour
{
    public AirDropManagerUtility airDropManager;


    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        airDropManager.Initialize(this.transform);
        StartCoroutine(airDropManager.SpawnAirDrop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
