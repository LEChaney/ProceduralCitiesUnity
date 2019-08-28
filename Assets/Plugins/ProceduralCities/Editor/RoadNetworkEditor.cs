using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadNetwork))]
public class RoadNetworkEditor : Editor
{
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
                roadNetwork.RoadVertices.Add(new RoadVertex(roadNetwork, Random.Range(0, 1000), Random.Range(0, 1000)));
            }

            // Add random connections between vertices
            for (int i = 0; i < 1000; ++i)
            {
                roadNetwork.RoadSegments.Add(new RoadSegment(roadNetwork, Random.Range(0, NUM_VERTS), Random.Range(0, NUM_VERTS)));
            }

            roadNetwork.UpdateLinks();
        }
    }
}
