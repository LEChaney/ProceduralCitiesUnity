using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(RoadNetwork))]
public class RoadNetworkEditor : Editor
{
    string jsonOutputPath = "Assets/Resources/out_road_network.json";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoadNetwork roadNetwork = (RoadNetwork)target;
        if (GUILayout.Button("Build Random Road Network"))
        {
            roadNetwork.BuildRandom();
        }

        jsonOutputPath = GUILayout.TextField(jsonOutputPath, 100);
        if (GUILayout.Button("Save to json file"))
        {
            string jsonOutput = roadNetwork.ToJson(true);
            File.WriteAllText(jsonOutputPath, jsonOutput);
        }

        if (GUILayout.Button("Load from json file"))
        {
            roadNetwork.FromJsonOverwrite(roadNetwork.inputJsonFile.text);
        }
    }
}
