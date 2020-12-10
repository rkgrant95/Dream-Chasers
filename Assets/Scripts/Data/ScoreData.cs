using SaveSystem;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class ScoreData 
{
    [XmlAttribute("Name")]
    public string name = "";
    [XmlAttribute("Value")]
    public int value = 0;

    public ScoreData() { }


    public ScoreData(ScoreData _scoreData)
	{
		this.name = _scoreData.name;
		this.value = _scoreData.value;
	}

    public void AddScore(int _score)
    {
        this.value += _score;
    }

    public void ChangeName(string _name)
    {
        this.name = _name;
    }
}

[XmlRoot("ScoreCollection")]
[System.Serializable]
public class ScoreDataContainer
{
    [XmlArray("Scores")]
    [XmlArrayItem("Score")]
    public List<ScoreData> scoreDataList = new List<ScoreData>();

    /// <summary>
    /// Saves score data to an XML and Binary file at the persistent data path of the project
    /// </summary>
    public void Save()
    {
        // Create a new list of score data and populate it
        ScoreDataContainer tempScoreDataContainer = this;

        //The standard file path in unity
        Debug.Log("Your files are located here: " + Application.persistentDataPath);

        //Creates a new FileSave object with the file format XML.
        FileSave fileSave = new FileSave(FileFormat.Xml);

        //Writes an XML file to the path.
        fileSave.WriteToFile(Application.persistentDataPath + "/scoreData.xml", tempScoreDataContainer);

        //Changes the file format to Binary file
        fileSave.fileFormat = FileFormat.Binary;

        //Writes a binary file
        fileSave.WriteToFile(Application.persistentDataPath + "/scoreData.bin", tempScoreDataContainer);

    }

    /// <summary>
    /// Loads a binary data file from the persistent data path of the project
    /// </summary>
    /// <returns></returns>
    public static ScoreDataContainer Load()
    {
        //Creates a new FileSave object with the file format XML.
        FileSave fileSave = new FileSave(FileFormat.Binary);

        //...and loads the data from the Binary file
        ScoreDataContainer retVal = fileSave.ReadFromFile<ScoreDataContainer>(Application.persistentDataPath + "/scoreData.bin");

        if (retVal != null)
            return retVal;
        else
            return new ScoreDataContainer();
    }
}