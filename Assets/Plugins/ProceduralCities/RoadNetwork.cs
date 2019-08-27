using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class RoadNetwork
{
    public List<RoadSegment> RoadSegments { get => roadSegments; }
    public List<RoadVertex> RoadVertices { get => roadVertices; }

    [SerializeField]
    private List<RoadSegment> roadSegments = new List<RoadSegment>();
    [SerializeField]
    private List<RoadVertex> roadVertices = new List<RoadVertex>();
}

[Serializable]
public class RoadVertex
{
    public RoadVertex(RoadNetwork roadNetwork)
    {
        this.roadNetwork = roadNetwork;
    }

    public RoadVertex(RoadNetwork roadNetwork, Vector2 position)
    {
        this.roadNetwork = roadNetwork;
        this.position = position;
    }

    public RoadVertex(RoadNetwork roadNetwork, float x, float y)
        : this(roadNetwork, new Vector2(x, y))
    { }

    // Returns references to connected road segments
    public List<RoadSegment> ConnectedSegments
    {
        get
        {
            List<RoadSegment> roadSegments = new List<RoadSegment>();
            foreach (int roadSegmentIndex in ConnectedSegmentIndices)
            {
                roadSegments.Add(roadNetwork.RoadSegments[roadSegmentIndex]);
            }
            return roadSegments;
        }
    }

    public List<int> ConnectedSegmentIndices { get => connectedSegmentIndices; }

    public Vector2 position;

    private List<int> connectedSegmentIndices = new List<int>();
    private RoadNetwork roadNetwork;
}

[Serializable]
public class RoadSegment
{
    public RoadSegment(RoadNetwork roadNetwork)
    {
        this.roadNetwork = roadNetwork;
    }

    public RoadSegment(RoadNetwork roadNetwork, int startVertIndex, int endVertIndex)
    {
        this.startVertIndex = startVertIndex;
        this.endVertIndex = endVertIndex;
        this.roadNetwork = roadNetwork;
    }

    // Returns a reference to the starting vertex of this road segment
    public RoadVertex StartVertex
    {
        get => roadNetwork.RoadVertices[startVertIndex];
    }

    // Returns a reference to the end vertex of this road segment
    public RoadVertex EndVertex
    {
        get => roadNetwork.RoadVertices[endVertIndex];
    }

    public int startVertIndex = -1;
    public int endVertIndex  = -1;

    private RoadNetwork roadNetwork;
}
