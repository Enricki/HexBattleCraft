using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexRaces : MonoBehaviour
{

    public static HexRaces Instance = null;

    public List<HexRacesContainer> Races = new List<HexRacesContainer>();
    public static int RaceIndex
    {
        get
        {
            return raceIndex;
        }
        set
        {
            if (raceIndex != value)
            {
                raceIndex = value;
            }
        }
    }
    static int raceIndex;

    void Awake()
    {
        Instance = this;
    }
}
