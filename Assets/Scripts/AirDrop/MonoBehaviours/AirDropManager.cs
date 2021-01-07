using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDropManager : MonoBehaviour
{
    public AirDropManagerUtility adUtility;
    public AirDropFlightManagerUtility adfUtility;


    #region create class for flyer manager
    public Collider airDropZone;
    public Collider backdropZone;
    #endregion


    void Awake()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        adUtility.Initialize(this.transform);
        StartCoroutine(adUtility.SpawnAirDrop());

        adfUtility.Initialize(this.transform);
        StartCoroutine(adfUtility.ShowDebugs(this.gameObject));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        adfUtility.FixedTick();
    }
}
