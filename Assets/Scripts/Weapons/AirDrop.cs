using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDrop : MonoBehaviour
{
    public AirDropUtility airDrop;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerHealth>())
        {
            other.GetComponent<PlayerHealth>().weaponSystem.SetActiveWeapon(airDrop.dropId);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
      //  weaponDrop.Initialise(weaponDrop);
    }

}
