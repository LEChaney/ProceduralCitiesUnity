using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class RoadNetwork : MonoBehaviour
{
    public TextAsset inputJsonFile;
    public Material roadMaterial;

    private RoadNetworkData data = new RoadNetworkData();

    public int VertCount { get => data.RoadVertices.Count; }
    public int SegmentCount { get => data.RoadSegments.Count; }

    // Returns a conveniance wrapper around the vertex data
    public RoadVertexView GetVertex(int vertIndex)
    {
        return new RoadVertexView(this, data.RoadVertices[vertIndex], vertIndex);
    }

    // Returns a conveniance wrapper around the segment data
    public RoadSegmentView GetSegment(int segmentIndex)
    {
        return new RoadSegmentView(this, data.RoadSegments[segmentIndex], segmentIndex);
    }

    public void AddVertex(RoadVertex newVertex)
    {
        data.RoadVertices.Add(newVertex);
    }

    public void AddSegment(RoadSegment newSegment)
    {
        data.RoadSegments.Add(newSegment);
    }

    // This is run after de-serialization to correctly set backward references from vertices to segments
    public void FixupReferences()
    {
        // Clear existing connected segments from all vertices
        for (int i = 0; i < VertCount; ++i)
        {
            data.RoadVertices[i].ConnectedSegmentIndices.Clear();
        }

        // Loop over road segments and add backward references from vertices
        for (int i = 0; i < SegmentCount; ++i)
        {
            data.RoadVertices[data.RoadSegments[i].StartVertIndex].ConnectedSegmentIndices.Add(i);
            data.RoadVertices[data.RoadSegments[i].EndVertIndex  ].ConnectedSegmentIndices.Add(i);
        }
    }

    public void Clear()
    {
        data.Clear();
    }

    public string ToJson(bool prettyPrint = false)
    {
        return JsonUtility.ToJson(data, prettyPrint);
    }

    // Overwrites the current network with one from json data
    public void FromJsonOverwrite(string json)
    {
        data = JsonUtility.FromJson<RoadNetworkData>(json);
        FixupReferences();
    }

    // Draw simple visualization in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Matrix4x4 transformMat = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = transformMat;
        for (int i = 0; i < SegmentCount; ++i)
        {
            Gizmos.DrawLine(GetSegment(i).StartVertex.Position3D, GetSegment(i).EndVertex.Position3D);
        }
    }

    void Start()
    {
        // De-serialize from json file on play
        FromJsonOverwrite(inputJsonFile.text);

        // BuildRandom();

        // Create line renderers for each segment
        for (int i = 0; i < SegmentCount; ++i)
        {
            Color color = new Color(0, 1.0f, 0);
            CreateLineRenderer(GetSegment(i).StartVertex.Position3D, GetSegment(i).EndVertex.Position3D, color);
        }
    }

    // Clears the current road network and builds a random one in its place
    public void BuildRandom()
    {
        Clear();

        // Add random road vertices
        const int NUM_VERTS = 10000;
        for (int i = 0; i < NUM_VERTS; ++i)
        {
            AddVertex(new RoadVertex(Random.Range(0, 1000), Random.Range(0, 1000)));
        }

        // Add random connections between vertices
        for (int i = 0; i < 1000; ++i)
        {
            AddSegment(new RoadSegment(Random.Range(0, NUM_VERTS), Random.Range(0, NUM_VERTS)));
        }

        FixupReferences();
    }

    private void CreateLineRenderer(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject("RoadSegmentRenderer");
        myLine.transform.position = start;
        myLine.transform.Rotate(new Vector3(90, 0, 0));
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.alignment = LineAlignment.TransformZ;

        lr.textureMode = LineTextureMode.Tile;
        lr.numCapVertices = 10;
        lr.material = roadMaterial;
        lr.shadowBias = 0;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 7;
        lr.endWidth = 7;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}

[Serializable]
public class RoadNetworkData
{
    public List<RoadSegment> RoadSegments => roadSegments;
    public List<RoadVertex> RoadVertices => roadVertices;

    [SerializeField]
    private List<RoadSegment> roadSegments = new List<RoadSegment>();

    [SerializeField]
    private List<RoadVertex> roadVertices = new List<RoadVertex>();

    public void Clear()
    {
        roadSegments.Clear();
        roadVertices.Clear();
    }
}

// Conveniance wrapper class around road segment data
// Allows easy direct access to connected road segments
public class RoadVertexView
{
    // Returns references to connected road segments
    public List<RoadSegmentView> ConnectedSegments
    {
        get
        {
            var roadSegmentViews = new List<RoadSegmentView>();
            foreach (int segmentIndex in Data.ConnectedSegmentIndices)
            {
                roadSegmentViews.Add(roadNetwork.GetSegment(segmentIndex));
            }
            return roadSegmentViews;
        }
    }

    public Vector3 Position3D
    {
        get => new Vector3((Data.Position.x - 500) * 10, 0, (Data.Position.y - 500) * 10);
    }

    public RoadVertex Data { get; }

    // Index in road network graph
    public int Index { get; }

    private readonly RoadNetwork roadNetwork;

    public RoadVertexView(RoadNetwork roadNetwork, RoadVertex vertexData, int vertIndex)
    {
        this.roadNetwork = roadNetwork;
        this.Data = vertexData;
        this.Index = vertIndex;
    }
}

[Serializable]
public class RoadVertex
{
    public List<int> ConnectedSegmentIndices => connectedSegmentIndices;
    private List<int> connectedSegmentIndices = new List<int>();

    [SerializeField]
    private Vector2 position;
    public Vector2 Position => position;

    public RoadVertex()
        : this(new Vector2(0, 0))
    { }

    public RoadVertex(Vector2 position, List<int> connectedSegmentIndices = null)
    {
        this.position = position;
        if (connectedSegmentIndices != null)
            this.connectedSegmentIndices = connectedSegmentIndices;
    }

    public RoadVertex(float x, float y, List<int> connectedSegmentIndices = null)
        : this(new Vector2(x, y), connectedSegmentIndices)
    { }
}

// Conveniance wrapper class around vertex data
// Allows easy direct access to connected vertices
public class RoadSegmentView
{
    // Returns a reference to the starting vertex of this road segment
    public RoadVertexView StartVertex
    {
        get => roadNetwork.GetVertex(Data.StartVertIndex);
    }

    // Returns a reference to the end vertex of this road segment
    public RoadVertexView EndVertex
    {
        get => roadNetwork.GetVertex(Data.EndVertIndex);
    }

    public RoadSegment Data { get; }

    // Index in road network graph
    public int Index { get; } 

    private readonly RoadNetwork roadNetwork;

    public RoadSegmentView(RoadNetwork roadNetwork, RoadSegment segmentData, int segmentIndex)
    {
        this.roadNetwork = roadNetwork;
        this.Data = segmentData;
        this.Index = segmentIndex;
    }
}

[Serializable]
public class RoadSegment
{
    [SerializeField]
    private int startVertIndex;
    public int StartVertIndex => startVertIndex;

    [SerializeField]
    private int endVertIndex;
    public int EndVertIndex => endVertIndex;

    public RoadSegment()
        : this(-1, -1)
    { }

    public RoadSegment(int startVertIndex, int endVertIndex)
    {
        this.startVertIndex = startVertIndex;
        this.endVertIndex = endVertIndex;
    }
}
