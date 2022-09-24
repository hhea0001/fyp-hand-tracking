//using Newtonsoft.Json;
//using System;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//[Serializable]
//public struct OldImageData
//{
//    public string filename;
//    public HandAction action;
//    public float x;
//    public float y;
//    public float z;

//    [MenuItem("FYP/Update Old Data")]
//    public static void UpdateData()
//    {
//        string filename = EditorUtility.OpenFilePanel("Select Old Data File", "", "json");
//        string fileString = File.ReadAllText(filename);
//        File.WriteAllText(filename.Replace(".json", "_backup.json"), fileString);

//		OldImageData[] oldData = JsonConvert.DeserializeObject<OldImageData[]>(fileString);

//        ImageData[] data = new ImageData[oldData.Length];

//        HandData previousHandData = new HandData
//        {
//            action = oldData[0].action,
//            x = oldData[0].x,
//            y = oldData[0].y,
//            z = oldData[0].z,
//        };

//        for (int i = 0; i < oldData.Length; i++)
//        {
//            data[i] = new ImageData
//            {
//                filename = oldData[i].filename,
//                current = new HandData
//                {
//                    action = oldData[i].action,
//                    x = oldData[i].x,
//                    y = oldData[i].y,
//                    z = oldData[i].z,
//                },
//                previous = previousHandData,
//            };

//            previousHandData = data[i].current;
//        }

//        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
//        File.WriteAllText(filename, jsonData);
//        Debug.Log($"Saved data to {filename}");
//    }
//}