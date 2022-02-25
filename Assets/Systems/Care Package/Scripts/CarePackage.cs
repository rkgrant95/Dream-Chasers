using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Rigidbody), typeof(BoxCollider))]
public class CarePackage : MonoBehaviour
{
    public CarePackage_SO cpData;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public Rigidbody rBody;
    [HideInInspector]
    public BoxCollider bCollider;

    //[HideInInspector]
    public GameObject model;
    public GameObject destroyedModel;

    public bool interactable;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rBody.AddForce(Vector3.up * 150);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cpData.IsValidTrigger(other))
        {
            cpData.CollectCarePackage(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        cpData.CollisionEnterCarePackage(this, Statics.GetAverageCollisionPoint(collision));
    }

    private void OnCollisionExit(Collision collision)
    {
        //cpData.CollisionExitCarePackage(this, Statics.GetAverageCollisionPoint(collision));
    }


}
