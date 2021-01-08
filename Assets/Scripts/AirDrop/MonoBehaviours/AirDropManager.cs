using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDropManager : MonoBehaviour
{
    public AirDropManagerUtility utility;


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
        utility.Initialize(this.transform);
        StartCoroutine(utility.RequestAirDrop());

        StartCoroutine(utility.ShowDebugs(this.gameObject));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        utility.FixedTick();
    }
}
