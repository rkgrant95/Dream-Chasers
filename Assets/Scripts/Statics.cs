using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics 
{

    #region Utilities
    public static string EmptyString = "";
    public static string PrefabExtension = ".prefab";
    public static string IndicatorName = "Indicator";
    public static string AirDropFlyerName = "Air-drop flyer";
    #endregion
    
    #region Air Drop Prefab Path Identification Strings

    #region Weapon ID's
    // Path for weapon drop prefabs in the asset folder. Need to add oweapon drop index & .prefab for the path to be valid
    public static string WeaponDropPath = "Assets/Prefabs/Air Drop Prefabs/Weapon Air Drop Prefabs/Weapon Air Drop ";
    public static string WeaponDropFolderPath = "Assets/Prefabs/Air Drop Prefabs/Weapon Air Drop Prefabs";
    #endregion

    #region Equipment ID's
    // Path for weapon drop prefabs in the asset folder. Need to add oweapon drop index & .prefab for the path to be valid
    public static string EquipmentDropPath = "Assets/Prefabs/Air Drop Prefabs/Equipment Air Drop Prefabs/Equipment Air Drop ";
    public static string EquipmentDropFolderPath = "Assets/Prefabs/Air Drop Prefabs/Equipment Air Drop Prefabs";
    #endregion

    #region Tactical ID's
    // Path for weapon drop prefabs in the asset folder. Need to add oweapon drop index & .prefab for the path to be valid
    public static string TacticalDropPath = "Assets/Prefabs/Air Drop Prefabs/Tactical Air Drop Prefabs/Tactical Air Drop ";
    public static string TacticalDropFolderPath = "Assets/Prefabs/Air Drop Prefabs/Tactical Air Drop Prefabs";
    #endregion

    #region Airplane ID's
    public static string AirPlanePath = "Assets/Prefabs/Air Drop Prefabs/Air Drop Plane Prefabs/Air Drop Plane Prefab.prefab";
    public static string AirPlaneFolderPath = "Assets/Prefabs/Air Drop Prefabs/Air Drop Plane Prefabs";
    #endregion

    #region Airplane Indicator ID's
    public static string AirPlaneIndicatorPath = "Assets/Prefabs/Air Drop Prefabs/Air Drop Indicator Prefabs/Air Drop Indicator Prefab.prefab";
    public static string AirPlaneIndicatorFolderPath = "Assets/Prefabs/Air Drop Prefabs/Air Drop Indicator Prefabs";
    #endregion

    #region Sub-Holder Names
    public static string WeaponDropListName = "Weapon Drop Sub-Holder ";
    public static string EquipmentDropListName = "Equipment Drop Sub-Holder ";
    public static string TacticalDropListName = "Tactical Drop Sub-Holder ";
    #endregion

    #endregion

    #region Air Drop Holder Identification Strings

    #region Holder Paths
    public static string AirDropManagerFolderPath = "Assets/Prefabs/Air Drop Prefabs/Air Drop Manager Prefabs";

    public static string AirDropManagerPath = "Assets/Prefabs/Air Drop Prefabs/Air Drop Manager Prefabs/Air Drop Manager.prefab";
    public static string AirDropListPath = "Assets/Prefabs/Air Drop Prefabs/Air Drop Manager Prefabs/Air Drop List Prefab.prefab";
    public static string AirDropHolderPath = "Assets/Prefabs/Air Drop Prefabs/Air Drop Manager Prefabs/Air Drop Holder Prefab.prefab";
    #endregion

    #region Holder Names
    public static string WeaponDropHolderName = "Weapon Drop Holder";
    public static string EquipmentDropHolderName = "Equipment Drop Holder";
    public static string TacticalDropHolderName = "Tactical Drop Holder";
    #endregion

    #region Holder Names
    public static string WeaponDropSubHolderName = "Weapon Drop Sub-Holder ";
    public static string EquipmentDropSubHolderName = "Equipment Drop Sub-Holder ";
    public static string TacticalDropSubHolderName = "Tactical Drop Sub-Holder ";
    #endregion

    #endregion

    #region Animation Variables

    #region Plane Animation Variables
    public static string BarrelRollAnim = "Barrel Roll";
    public static string TurbulenceAnim = "Turbulence";
    public static string ReverseAnim = "Reverse";
    #endregion
    #endregion


    #region Utility Functions


    #endregion
}
