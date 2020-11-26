using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

enum MoveState { selected, unselected, searching, endOfMoving};

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;
    HexCell currentCell, startCell, endCell;
    HexUnit selectedUnit, previousUnit;
    public Material terrainMaterial;
    public static int movePower = 0;
    public static int maxMoveTurn = 0;

    MoveState state;



    void Awake()
    {
        terrainMaterial.EnableKeyword("GRID_ON");
        Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
        //        SetEditMode(false);
    }
    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        grid.ShowUI(!toggle);
        grid.ClearPath();
        if (toggle)
        {
            Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
        }
        else
        {
            Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
        }
    }

    bool UpdateCurrentCell()
    {
        HexCell cell =
            grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != currentCell)
        {
            currentCell = cell;
            return true;
        }
        return false;
    }





    void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (currentCell && selectedUnit.IsValidDestination(currentCell))
            {
                    grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
            }
            else
            {
                grid.ClearPath();
            }
        }
    }
    void DoMove()
    {
        if (grid.HasPath)
        {

            selectedUnit.Travel(grid.GetPath());
            grid.ClearPath();
            grid.innactiveUnits.Add(selectedUnit.Location);
            startCell = selectedUnit.Location;
            selectedUnit.canMove = false;
            selectedUnit.beActive = false;
            selectedUnit = null;
            HexMapCamera.BlockedCam = false;
            Moved = true;
        }
    }
    void DoSelection()
    {
        grid.ClearPath();
        UpdateCurrentCell();
        if (currentCell)
        {
            selectedUnit = currentCell.Unit;
            if (selectedUnit)
            {
                startCell = currentCell;
                HexMapCamera.BlockedCam = true;
                selectedUnit.beActive = true;
                if (!previousUnit)
                {
                    previousUnit = selectedUnit;
                }
                else
                {
                    if (selectedUnit != previousUnit)
                    {
                        
                        previousUnit.beActive = false;
                        UpdateUI(previousUnit);
                    }
                    previousUnit = selectedUnit;
                }
                UpdateUI(selectedUnit);
            }
            else
            {
                HexMapCamera.BlockedCam = false;
                if (previousUnit)
                {
                    previousUnit.beActive = false;
                    UpdateUI(previousUnit);
                } 
            }
        }
    }

    void UpdateUI(HexUnit unit)
    {
        if (unit)
        {
            if (unit.beActive)
            {
                unit.Location.EnableHighlight(Color.green, 0);
            }
            else
            {
                if (unit.canMove)
                {
                    unit.Location.DisableHighlight();
                }
                else
                {
                    unit.Location.EnableHighlight(Color.grey, 0);
                }
            }
        }
    }
    bool Moved = false;
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (!HexMapCamera.MultyTouch && !HexMapCamera.MovedCamera)
            {
                if (!selectedUnit)
                {
                    DoSelection();
                }
                else
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        endCell = grid.GetCell(Camera.main.ScreenPointToRay(Input.touches[0].position));
                        if (currentCell == endCell)
                        {
                            if (selectedUnit.canMove)
                            {
                                DoMove();
                            }
                            else
                            {
                                DoSelection();
                            }
                        }
                        else
                        {
                            DoSelection();
                        }
                        Debug.Log(currentCell.coordinates);
                        Debug.Log(currentCell == endCell);
                        Debug.Log(endCell.coordinates);
                    }
                    if (Input.touches[0].phase == TouchPhase.Ended)
                    {
                        if (Moved)
                        {
                            selectedUnit.Location.EnableHighlight(Color.grey, 0);
                            Moved = false;
                        }
                    }
                    if (Input.touches[0].phase == TouchPhase.Moved)
                    {
                        if (selectedUnit.canMove)
                        {
                            DoPathfinding();
                        }
                    }
                }
            }
        }
    }

    //private void Update()
    //{
    //    if (!EventSystem.current.IsPointerOverGameObject() && !HexMapCamera.MultyTouch)
    //    {
    //        if (Input.GetMouseButtonDown(0) && !click)
    //        {
    //            DoSelection();
    //        }
    //        else if (selectedUnit && click)
    //        {
    //            if (Input.GetMouseButtonDown(0))
    //            {
    //                //if (currentCell != startCell)
    //                //{
    //                DoMove();

    //                //}
    //                //else
    //                //{
    //                //    selectedUnit = null;
    //                //    currentCell.DisableHighlight();
    //                //    click = false;
    //                //}
    //                if (!canMove)
    //                {
    //                    startCell.EnableHighlight(Color.grey, 0);
    //                    selectedUnit = null;
    //                    click = false;
    //                }
    //            }
    //            else if (selectedUnit.canMove)
    //            {
    //                DoPathfinding();
    //                HexMapCamera.BlockedCam = true;
    //            }
    //        }
    //    }
    //}

    public void nextTurn()
    {
        startCell = null;
        selectedUnit = null;
        previousUnit = null;

        for (int i = 0; i < grid.innactiveUnits.Count; i++)
        {
            grid.innactiveUnits[i].Unit.canMove = true;
            grid.innactiveUnits[i].DisableHighlight();
        }
        grid.innactiveUnits.Clear();
    }
}
