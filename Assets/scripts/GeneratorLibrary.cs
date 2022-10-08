using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorLibrary : MonoBehaviour
{
    // Noise Controller
    [System.Serializable]
    public class NoiseProperties
    {
        // One directional noise is usually just for Height values, two dimensional ones are for cave generation, ore and material generation.
        public enum NoiseType { ONE_DIMENSIONAL, TWO_DIMENSIONAL };
        public NoiseType noiseType;
        // Speaks for itself
        public Vector2 offset;
        // Speaks for itself... Again
        public long seed;
        // Sets the scale of noise.
        public float scale;
    }

    public NoiseProperties[] noiseSettings;
    public Vector2[] noise1D, noise2D;

    // 
    public Vector2[] GenerateNoise1D()
    {
        // Set a basic One dimensional type noise

        return null;
    }
    // Cave generation or Ore generation
    public Vector2[] GenerateNoise2D()
    {


        return null;
    }




    // World Controller

    //Generate chunk at designated location if not generated
    public void GenerateChunk()
    {

    }

    // Load chunk from file if generated before
    public void LoadChunk()
    {

    }

    // Unload chunk
    public void RemoveChunk()
    {

    }
    // Save chunk
    public void SaveChunk()
    {

    }

}
