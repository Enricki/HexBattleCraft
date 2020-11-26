using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponsList
{

    public string name;                            // Inspector Element Name
    public GameObject weaponGameObject;            // Weapon's physical GameObject
    public string weaponName;                    // Weapon name
    public string weaponDescription;            // Weapon description
    public int weaponLevel;                        // Weapon level
    public bool isRangedWeapon;                    // Is this a ranged weapon? (true=ranged; false=melee)
    public bool isExplosive;                    // Is this an explosive weapon?
    public float weaponRange;                    // Weapon range
    public float weaponDamage;                    // Weapon damage
    public float weaponFireRate;                // Weapon rate of fire

    public WeaponsList(GameObject newWeapon, string newName, string newDescription, int newLevel, bool newIsRangedWeapon, bool newIsExplosive,
        float newWeaponRange, float newWeaponDamage, float newWeaponFireRate)
    {
        weaponGameObject = newWeapon;
        weaponName = newName;
        weaponDescription = newDescription;
        weaponLevel = newLevel;
        isRangedWeapon = newIsRangedWeapon;
        isExplosive = newIsExplosive;
        weaponRange = newWeaponRange;
        weaponDamage = newWeaponDamage;
        weaponFireRate = newWeaponFireRate;
    }

}
