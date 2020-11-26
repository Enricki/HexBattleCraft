using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRaceMenu : MonoBehaviour
{
    public HexGrid grid;
    public void Open()
    {
        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
    }

    public void ChooseRace (int index)
    {
        HexRaces.RaceIndex = index;
    }
}
