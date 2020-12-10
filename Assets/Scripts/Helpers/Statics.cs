using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    // Layermask reference for enemy layer
    public static LayerMask enemyLayer = LayerMask.NameToLayer("Enemy");
    // Layermask reference for enemy layer
    public static LayerMask playerLayer = LayerMask.NameToLayer("LocalPlayer");
    // Layermask reference for ignore raycast layer
    public static LayerMask ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");

    #region Enemy References
    // Static references to enemy names as string
    public static string zomBunnyName = "ZomBunny";
    public static string zomBearName = "ZomBear";
    public static string zomDuckName = "ZomDuck";
    public static string zomClownName = "ZomClown";
    public static string hellephantName = "Hellephant";

    public static int enemyTypeCount = 5;                   // The amount of enemy types in the game
    #endregion
}
