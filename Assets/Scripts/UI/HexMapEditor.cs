using UnityEngine;
using UnityEngine.EventSystems;

enum OptionalToggle
{
    Ignore, Yes, No
}

public class HexMapEditor : MonoBehaviour
{
    OptionalToggle riverMode, roadMode, walledMode;

    public HexGrid hexGrid;
    public HexMapCamera MapCamera;
    public Material terrainMaterial;

    public bool touchedUI = false;

    int activeTerrainTypeIndex = -1;
    int activeElevation;
    int activeWaterLevel;
    int brushsize;
    int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;

    bool applyElevation = false;
    bool applyWaterLevel = false;
    bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;
    bool applyPathMode;
    bool SpawnUnit = false;
    bool editMode = true;
    HexDirection dragDirection;
    HexCell previousCell;



    bool isDrag;


    void Awake()
    {
        terrainMaterial.DisableKeyword("GRID_ON");
        Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
        SetEditMode(true);
    }

    void LateUpdate()
    {

        if (Input.GetMouseButton(0))
        {
            if (!HexMapCamera.TouchedUI)
            {
                HandleInput();
                if (SpawnUnit)
                {
                    CreateUnit();
                }
                else if (!SpawnUnit)
                {
                    DestroyUnit();
                }
                return;
            }

        }
        previousCell = null;
    }
    HexCell GetCellUnderCursor()
    {
        return
            hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    void HandleInput()
    {
        HexCell currentCell = GetCellUnderCursor();
        if (currentCell)
        {
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            EditCells(currentCell);
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }

    void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && !cell.Unit)
        {
            hexGrid.AddUnit(
                Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f), true);
        }
    }

    void DestroyUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && cell.Unit)
        {
            hexGrid.RemoveUnit(cell.Unit);
        }
    }

    void ValidateDrag(HexCell currentCell)
    {
        for (
            dragDirection = HexDirection.NE;
            dragDirection <= HexDirection.NW;
            dragDirection++
            )
        {
            if (previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushsize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushsize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushsize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushsize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }


    void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (activeTerrainTypeIndex >= 0)
            {
                cell.TerrainTypeIndex = activeTerrainTypeIndex;
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }
            if (applyWaterLevel)
            {
                cell.WaterLevel = activeWaterLevel;
            }
            if (applySpecialIndex)
            {
                cell.SpecialIndex = activeSpecialIndex;
            }
            if (riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            }
            if (applyUrbanLevel)
            {
                cell.UrbanLevel = activeUrbanLevel;
            }
            if (applyFarmLevel)
            {
                cell.FarmLevel = activeFarmLevel;
            }
            if (applyPlantLevel)
            {
                cell.PlantLevel = activePlantLevel;
            }
            if (roadMode == OptionalToggle.No)
            {
                cell.RemoveRoads();
            }
            if (walledMode != OptionalToggle.Ignore)
            {
                cell.Walled = walledMode == OptionalToggle.Yes;
            }
            if (isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                {
                    if (riverMode == OptionalToggle.Yes)
                    {
                        otherCell.SetOutgoingRiver(dragDirection);
                    }
                    if (roadMode == OptionalToggle.Yes)
                    {
                        otherCell.AddRoad(dragDirection);
                    }

                }
            }
        }
    }


    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
        CanMove();
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
        CanMove();
    }

    public void SetBrushSize(float size)
    {
        brushsize = (int)size;
    }
    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
        CanMove();
    }

    public void SetRoadMode(int mode)
    {
        roadMode = (OptionalToggle)mode;
        CanMove();
    }

    public void SetWalledMode(int mode)
    {
        walledMode = (OptionalToggle)mode;
        CanMove();
    }
    public void SetApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
        CanMove();
    }

    public void SetWaterLevel(float level)
    {
        activeWaterLevel = (int)level;
    }

    public void SetApplyUrbanLevel(bool toggle)
    {
        applyUrbanLevel = toggle;
        CanMove();
    }

    public void SetUrbanLevel(float level)
    {
        activeUrbanLevel = (int)level;
    }

    public void SetApplyFarmLevel(bool toggle)
    {
        applyFarmLevel = toggle;
        CanMove();
    }

    public void SetFarmLevel(float level)
    {
        activeFarmLevel = (int)level;
    }

    public void SetApplyPlantLevel(bool toggle)
    {
        applyPlantLevel = toggle;
        CanMove();
    }

    public void SetPlantLevel(float level)
    {
        activePlantLevel = (int)level;
    }

    public void SetApplySpecialIndex(bool toggle)
    {
        applySpecialIndex = toggle;
        CanMove();
    }

    public void SetSpecialIndex(float index)
    {
        activeSpecialIndex = (int)index;
    }

    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
        CanMove();
    }

    public void ShowGrid(bool visible)
    {
        if (visible)
        {
            terrainMaterial.EnableKeyword("GRID_ON");
        }
        else
        {
            terrainMaterial.DisableKeyword("GRID_ON");
        }
    }

    public void SetEditMode(bool toggle)
    {
        enabled = toggle;
        editMode = toggle;
        CanMove();
    }

    public void SetApplyPathMode(bool toggle)
    {
        applyPathMode = toggle;
        CanMove();
    }

    public void SetApplyUnitMode(bool toggle)
    {
        SpawnUnit = toggle;
        CanMove();
    }

    void CanMove()
    {
        if (editMode)
        {
            if (SpawnUnit || applySpecialIndex || applyPlantLevel || applyFarmLevel || applyUrbanLevel || applyWaterLevel || applyElevation ||
                riverMode != OptionalToggle.Ignore || roadMode != OptionalToggle.Ignore || walledMode != OptionalToggle.Ignore || activeTerrainTypeIndex >= 0)
            {
                HexMapCamera.BlockedCam = true;
            }
            else
            {
                HexMapCamera.BlockedCam = false;
            }
        }
        else
        {
            HexMapCamera.BlockedCam = false;
        }
    }
}
