using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public void parseData()
    {
        FileStream file = null;
        BinaryFormatter bf = new BinaryFormatter();

    }
    public void saveData(GeneratorLibrary.Chunk chunk)
    {
        CreateFile(chunk);
    }
    void CreateFile(GeneratorLibrary.Chunk chunk)
    {
        if (!Directory.Exists(path()))
        {
            Directory.CreateDirectory(path());
        }
        string regionpath = path() + "/" + chunk.x/16 + "-" + chunk.y/16 + "-Chunk.dat";
        if (!File.Exists(regionpath)) 
        {
            File.Create(regionpath);
        }
    }
    void RemoveFile()
    {

    }

    void ReadFile()
    {
    
    }
    string path() { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "2DRPGSurvival");}
}
