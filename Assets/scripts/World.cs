using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class World : MonoBehaviour
{
    public GeneratorLibrary genLib;
    Vector2Int cpos;

    private void Start()
    {
        genLib.world.ClearAllTiles();
        _ = GarbageCollectorThread();
    }

    void Update()
    {
        if (cpos != GetChunkPosition())
        {
            StartCoroutine(GenerateMultipleChunks());
            cpos = GetChunkPosition();
        }
    }

    IEnumerator GenerateMultipleChunks()
    {
        Vector2Int visiblechunk = getVisibleChunkAmount();

        for (int x = 0 - visiblechunk.x / 2; x < visiblechunk.x / 2; x++)
        {
            for (int y = 0 - visiblechunk.y / 2; y < visiblechunk.y / 2; y++)
            {
                genLib.GenerateChunk(new Vector2Int(cpos.x / 16 + x, cpos.y / 16 + y));
                yield return new WaitForSecondsRealtime(0.0001f);
            }
        }
    }

    Task collector;
    async Task GarbageCollectorThread()
    {
        while (true)
        {
            await Task.Delay(1);
            collector = genLib.GarbageCollectorAsync();
            if (collector.Status == TaskStatus.RanToCompletion)
            {
                await collector;
            }
            else if (collector.Status != TaskStatus.Running)
            {
                await collector;
            }
        }
    }



    #region Utils
    public Vector2Int GetChunkPosition() { return new Vector2Int((int)Camera.main.transform.position.x, (int)Camera.main.transform.position.y); }
    public Vector2Int getVisibleChunkAmount()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        return new Vector2Int((int)width * 2 / 16, (int)height * 3 / 16);
    }
    #endregion
}
