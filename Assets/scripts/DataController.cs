using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class DataController : MonoBehaviour {
    string data_path;
    FileStream worldFile;
    public List<GeneratorLibrary.Chunk> cache = new List<GeneratorLibrary.Chunk>();
    void Start() {
        Initialize();
        Debug.Log(new Vector4(0, 1, 2, 3).ToString());
    }

    public void Initialize() {
        data_path = path() + "World.dat";
        worldFile = File.Create(data_path);
    }
    public void AddtoCache(GeneratorLibrary.Chunk[] chunk) {
        cache = chunk.Concat(cache.ToArray()).ToList();
    }
    public async Task SaveFromCache() {

    }

    public void SaveData(GeneratorLibrary.Chunk[] chunkStack) {
        BinaryFormatter bf = new BinaryFormatter();
        GeneratorLibrary.Chunk[] result = chunkStack.Concat(GetWorldData().chunks).ToArray();
        GeneratorLibrary.World resultWorld = new GeneratorLibrary.World();
        resultWorld.chunks = result.Distinct().ToArray();
        bf.Serialize(worldFile, resultWorld);
    }
    public void ReadData(Vector2Int chunkPosition) {

    }
    public GeneratorLibrary.World GetWorldData() {
        BinaryFormatter bf = new BinaryFormatter();
        return (GeneratorLibrary.World)bf.Deserialize(worldFile);
    }

    string path() { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "2DRPGSurvival"); }
}
