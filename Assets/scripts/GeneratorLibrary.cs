using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorLibrary : MonoBehaviour
{
    #region Structs and Classes
    [System.Serializable]
    public class NoiseProperties
    {
        // One directional noise is usually just for Height values, two dimensional ones are for cave generation, ore and material generation.
        public enum NoiseType { ONE_DIMENSIONAL, TWO_DIMENSIONAL };
        public NoiseType noiseType;
        public long seed;
        // Sets the scale of noise.
        public float scale;
        public float multiplier;
    }
    public struct Tree
    {
        public Dictionary<Vector2Int, string> location;
        // < Coordinates, "TileName" >
    }

    public struct Tile
    {
        public int x;
        public int y;
        public string tilename;
    }
    public struct Chunk
    {
        public int x;
        public int y;
        public Tile[] tiles;
    }
    public struct Region
    {
        public Chunk[] chunks;
    }
    #endregion



    #region Variables
    Dictionary<string, Tree> treeVariations;
    public TileHolder tileLib;
    public int chunkWidth = 16;
    public Vector2Int offset;
    public NoiseProperties[] noiseSettings;
    public Tilemap world;
    public TileBase tile;
    public DataController dataController;
    public List<Vector2Int> loadedChunks = new List<Vector2Int>();

    int ONE_DIMENSIONAL_LAYER_COUNT = 0;
    int TWO_DIMENSIONAL_LAYER_COUNT = 0;





    #endregion
    void Start()
    {
        int oneDimCount = 0;
        int twoDimCount = 0;

        foreach (var item in noiseSettings)
        {
            if (item.noiseType == NoiseProperties.NoiseType.ONE_DIMENSIONAL)
            {
                oneDimCount++;
            }
            else
            {
                twoDimCount++;
            }
        }

        ONE_DIMENSIONAL_LAYER_COUNT = oneDimCount;
        TWO_DIMENSIONAL_LAYER_COUNT = twoDimCount;
    }

    #region Tree and Vegetation Data

    void TreeInit()
    {
        Tree tree = new Tree();
        tree.location.Add(new Vector2Int(0, 0), "default:log");
        tree.location.Add(new Vector2Int(0, 1), "default:log");
        tree.location.Add(new Vector2Int(0, 2), "default:log");
        tree.location.Add(new Vector2Int(0, 3), "default:log_branch_right");
        tree.location.Add(new Vector2Int(1, 3), "default:branch_right_leaves");
        tree.location.Add(new Vector2Int(0, 4), "default:log");
        tree.location.Add(new Vector2Int(0, 5), "default:log_branch_left");
        tree.location.Add(new Vector2Int(-1, 5), "default:branch_left_leaves");
        tree.location.Add(new Vector2Int(0, 6), "default:log_top");


        treeVariations.Add("Tree_1", tree);
    }
    #endregion
    #region Noise
    public Vector2[] GenerateNoise1D(Vector2Int cPos)
    {
        Vector2[] result = new Vector2[chunkWidth];
        for (int i = cPos.x * chunkWidth; i < chunkWidth + cPos.x * chunkWidth; i++)
        {
            float noise1d = 0;
            foreach (var Kemal in noiseSettings)
            {
                if (Kemal.noiseType == NoiseProperties.NoiseType.ONE_DIMENSIONAL)
                {
                    noise1d += (Mathf.PerlinNoise((i + (Kemal.seed * 256) + 0.5f + offset.x) * Kemal.scale, 1) * Kemal.multiplier) / ONE_DIMENSIONAL_LAYER_COUNT;
                }
            }
            result[i - cPos.x * chunkWidth] = new Vector2(i, noise1d);
        }
        return result;
    }

    public float GenerateNoise2D(int x, int y)
    {
        float noise2d = 0;
        foreach (var item in noiseSettings)
        {
            if (item.noiseType == NoiseProperties.NoiseType.TWO_DIMENSIONAL)
            {
                noise2d += (Mathf.PerlinNoise((x + (item.seed * 256) + 0.5f + offset.x) * item.scale,
                (y + (item.seed * 256) + 0.5f + offset.y) * item.scale) * item.multiplier) / TWO_DIMENSIONAL_LAYER_COUNT;
            }
        }
        return noise2d;
    }
    #endregion
    #region WorldController
    //Generate chunk at designated location if not generated
    public void GenerateChunk(Vector2Int cPos)
    {
        float distance = Vector2Int.Distance(cPos, GetChunkPosition());
        if (distance < 3)
        {
            Vector3 TopL = Camera.main.ViewportToWorldPoint(new Vector3Int(0, 1));
            Vector3 BottomR = Camera.main.ViewportToWorldPoint(new Vector3Int(1, 0));
            Vector2Int _TopL = new Vector2Int((int)TopL.x, (int)TopL.y);
            Vector2Int _BottomR = new Vector2Int((int)BottomR.x, (int)BottomR.y);

            Vector2[] noise1d = GenerateNoise1D(cPos);
            loadedChunks.Add(cPos);
            Chunk _chunk = new Chunk();
            Tile[] _tile = new Tile[chunkWidth * chunkWidth];
            for (int x = cPos.x * chunkWidth; x < chunkWidth + cPos.x * chunkWidth; x++)
            {
                for (int y = cPos.y * chunkWidth; y < cPos.y * chunkWidth + chunkWidth; y++)
                {
                    int _x = x - cPos.x * chunkWidth;
                    int _y = y - cPos.y * chunkWidth;
                    if (y <= noise1d[x - cPos.x * chunkWidth].y)
                    {

                        if (GenerateNoise2D(x, y) > 0.2f)
                        {
                            if (y > noise1d[x - cPos.x * chunkWidth].y - 1)
                            {
                                if ((x % 2 == 0 && y % 2 == 0) && x - cPos.x * chunkWidth < y - cPos.y * chunkWidth)
                                {
                                    world.SetTile(new Vector3Int(x, y), tileLib.getTile("default:iron"));
                                    Tile _t = new Tile();
                                    _t.x = x;
                                    _t.y = y;
                                    _t.tilename = "default:iron";
                                    _tile[_x * y] = _t;
                                }
                            }
                            else if (y > noise1d[x - cPos.x * chunkWidth].y - 2)
                            {
                                world.SetTile(new Vector3Int(x, y), tileLib.getTile("default:grass"));
                                Tile _t = new Tile();
                                _t.x = x;
                                _t.y = y;
                                _t.tilename = "default:grass";
                                _tile[_x * y] = _t;
                            }
                            else if (y > noise1d[x - cPos.x * chunkWidth].y - 8)
                            {
                                world.SetTile(new Vector3Int(x, y), tileLib.getTile("default:dirt"));
                                Tile _t = new Tile();
                                _t.x = x;
                                _t.y = y;
                                _t.tilename = "default:dirt";
                                _tile[_x * y] = _t;
                            }
                            else
                            {
                                world.SetTile(new Vector3Int(x, y), tileLib.getTile("default:stone"));
                                Tile _t = new Tile();
                                _t.x = x;
                                _t.y = y;
                                _t.tilename = "default:stone";
                                _tile[_x * _y] = _t;
                            }
                        }

                    }
                }
            }
            _chunk.tiles = _tile;
            _chunk.x = cPos.x;
            _chunk.y = cPos.y;
            dataController.saveData(_chunk);
        }
    }
    // Load chunk from file if generated before
    public void LoadChunk(Vector2Int cPos)
    {
        Vector3 TopL = Camera.main.ViewportToWorldPoint(new Vector3Int(0, 1));
        Vector3 BottomR = Camera.main.ViewportToWorldPoint(new Vector3Int(1, 0));
        Vector2Int _TopL = new Vector2Int((int)TopL.x, (int)TopL.y);
        Vector2Int _BottomR = new Vector2Int((int)BottomR.x, (int)BottomR.y);
        for (int x = cPos.x * chunkWidth; x < chunkWidth + cPos.x * chunkWidth; x++)
        {
            for (int y = cPos.y * chunkWidth; y < cPos.y * chunkWidth + chunkWidth; y++)
            {
                if (y < _TopL.y + 6 && x > _TopL.x - 6)
                {
                    if (y > _BottomR.y - 6 && x < _BottomR.x + 6)
                    {

                    }
                }
            }
        }
    }

    // Save chunk
    public void SaveChunk() { }

    // Unload chunk
    async Task RemoveChunks(List<Vector2Int> cPos)
    {
        int index = 0;
        foreach (var item in cPos)
        {
            for (int x = item.x * chunkWidth; x < chunkWidth + item.x * chunkWidth; x++)
            {
                for (int y = item.y * chunkWidth; y < item.y * chunkWidth + chunkWidth; y++) // noise[x - cPos.x * chunkWidth]
                {
                    //if (world.GetTile(new Vector3Int(x, y)) != null)
                    //{
                    world.SetTile(new Vector3Int(x, y), null);
                    //} 
                }
                index++;
            }
            loadedChunks.Remove(item);
        }
    }
    // Find, save and remove non-visible chunks
    public async Task GarbageCollectorAsync()
    {
        Vector2Int cpos = GetChunkPosition();
        List<Vector2Int> chunksToBeRemoved = new List<Vector2Int>();
        foreach (var item in loadedChunks)
        {
            float distance = Vector2Int.Distance(item, cpos);
            if (distance > 3)
            {
                chunksToBeRemoved.Add(item);
            }
        }
        var _removechunks = RemoveChunks(chunksToBeRemoved);
        await _removechunks;
    }
    #endregion
    #region Decorator
    void PlaceTree(Vector2Int pos)
    {

    }
    void PlaceVegetation(Vector2Int pos)
    {

    }
    #endregion
    #region Utils
    public Vector2Int GetChunkPosition()
    {
        return new Vector2Int((int)Camera.main.transform.position.x / 16, (int)Camera.main.transform.position.y / 16);
        #endregion
    }
}