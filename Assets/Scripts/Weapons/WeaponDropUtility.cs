using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponID { Weapon1, Weapon2, Weapon3, Weapon4, Weapon5, }

[System.Serializable]
public class WeaponDropUtility 
{
    public WeaponID weaponId;


    public void Initialize()
    {
        switch (weaponId)
        {
            case WeaponID.Weapon1:
                break;
            case WeaponID.Weapon2:
                break;
            case WeaponID.Weapon3:
                break;
            case WeaponID.Weapon4:
                break;
            case WeaponID.Weapon5:
                break;
            default:
                break;
        }
    }
}

[System.Serializable]
public class WeaponDropManagerUtility
{
    public List<WeaponDropUtility> weaponDrops;


}

