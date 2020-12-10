/// <summary>
/// Weapon.cs
/// Author: MutantGopher
/// This is the core script that is used to create weapons.  There are 3 basic
/// types of weapons that can be made with this script:
/// 
/// Raycast - Uses raycasting to make instant hits where the weapon is pointed starting at
/// the position of raycastStartSpot
/// 
/// Projectile - Instantiates projectiles and lets them handle things like damage and accuracy.
/// 
/// Beam - Uses line renderers to create a beam effect.  This applies damage and force on 
/// every frame in small amounts, rather than all at once.  The beam type is limited by a
/// heat variable (similar to ammo for raycast and projectile) unless otherwise specified.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.IO;

public enum WeaponType
{
	Projectile,
	Raycast,
	Beam
}
public enum Auto
{
	Full,
	Semi
}
public enum BulletHoleSystem
{
	Tag,
	Material,
	Physic_Material
}


[System.Serializable]
public class SmartBulletHoleGroup
{
	public string tag;
	public Material material;
	public PhysicMaterial physicMaterial;
	public BulletHolePool bulletHole;
	
	public SmartBulletHoleGroup()
	{
		tag = "Everything";
		material = null;
		physicMaterial = null;
		bulletHole = null;
	}
	public SmartBulletHoleGroup(string t, Material m, PhysicMaterial pm, BulletHolePool bh)
	{
		tag = t;
		material = m;
		physicMaterial = pm;
		bulletHole = bh;
	}
}





// The Weapon class itself handles the weapon mechanics
[System.Serializable]
public class Weapon : MonoBehaviour
{
	public WeaponData weaponData;

	// Use this for initialization
	void Start()
	{
		Debug.Log("Prefab directory is: " + GetComponent<WeaponData>().shell.GetComponent<FileInfo>().Directory);

		weaponData.gameObject = this.gameObject;
		weaponData.monoBehaviour = GetComponent<MonoBehaviour>();
		weaponData.Initialize();

	}

	// Update is called once per frame
	void Update()
	{
		weaponData.Tick();
	}

	private void OnGUI()
	{
		weaponData.OnGUI();
	}


}


