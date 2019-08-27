using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SegmentList
{
    public SegmentList()
    {
        roadSegments = new List<Segment>();
    }

    public List<Segment> roadSegments;
}

[Serializable]
public class Segment
{
    public Segment(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }

    public Vector2 start;
    public Vector2 end;
}

public class JsonReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    
    SegmentList roadSegments = new SegmentList();
        roadSegments.roadSegments.Add(new Segment(new Vector2(0, 0), new Vector2(1, 0)));
        roadSegments.roadSegments.Add(new Segment(new Vector2(1, 0), new Vector2(1, 1)));
        Debug.Log(JsonUtility.ToJson(roadSegments));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
