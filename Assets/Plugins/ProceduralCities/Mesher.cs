using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Mesher
{
    //private struct MeshData
    //{
    //    List<Vector3> meshVerts;
    //    List<int> connectedSegIndices;
    //}

    //// For each road segment we have two vertices associated with each endpoint
    //private struct RoadSegmentMeshInfo
    //{
    //    public RoadMeshEndVerts startVertIndices;
    //    public RoadMeshEndVerts endVertIndices;
    //}

    private struct RoadSegmentEdges
    {
        public Line left;
        public Line right;
    }

    private struct Line
    {
        public Vector2 start;
        public Vector2 end;
    }

    //// From the point of view of entering the road vertex
    //private struct RoadMeshEndVerts
    //{
    //    int leftMeshVertIdx;
    //    int rightMeshVertIdx;
    //}

    public static ProBuilderMesh MeshRoadNetwork(RoadNetwork roadNet)
    {
        var (vertices, faces) = CalcVerticesAndFaces(roadNet);
        ProBuilderMesh roadMesh = ProBuilderMesh.Create(vertices, faces);

        return roadMesh;
    }

    private static (List<Vector3>, List<Face>) CalcVerticesAndFaces(RoadNetwork roadNet)
    {
        var vertices = new List<Vector3>();
        var faces = new List<Face>();

        for (int i = 0; i < roadNet.VertCount; ++i)
        {
            // Construct incoming road edge descriptors
            RoadVertexView roadVert = roadNet.GetVertex(i);
            List<RoadSegmentView> connSegments = roadVert.ConnectedSegments; // Assumed to be sorted in clockwise order
            var incRoadEdges = new RoadSegmentEdges[connSegments.Count];
            for (int j = 0; j < connSegments.Count; ++j)
            {
                // Get adjacent vertex on this road segment
                int adjVertIdx = connSegments[j].StartVertex.Index != i ? connSegments[j].StartVertex.Index : connSegments[j].EndVertex.Index;
                RoadVertexView adjRoadVert = roadNet.GetVertex(adjVertIdx);

                // Construct the incoming left and right road edge descriptors
                Vector2 incDir = (roadVert.Position - adjRoadVert.Position).normalized;
                Vector2 rot90Dir = new Vector2(-incDir.y, incDir.x); // Rotated 90 degrees counter clockwise
                Vector2 leftOffset = rot90Dir * connSegments[j].HalfWidth;
                Vector2 rightOffset = -rot90Dir * connSegments[j].HalfWidth;
                RoadSegmentEdges roadSegmentEdges = new RoadSegmentEdges()
                {
                    left = new Line()
                    {
                        start = adjRoadVert.Position + leftOffset,
                        end = roadVert.Position + leftOffset
                    },
                    right = new Line()
                    {
                        start = adjRoadVert.Position + rightOffset,
                        end = roadVert.Position + rightOffset
                    }
                };

                // Add incoming edges to array
                incRoadEdges[j] = roadSegmentEdges;
            }

            // Process incoming road edges.
            // This meshes any intersection polygons.
            if (incRoadEdges.Length == 1)
                ProcessRoadEnd(incRoadEdges[0], vertices, faces);
            else if (incRoadEdges.Length > 1)
                ProcessRoadIntersection(incRoadEdges, vertices, faces);
        }

        // TODO: Triangulate road segments


        return (vertices, faces);
    }

    private static void ProcessRoadEnd(RoadSegmentEdges incRoadEdges, List<Vector3> vertices, List<Face> faces)
    {
        vertices.Add(ToVec3(incRoadEdges.left.end));
        vertices.Add(ToVec3(incRoadEdges.right.end));
    }

    private static void ProcessRoadIntersection(RoadSegmentEdges[] incRoadEdges, List <Vector3> vertices, List<Face> faces)
    {
        // Perform right vs left road edge intersection in clockwise order.
        // Triangulate the road intersection polygon.
        List<int> intersectionMeshIndices = new List<int>();
        for (int j = 0; j < incRoadEdges.Length; ++j)
        {
            (bool isIntersect, Vector3 intersection) = WorldLineIntersect(incRoadEdges[j].left, incRoadEdges[(j + 1) % incRoadEdges.Length].right);
            if (isIntersect)
            {
                vertices.Add(intersection);
            }
            else // When parallel
            {
                vertices.Add(ToVec3(incRoadEdges[j].right.end));
            }

            if (j >= 2) // Need at least 3 vertices to form a polygon
            {
                // Vertices are in clockwise order.
                // Fan triangulation, assuming convex.
                int firstVertIdx = vertices.Count - j - 1;
                int secondVertIdx = vertices.Count - 2;
                int thirdVertIdx = vertices.Count - 1;
                intersectionMeshIndices.AddRange(new int[] { firstVertIdx, secondVertIdx, thirdVertIdx });
            }
        }
        if (intersectionMeshIndices.Count >= 3)
            faces.Add(new Face(intersectionMeshIndices.ToArray()));
    }

    private static Vector3 ToVec3(Vector2 vec2)
    {
        return new Vector3(vec2.x, 0, vec2.y);
    }

    // Intersects two infinite 2D lines.
    // Returns success or failure and the world space intersection point (y axis set to 0).
    private static (bool, Vector3) WorldLineIntersect(Line line1, Line line2)
    {
        bool isIntersection = false;
        Vector3 intersection = new Vector3(0, 0, 0);

        //3d -> 2d
        Vector2 p1 = line1.start;
        Vector2 p2 = line1.end;

        Vector2 p3 = line2.start;
        Vector2 p4 = line2.end;

        float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

        //Make sure the denominator is > 0, if so the lines are parallel
        if (denominator != 0)
        {
            isIntersection = true;

            float u_a = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
            float u_b = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

            intersection = ToVec3(p1 + u_a * (p2 - p1));
        }

        return (isIntersection, intersection);
    }
}
