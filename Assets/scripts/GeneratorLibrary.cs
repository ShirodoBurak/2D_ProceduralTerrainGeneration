using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    // Noise Controller
    public class NoiseProperties
    {
        public enum NoiseType { ONE_DIMENSIONAL, TWO_DIMENSIONAL };
        public NoiseType noiseType;
        public Vector2 offset;
        public long seed;
        public float scale;
    }

    public NoiseProperties[] noiseSettings;
    public Vector2[] noise1D, noise2D;

    public Vector2[] GenerateNoise1D()
    {
        NoiseProperties noiseitem = new NoiseProperties();
        noiseitem.noiseType = NoiseProperties.NoiseType.ONE_DIMENSIONAL;
        noiseitem.offset = new Vector2(0, 0);
        noiseitem.seed = -4180563108658035;
        noiseitem.scale = 10f;

        return null;
    }

    public Vector2[] GenerateNoise2D()
    {


        return null;
    }




    // World Controller
    public void GenerateChunk()
    {

    }

    public void LoadChunk()
    {

    }

    public void RemoveChunk()
    {

    }

    public void SaveChunk()
    {

    }

}
