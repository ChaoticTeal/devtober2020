using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public enum Seasons { Spring, Summer, Fall, Winter };

    [SerializeField]
    private List<Tilemap> tilemaps;

    [SerializeField]
    private List<SeasonalTile> seasonalTiles;

    [SerializeField]
    [Tooltip("The parent objects for any seasonal tilemaps (for decor/interactive elements that change by season)")]
    private List<GameObject> seasonalRootObjects;

    [SerializeField]
    private Seasons startingSeason = Seasons.Spring;

    private Dictionary<TileBase, SeasonalTile> tiles = new Dictionary<TileBase, SeasonalTile>();
    private Seasons currentSeason;

    // Start is called before the first frame update
    void Awake()
    {
        foreach(SeasonalTile st in seasonalTiles)
            foreach (TileBase tb in st.seasonTiles)
                tiles.Add(tb, st);
    }

    private void Start()
    {
        currentSeason = startingSeason;
        SetSeasonalObjects();
    }

    private void ChangeSeason()
    {
        SeasonalTile st;
        IncrementSeason();
        SetSeasonalObjects();
        foreach(Tilemap tm in tilemaps)
        {
            tm.CompressBounds();
            BoundsInt tilemapBounds = tm.cellBounds;
            for(int x = tilemapBounds.xMin; x <= tilemapBounds.xMax; x++)
                for(int y = tilemapBounds.yMin; y <= tilemapBounds.yMax; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tile = tm.GetTile(tilePosition);
                    if(tile != null && tiles.TryGetValue(tile, out st))
                        tm.SetTile(tilePosition, st.seasonTiles[(int)currentSeason]);
                }
        }
    }

    private void SetSeasonalObjects()
    {
        for(int i = 0; i <= 3; i++)
        {
            if (i == (int)currentSeason)
            {
                seasonalRootObjects[i].SetActive(true);
                continue;
            }
            seasonalRootObjects[i].SetActive(false);
        }
    }

    private void IncrementSeason()
    {
        if (currentSeason < Seasons.Winter)
            currentSeason++;
        else
            currentSeason = Seasons.Spring;
    }

    public void GetSeasonChangeInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
            ChangeSeason();
    }
}
