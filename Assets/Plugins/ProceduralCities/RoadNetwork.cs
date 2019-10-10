using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Random = UnityEngine.Random;

[Serializable]
public class RoadNetwork : MonoBehaviour
{
    public TextAsset inputJsonFile;
    public Material roadMaterial;
    public float inputWidth = 872;
    public float inputHeight = 843;

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
    public void RebuildAdjacencies()
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

        // Sort all segment links in clockwise order
        for (int i = 0; i < VertCount; ++i)
        {
            var roadVert = GetVertex(i);
            List<int> connSegmentIndices = roadVert.Data.ConnectedSegmentIndices; // Need to operate on index data (this is actually what is stored)
            connSegmentIndices.Sort(delegate (int conn1Idx, int conn2Idx)
            {
                RoadSegmentView conn1 = GetSegment(conn1Idx);
                RoadSegmentView conn2 = GetSegment(conn2Idx);
                int adjVertIdx1 = conn1.StartVertex.Index != i ? conn1.StartVertex.Index : conn1.EndVertex.Index;
                int adjVertIdx2 = conn2.StartVertex.Index != i ? conn2.StartVertex.Index : conn2.EndVertex.Index;
                RoadVertexView adjVert1 = GetVertex(adjVertIdx1);
                RoadVertexView adjVert2 = GetVertex(adjVertIdx2);
                Vector2 vecToAdjVert1 = adjVert1.Position - roadVert.Position;
                Vector2 VecToAdjVert2 = adjVert2.Position - roadVert.Position;
                float theta1 = Mathf.PI - Mathf.Atan2(vecToAdjVert1.y, -vecToAdjVert1.x); // [0, 2pi)
                float theta2 = Mathf.PI - Mathf.Atan2(VecToAdjVert2.y, -VecToAdjVert2.x); // [0, 2pi)
                if (theta1 < theta2)
                    return 1;
                else if (theta1 > theta2)
                    return -1;
                else
                    return 0;
            });
        }
    }

    public void Clear()
    {
        data.Clear();
    }

    public string ToJson(bool prettyPrint = false)
    {
        // TODO: Remove this hacky fix for incoming scale / position
        // move scaling / position correction to python output code
        foreach (RoadVertex roadVert in data.RoadVertices)
        {
            roadVert.position /= 10;
        }

        string jsonStr = JsonUtility.ToJson(data, prettyPrint);

        // TODO: Remove this hacky fix for incoming scale / position
        // move scaling / position correction to python output code
        foreach (RoadVertex roadVert in data.RoadVertices)
        {
            roadVert.position *= 10;
        }

        return jsonStr;
    }

    // Overwrites the current network with one from json data
    public void FromJsonOverwrite(string json)
    {
        data = JsonUtility.FromJson<RoadNetworkData>(json);

        // TODO: Remove this hacky fix for incoming scale / position
        // move scaling / position correction to python output code
        foreach (RoadVertex roadVert in data.RoadVertices)
        {
            roadVert.position.x *= Terrain.activeTerrain.terrainData.size.x / inputWidth;
            roadVert.position.y = Terrain.activeTerrain.terrainData.size.z - roadVert.position.y * Terrain.activeTerrain.terrainData.size.z / inputHeight; // Flip y-axis
        }

        RebuildAdjacencies();
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
        //FromJsonOverwrite(inputJsonFile.text);

        // BuildRandom();

        // Create line renderers for each segment
        //for (int i = 0; i < SegmentCount; ++i)
        //{
        //    Color color = new Color(0, 1.0f, 0);
        //    Vector3 offset = new Vector3(0, -0.1f, 0);
        //    CreateLineRenderer(GetSegment(i).StartVertex.Position3D + offset, GetSegment(i).EndVertex.Position3D + offset, color);
        //}

        //ProBuilderMesh roadMesh = Mesher.MeshRoadNetwork(this);
        //roadMesh.GetComponent<MeshRenderer>().sharedMaterial = this.roadMaterial;
    }

    // Clears the current road network and builds a random one in its place
    public void BuildRandom()
    {
        Clear();

        // Add random road vertices
        const int NUM_VERTS = 10000;
        for (int i = 0; i < NUM_VERTS; ++i)
        {
            AddVertex(new RoadVertex(Random.Range(0, Terrain.activeTerrain.terrainData.size.x), Random.Range(0, Terrain.activeTerrain.terrainData.size.z)));
        }

        // Add random connections between vertices
        for (int i = 0; i < 1000; ++i)
        {
            AddSegment(new RoadSegment(Random.Range(0, NUM_VERTS), Random.Range(0, NUM_VERTS)));
        }

        RebuildAdjacencies();
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
        get => new Vector3(Data.position.x, 0, Data.position.y);
    }

    public Vector2 Position
    {
        get => Data.position;
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

    public Vector2 position;

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

    public float HalfWidth
    {
        get => Data.HalfWidth;
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

    [SerializeField]
    private float halfWidth;
    public float HalfWidth => halfWidth;

    public RoadSegment()
        : this(-1, -1)
    { }

    public RoadSegment(int startVertIndex, int endVertIndex, float halfWidth=3.5f)
    {
        this.startVertIndex = startVertIndex;
        this.endVertIndex = endVertIndex;
        this.halfWidth = halfWidth;
    }
}
