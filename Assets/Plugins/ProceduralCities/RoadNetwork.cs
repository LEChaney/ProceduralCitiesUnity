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
            roadVertex.roadNetwork = this;
            roadVertex.ConnectedSegmentIndices.Clear();
        }

        // Loop over road segments and add backward references from vertices
        for (int i = 0; i < roadSegments.Count; ++i)
        {
            roadSegments[i].roadNetwork = this;
            roadSegments[i].StartVertex.ConnectedSegmentIndices.Add(i);
            roadSegments[i].EndVertex.ConnectedSegmentIndices.Add(i);
        }
    }

    void Start()
    {
        JsonUtility.FromJsonOverwrite(inputJsonFile.text, this);
        UpdateLinks();

        foreach (RoadSegment roadSegment in roadSegments)
        {
            Color color = new Color(0, 1.0f, 0);
            DrawLine(roadSegment.StartVertex.Position3D, roadSegment.EndVertex.Position3D, color);
        }
    }

    public void Clear()
    {
        roadSegments.Clear();
        roadVertices.Clear();
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject("RoadSegmentRenderer");
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Lightweight Render Pipeline/Particles/Unlit"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
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
        get => new Vector3(position.x - 500, 0, position.y - 500);
    }

    public Vector2 position;

    private List<int> connectedSegmentIndices;

    public RoadNetwork roadNetwork;

    public RoadVertex()
    {
        connectedSegmentIndices = new List<int>();
    }

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

    public RoadNetwork roadNetwork;

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
