using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class RoadNetwork : MonoBehaviour
{
    public List<RoadSegment> RoadSegments { get => roadSegments; }
    public List<RoadVertex> RoadVertices { get => roadVertices; }

    public TextAsset inputJsonFile;

    [SerializeField]
    [HideInInspector]
    private List<RoadSegment> roadSegments = new List<RoadSegment>();

    [SerializeField]
    [HideInInspector]
    private List<RoadVertex> roadVertices = new List<RoadVertex>();

    // Draw simple visualization in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Matrix4x4 transformMat = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = transformMat;
        foreach (RoadSegment roadSegment in roadSegments)
        {
            Gizmos.DrawLine(roadSegment.StartVertex.Position3D, roadSegment.EndVertex.Position3D);
        }
    }

    // Updates all vertices so they have the correct references to their connected road segments
    public void UpdateLinks()
    {
        // Clear existing connected segments from all vertices
        foreach (RoadVertex roadVertex in roadVertices)
        {
            roadVertex.ConnectedSegmentIndices.Clear();
        }

        // Loop over road segments and add backward references from vertices
        for (int i = 0; i < roadSegments.Count; ++i)
        {
            roadSegments[i].StartVertex.ConnectedSegmentIndices.Add(i);
            roadSegments[i].EndVertex.ConnectedSegmentIndices.Add(i);
        }
    }

    public void Clear()
    {
        roadSegments.Clear();
        roadVertices.Clear();
    }
}

[Serializable]
public class RoadVertex
{
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

    public Vector3 Position3D
    {
        get => new Vector3(position.x, 0, position.y);
    }

    public Vector2 position;

    private List<int> connectedSegmentIndices = new List<int>();
    private RoadNetwork roadNetwork;

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
}

[Serializable]
public class RoadSegment
{
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
}
