using SaveSystem;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class KillData 
{
    [XmlAttribute("Enemy Name")]
    public string enemyName = "";
    [XmlAttribute("Current Value")]
    public int currentValue = 0;
    [XmlAttribute("Total Value")]
    public int totalValue = 0;
    [XmlAttribute("EXP Value")]
    public int expValue = 0;

    public KillData() { }


    public KillData(KillData _killData)
	{
		this.enemyName = _killData.enemyName;
		this.currentValue = _killData.currentValue;
        this.totalValue = _killData.totalValue;
        this.expValue = _killData.expValue;
    }

    public void AddScore(int _value)
    {
        this.currentValue += _value;
    }

    public void ChangeName(string _name)
    {
        this.enemyName = _name;
    }
}

[XmlRoot("KillCollection")]
[System.Serializable]
public class KillDataContainer
{
    [XmlArray("Kills")]
    [XmlArrayItem("Kill")]
    public List<KillData> killDataList = new List<KillData>();

    /// <summary>
    /// Saves score data to an XML and Binary file at the persistent data path of the project
    /// </summary>
    public void Save()
    {
        // Create a new list of score data and populate it
        KillDataContainer tempDataContainer = this;

        //The standard file path in unity
        Debug.Log("Your files are located here: " + Application.persistentDataPath);

        //Creates a new FileSave object with the file format XML.
        FileSave fileSave = new FileSave(FileFormat.Xml);

        //Writes an XML file to the path.
        fileSave.WriteToFile(Application.persistentDataPath + "/killData.xml", tempDataContainer);

        //Changes the file format to Binary file
        fileSave.fileFormat = FileFormat.Binary;

        //Writes a binary file
        fileSave.WriteToFile(Application.persistentDataPath + "/killData.bin", tempDataContainer);

    }

    /// <summary>
    /// Loads a binary data file from the persistent data path of the project
    /// </summary>
    /// <returns></returns>
    public static KillDataContainer Load()
    {
        //Creates a new FileSave object with the file format XML.
        FileSave fileSave = new FileSave(FileFormat.Binary);

        //...and loads the data from the Binary file
        KillDataContainer retVal = fileSave.ReadFromFile<KillDataContainer>(Application.persistentDataPath + "/killData.bin");

        if (retVal != null)
            return retVal;
        else
            return new KillDataContainer();
    }
}