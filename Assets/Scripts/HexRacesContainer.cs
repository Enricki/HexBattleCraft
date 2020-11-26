using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexRacesContainer
{

    public string name;     
    
    public HexUnit townHole;
    public HexUnit buildUnit;
    public HexUnit meleeUnit;

    public HexRacesContainer(HexUnit Hole, HexUnit bUnit, HexUnit mUnit)
    {
        townHole = Hole;
        buildUnit = bUnit;
        meleeUnit = mUnit;
    }
}
