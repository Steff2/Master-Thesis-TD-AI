using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileWriter
{
    protected string storedData;
    public string Filename { get; private set; }
    public string FileExtension { get; private set;}
    public string FullPath { get { return "f:\\Master Thesis\\Master-Thesis-TD-AI\\Unity Template TD\\Telemetry\\" + Filename + "." + FileExtension; } }

    public FileWriter(string filename, string fileExtension)
    {
        Filename = filename;
        FileExtension = fileExtension;
    }

    //Generate streamwriter function
    public void WriteString()
    {
        StreamWriter writer = new StreamWriter(FullPath, true);
        writer.Write(storedData);
        writer.Close();
    }

    //Generate streamreader function
    public string ReadString()
    {
        StreamReader reader = new StreamReader(FullPath);
        string data = reader.ReadToEnd();
        reader.Close();
        return data;
    }

    public void SetStoredData(string data)
    {
        storedData += data + "\n";
    }
}
