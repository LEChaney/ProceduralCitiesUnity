﻿using System.Collections;
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
            roadNetwork.Clear();

            // Add random road vertices
            const int NUM_VERTS = 10000;
            for (int i = 0; i < NUM_VERTS; ++i)
            {
                roadNetwork.AddVertex(new RoadVertex(Random.Range(0, 1000), Random.Range(0, 1000)));
            }

            // Add random connections between vertices
            for (int i = 0; i < 1000; ++i)
            {
                roadNetwork.AddSegment(new RoadSegment(Random.Range(0, NUM_VERTS), Random.Range(0, NUM_VERTS)));
            }

            roadNetwork.FixupReferences();
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
