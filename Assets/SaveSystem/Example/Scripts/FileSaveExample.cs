using SaveSystem;
using System.Collections.Generic;
using UnityEngine;

public class FileSaveExample : MonoBehaviour
{
    private void Awake()
    {

        List<MyClass> classList = new List<MyClass>();

        for (int i = 0; i < 5; i++)
        {
            classList.Add(new MyClass(i, new List<int> { i, i + 1, i + 2 }));
        }

        //The standard file path in unity
        Debug.Log("Your files are located here: " + Application.persistentDataPath);

        //Creates a new FileSave object with the file format XML.
        FileSave fileSave = new FileSave(FileFormat.Xml);

        //Writes an XML file to the path.
        //fileSave.WriteToFile(Application.persistentDataPath + "/myFile.xml", new MyClass(87, new List<int> { 1, 2, 3 }));
        fileSave.WriteToFile(Application.persistentDataPath + "/myFile.xml", classList);

        //Changes the file format to Binary file
        fileSave.fileFormat = FileFormat.Binary;

        //Writes a binary file
        fileSave.WriteToFile(Application.persistentDataPath + "/myFile.bin", new MyClass());

        //Changes the file format back to XML...
        fileSave.fileFormat = FileFormat.Xml;

        //...and loads the data from the Xml file
       // MyClass myClass = fileSave.ReadFromFile<MyClass>(Application.persistentDataPath + "/myFile.xml");
       // Debug.Log("Loaded data: " + myClass);

        List<MyClass> myClass = fileSave.ReadFromFile<List<MyClass>>(Application.persistentDataPath + "/myFile.xml");

        for (int i = 0; i < myClass.Count; i++)
        {
            Debug.Log("Loaded data: " + myClass[i]);
        }
        
    }
}

