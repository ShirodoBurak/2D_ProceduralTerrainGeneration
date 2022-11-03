using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorLibrary : MonoBehaviour {
    #region Structs and Classes
    [Serializable]
    public class NoiseProperties {
        // One directional noise is usually just for Height values, two dimensional ones are for cave generation, ore and material generation.
        public enum NoiseType { ONE_DIMENSIONAL, TWO_DIMENSIONAL };
        public NoiseType noiseType;
        public long seed;
        // Sets the scale of noise.
        public float scale;
        public float multiplier;
        [Range(0.1f,1f)]
        public float weight = 1;
    }
    public struct Tree {
        public Dictionary<Vector2Int, string> location;
        // < Coordinates, "TileName" >
    }
    [Serializable]
    public class Tile {
        public int x { get; set; }
        public int y { get; set; }
        public string tilename { get; set; }
    }
    [Serializable]
    public class Chunk {
        public int x { get; set; }
        public int y { get; set; }
        public Tile[] tiles { get; set; }
    }
    [Serializable]
    public class World {
        public Chunk[] chunks;
    }
    #endregion
    #region Variables
    public Vector2Int _Region;
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
    void Start() {
        //StartCoroutine(AutoSave());
        int oneDimCount = 0;
        int twoDimCount = 0;

        foreach(var item in noiseSettings) {
            if(item.noiseType == NoiseProperties.NoiseType.ONE_DIMENSIONAL) {
                oneDimCount++;
            } else {
                twoDimCount++;
            }
        }
        ONE_DIMENSIONAL_LAYER_COUNT = oneDimCount;
        TWO_DIMENSIONAL_LAYER_COUNT = twoDimCount;
    }
    #region Tree and Vegetation Data

    void TreeInit() {
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
    public Vector2[] GenerateNoise1D(Vector2Int cPos) {
        Vector2[] result = new Vector2[chunkWidth];
        for(int i = cPos.x * chunkWidth;i < chunkWidth + cPos.x * chunkWidth;i++) {
            float noise1d = 0;
            foreach(var Kemal in noiseSettings) {
                if(Kemal.noiseType == NoiseProperties.NoiseType.ONE_DIMENSIONAL) {
                    noise1d += ( Mathf.PerlinNoise(( i + ( Kemal.seed * 256 ) + 0.5f + offset.x ) * Kemal.scale, 1) * Kemal.multiplier )*Kemal.weight / ONE_DIMENSIONAL_LAYER_COUNT;
                }
            }
            result[i - cPos.x * chunkWidth] = new Vector2(i, noise1d);
        }
        return result;
    }
    public float GenerateNoise2D(int x, int y) {
        float noise2d = 0;
        foreach(var item in noiseSettings) {
            if(item.noiseType == NoiseProperties.NoiseType.TWO_DIMENSIONAL) {
                noise2d += ( Mathf.PerlinNoise(( x + ( item.seed * 256 ) + 0.5f + offset.x ) * item.scale,
                ( y + ( item.seed * 256 ) + 0.5f + offset.y ) * item.scale) * item.multiplier ) * item.weight / TWO_DIMENSIONAL_LAYER_COUNT;
            }
        }
        return noise2d;
    }
    public Dictionary<Vector2Int, Chunk[]> chunksToBeSaved = new Dictionary<Vector2Int, Chunk[]>();
    #endregion
    #region WorldController

    int chunkSaveAmount = 0;
    //Generate chunk at designated location if not generated
    public void GenerateChunk(Vector2Int cPos) {
        float distance = Vector2Int.Distance(cPos, GetChunkPosition());
        Vector2[] noise1d = GenerateNoise1D(cPos);
        loadedChunks.Add(cPos);
        Chunk _chunk = new Chunk();
        Tile[] _tile = new Tile[chunkWidth * chunkWidth];
        for(int x = cPos.x * chunkWidth;x < chunkWidth + cPos.x * chunkWidth;x++) {
            for(int y = cPos.y * chunkWidth;y < cPos.y * chunkWidth + chunkWidth;y++) {
                int _x = x - cPos.x * chunkWidth;
                int _y = y - cPos.y * chunkWidth;
                if(y <= noise1d[x - cPos.x * chunkWidth].y) {
                    if(GenerateNoise2D(x, y) > 0.2f) {
                        Tile _t = new Tile();
                        if(y > noise1d[x - cPos.x * chunkWidth].y - 1) {
                            if(( x % 2 == 0 && y % 2 == 0 ) && x - cPos.x * chunkWidth < y - cPos.y * chunkWidth) {

                                _t.x = x;
                                _t.y = y;
                                _t.tilename = "default:iron";
                                _tile[_x * _y] = _t;
                                world.SetTile(new Vector3Int(x, y, 0), tileLib.getTile("default:iron"));
                            }
                        } else if(y > noise1d[x - cPos.x * chunkWidth].y - 2) {
                            _t.x = x;
                            _t.y = y;
                            _t.tilename = "default:grass";
                            _tile[_x * _y] = _t;
                            world.SetTile(new Vector3Int(x, y, 0), tileLib.getTile("default:grass"));

                        } else if(y > noise1d[x - cPos.x * chunkWidth].y - 8) {
                            _t.x = x;
                            _t.y = y;
                            _t.tilename = "default:dirt";
                            _tile[_x * _y] = _t;
                            world.SetTile(new Vector3Int(x, y, 0), tileLib.getTile("default:dirt"));

                        } else {
                            _t.x = x;
                            _t.y = y;
                            _t.tilename = "default:stone";
                            _tile[_x * _y] = _t;
                            world.SetTile(new Vector3Int(x, y, 0), tileLib.getTile("default:stone"));

                        }
                    }
                }
            }
        }
        _chunk.tiles = _tile;
        _chunk.x = cPos.x;
        _chunk.y = cPos.y;
        Chunk[] chunkArray = { _chunk };
        Vector2Int rPos = new Vector2Int(cPos.x / 16, cPos.y / 16);
        if(!chunksToBeSaved.ContainsKey(rPos)) {
            chunksToBeSaved.Add(rPos, chunkArray.Distinct().ToArray());
            chunkSaveAmount++;
        } else {
            chunksToBeSaved.TryGetValue(rPos, out Chunk[] ExistingChunkValue);
            chunkArray = chunkArray.Concat(ExistingChunkValue).ToArray();
            chunksToBeSaved.Remove(rPos);
            chunksToBeSaved.Add(rPos, chunkArray.Distinct().ToArray());
            chunkSaveAmount++;
        }
    }
    public void LoadChunk(Vector2Int cPos) {
        Vector3 TopL = Camera.main.ViewportToWorldPoint(new Vector3Int(0, 1));
        Vector3 BottomR = Camera.main.ViewportToWorldPoint(new Vector3Int(1, 0));
        Vector2Int _TopL = new Vector2Int((int)TopL.x, (int)TopL.y);
        Vector2Int _BottomR = new Vector2Int((int)BottomR.x, (int)BottomR.y);


        // To be implemented...
    }

    // Unload chunk
    async Task RemoveChunks(List<Vector2Int> cPos) {
        foreach(var item in cPos) {
            for(int x = item.x * chunkWidth;x < chunkWidth + item.x * chunkWidth;x++) {
                for(int y = item.y * chunkWidth;y < item.y * chunkWidth + chunkWidth;y++) // noise[x - cPos.x * chunkWidth]
                {
                    world.SetTile(new Vector3Int(x, y), null);
                }
            }
            loadedChunks.Remove(item);
        }
    }
    // Find, save and remove non-visible chunks
    public async Task GarbageCollectorAsync() {
        Vector2Int cpos = GetChunkPosition();
        List<Vector2Int> chunksToBeRemoved = new List<Vector2Int>();
        foreach(var item in loadedChunks) {
            float distance = Vector2Int.Distance(item, cpos);
            if(distance > 3) {
                chunksToBeRemoved.Add(item);
            }
        }
        var _removechunks = RemoveChunks(chunksToBeRemoved);
        await _removechunks;
    }
    #endregion
    #region Decorator
    void PlaceTree(Vector2Int pos) { }
    void PlaceVegetation(Vector2Int pos) { }
    #endregion
    #region Utils
    public Vector2Int GetChunkPosition() {
        return new Vector2Int((int)Camera.main.transform.position.x / 16, (int)Camera.main.transform.position.y / 16);
        #endregion
    }
}