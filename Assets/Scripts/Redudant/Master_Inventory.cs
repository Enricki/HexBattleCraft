using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master_Inventory : MonoBehaviour
{

    public static Master_Inventory Instance = null;

    public List<WeaponsList> m_WeaponsList = new List<WeaponsList>();

    void Awake()
    {
        Instance = this;
    }
}
