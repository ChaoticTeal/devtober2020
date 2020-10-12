using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SeasonalTile : ScriptableObject
{
    [Tooltip("Seasonal variants of each tile.\nSpring, summer, fall, winter.")]
    public TileBase[] seasonTiles;
}
