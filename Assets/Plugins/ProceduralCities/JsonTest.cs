using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var roadNetwork = new RoadNetwork();
        roadNetwork.AddVertex(new RoadVertex(0, 0));
        roadNetwork.AddVertex(new RoadVertex(50, 0));
        roadNetwork.AddVertex(new RoadVertex(100, 0));

        roadNetwork.AddSegment(new RoadSegment(0, 1));
        roadNetwork.AddSegment(new RoadSegment(1, 2));

        Debug.Log(roadNetwork.ToJson(true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
