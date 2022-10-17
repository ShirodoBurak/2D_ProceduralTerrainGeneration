using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GeneratorLibrary genLib;
    Vector2Int cpos;

    private void Start()
    {
        StartCoroutine(GarbageCollectorThread());
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

        for (int x = 0 - visiblechunk.x/2; x < visiblechunk.x/2; x++)
        {
            for (int y = 0 - visiblechunk.y / 2; y < visiblechunk.y/2; y++)
            {
                genLib.GenerateChunk(new Vector2Int(cpos.x + x, cpos.y + y));
                yield return new WaitForSecondsRealtime(0.0001f);
            }
        }
    }

    
    IEnumerator GarbageCollectorThread()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            genLib.GarbageCollector();
        }
    }



    #region Utils
    public Vector2Int GetChunkPosition() { return new Vector2Int((int)Camera.main.transform.position.x / 16, (int)Camera.main.transform.position.y / 16); }
    public Vector2Int getVisibleChunkAmount(){
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        return new Vector2Int((int)width*2 / 16, (int)height*3 / 16);
    }
    #endregion
}
