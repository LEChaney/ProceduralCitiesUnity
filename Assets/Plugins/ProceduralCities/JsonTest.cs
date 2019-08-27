using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var roadNetwork = new RoadNetwork();
        roadNetwork.RoadSegments.Add(new RoadSegment(roadNetwork, 0, 1));
        roadNetwork.RoadSegments.Add(new RoadSegment(roadNetwork, 1, 2));
        roadNetwork.RoadVertices.Add(new RoadVertex(roadNetwork, 0, 0));
        roadNetwork.RoadVertices.Add(new RoadVertex(roadNetwork, 50, 0));
        roadNetwork.RoadVertices.Add(new RoadVertex(roadNetwork, 100, 0));
        Debug.Log(JsonUtility.ToJson(roadNetwork, true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
