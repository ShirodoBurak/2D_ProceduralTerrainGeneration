using System.Collections;
using System.Collections.Generic;
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
    #endregion



    #region Variables
    Dictionary<string, Tree> treeVariations;
    public TileHolder tileLib;
    public int chunkWidth = 16;
    public Vector2Int offset;
    public NoiseProperties[] noiseSettings;
    public Tilemap world;
    public TileBase tile;
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
        // Set a basic One dimensional type noise
        Vector2[] result = new Vector2[chunkWidth];
        for (int i = cPos.x * chunkWidth; i < chunkWidth + cPos.x * chunkWidth; i++)
        {
            float noise1d = 0;
            foreach (var item in noiseSettings)
            {
                if (item.noiseType == NoiseProperties.NoiseType.ONE_DIMENSIONAL)
                {
                    noise1d += (Mathf.PerlinNoise((i + (item.seed * 256) + 0.5f + offset.x) * item.scale, 1) * item.multiplier) / ONE_DIMENSIONAL_LAYER_COUNT;
                }
            }
            result[i - cPos.x * chunkWidth] = new Vector2(i, noise1d);
        }
        return result;
    }


    // Cave generation or Ore generation
    /*public Vector3[] GenerateNoise2D(Vector2Int cPos)
    {
        Vector3[] result = new Vector3[chunkWidth * chunkWidth];

        for (int i = cPos.x * chunkWidth; i < chunkWidth + cPos.x * chunkWidth; i++)
        {
            for (int j = cPos.y * chunkWidth; j < cPos.y*chunkWidth+chunkWidth; j++)
            {
                float noise2d = 0;
                foreach (var item in noiseSettings)
                {
                    if (item.noiseType == NoiseProperties.NoiseType.TWO_DIMENSIONAL)
                    {
                        noise2d += (Mathf.PerlinNoise((i + (item.seed * 256) + 0.5f + offset.x) * item.scale,
                        (j + (item.seed * 256) + 0.5f + offset.y) * item.scale) * item.multiplier) / TWO_DIMENSIONAL_LAYER_COUNT;
                    }
                }
                result[(i - cPos.x * chunkWidth) * (j - cPos.y * chunkWidth)] = new Vector3(i, j, noise2d);
            }
        }
        return result;
    }*/

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
            Vector2[] noise1d = GenerateNoise1D(cPos);
            //Vector3[] noise2d = GenerateNoise2D(cPos);
            loadedChunks.Add(cPos);
            for (int x = cPos.x * chunkWidth; x < chunkWidth + cPos.x * chunkWidth; x++)
            {
                for (int y = cPos.y * chunkWidth; y < cPos.y * chunkWidth + chunkWidth; y++) // noise[x - cPos.x * chunkWidth]
                {
                    if (y <= noise1d[x - cPos.x * chunkWidth].y)
                    {
                        if (GenerateNoise2D(x,y) > 0.2f)
                        {
                            if (y > noise1d[x - cPos.x * chunkWidth].y - 1)
                            {
                                if ((x % 2 == 0 && y % 2 == 0) && x - cPos.x * chunkWidth < y - cPos.y * chunkWidth)
                                {
                                    world.SetTile(new Vector3Int(x, y), tileLib.iron);
                                }
                            }
                            else if (y > noise1d[x - cPos.x * chunkWidth].y - 2)
                            {
                                world.SetTile(new Vector3Int(x, y), tileLib.grass);
                            }
                            else if (y > noise1d[x - cPos.x * chunkWidth].y - 8)
                            {
                                world.SetTile(new Vector3Int(x, y), tileLib.dirt);
                            }
                            else
                            {
                                world.SetTile(new Vector3Int(x, y), tileLib.stone);
                            }
                        }
                    }
                }
            }
        }
    }
    // Load chunk from file if generated before
    public void LoadChunk() { }
    // Save chunk
    public void SaveChunk() { }
    // Unload chunk
    bool removeChunksRunning = false;
    void RemoveChunks(List<Vector2Int> cPos)
    {
        removeChunksRunning = true;
        foreach (var item in cPos)
        {
            for (int x = item.x * chunkWidth; x < chunkWidth + item.x * chunkWidth; x++)
            {
                for (int y = item.y * chunkWidth; y < item.y * chunkWidth + chunkWidth; y++) // noise[x - cPos.x * chunkWidth]
                {
                    world.SetTile(new Vector3Int(x, y), null);
                }
            }
            loadedChunks.Remove(item);
        }
        removeChunksRunning = false;
    }
    // Find, save and remove non-visible chunks
    public void GarbageCollector()
    {
        if (!removeChunksRunning)
        {
            Vector2Int cpos = GetChunkPosition();
            List<Vector2Int> chunksToBeRemoved = new List<Vector2Int>();
            foreach (var item in loadedChunks)
            {
                float distance = Vector2Int.Distance(item, cpos);
                if (distance > 5)
                {
                    chunksToBeRemoved.Add(item);
                }
            }
            RemoveChunks(chunksToBeRemoved);
        }
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
    }
    #endregion
}