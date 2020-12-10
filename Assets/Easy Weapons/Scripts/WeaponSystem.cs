/// <summary>
/// WeaponSystem.cs
/// Author: MutantGopher
/// This script manages weapon switching.  It's recommended that you attach this to a parent GameObject of all your weapons, but this is not necessary.
/// This script allows the player to switch weapons in two ways, by pressing the numbers corresponding to each weapon, or by scrolling with the mouse.
/// </summary>

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WeaponSystem : MonoBehaviour
{
	public WeaponSystemData weaponSystemData;
	//public GameObject gameObject;               // A reference to this game object
	public GameObject[] weapons;                // The array that holds all the weapons that the player has

	public WeaponData weapon = new WeaponData();
												// Use this for initialization
	void Start()
	{
		//weaponSystemData.gameObject = this.gameObject;

		weaponSystemData.Initialize(weapons);

		/*
		for (int i = 0; i < weapons.Length; i++)
		{
			weaponList.Add(weapons[i].GetComponent<Weapon>().weaponData);
		}
		*/

		weapon = new WeaponData(weapons[1].GetComponent<Weapon>().weaponData);						// Create a new weapondata using a weapon in the inspector
		//weapon = new WeaponData(DataManager.Instance.weaponDataContainer.weaponDataList[0]);							// Override our weapon data using our saved weapon data

	}

	// Update is called once per frame
	void Update()
	{
		weaponSystemData.Tick(weapons);

		if(Input.GetKeyDown(KeyCode.S))
		{
			DataManager.Instance.weaponDataContainer.weaponDataList[0] = new WeaponData(weapon);            // Assign new weapondata to our saved weapon data
			DataManager.Instance.weaponDataContainer.Save();
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			DataManager.Instance.weaponDataContainer = WeaponDataContainer.Load();
		}

	}

	void OnGUI()
	{


	}


}
