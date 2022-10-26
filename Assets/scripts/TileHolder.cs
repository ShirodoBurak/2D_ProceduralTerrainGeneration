using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileHolder : MonoBehaviour
{

    [Header("Textures")]
    public TileBase grass;
    public TileBase dirt;
    public TileBase stone;
    public TileBase iron;
    //[Header("Experimental")]
    //public TileBase[] lantern = new TileBase[1];
    public Dictionary<string, TileBase> Tiles = new Dictionary<string, TileBase>();
    private void Start()
    {
        Tiles.Add("default:grass", grass);
        Tiles.Add("default:dirt", dirt);
        Tiles.Add("default:stone", stone);
        Tiles.Add("default:iron", iron);
    }
    public TileBase getTile(string tilename)
    {
        TileBase value;
        Tiles.TryGetValue(tilename, out value);
        return value;
    }
}
