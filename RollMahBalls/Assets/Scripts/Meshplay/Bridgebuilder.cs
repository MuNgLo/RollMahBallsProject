using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
public class Bridgebuilder : MonoBehaviour
{
    [Range(1,100)]
    public int _segments = 1;
    public float _width = 1.0f;
    private MeshFilter _meshF;
    private MeshRenderer _meshR;
    private MeshCollider _meshC;
    private Transform _start, _end;
    private Mesh _segment;
    void Start()
    {
        _meshF = GetComponent<MeshFilter>();
        _meshR = GetComponent<MeshRenderer>();
        _meshC = GetComponent<MeshCollider>();
        _start = this.transform.Find("Start");
        _end = this.transform.Find("End");
        BuildSegmentMesh();
    }

    private void Update()
    {
            BuildBridge();
    }

    private void BuildSegmentMesh()
    {
        _segment = new Mesh();
        _segment.name = "segmentMesh";
        _segment.vertices = new Vector3[] {
                _start.localPosition + _start.TransformDirection(Vector3.forward*_width),
                _start.localPosition + _start.TransformDirection(-Vector3.forward*_width),
                _end.localPosition + _end.TransformDirection(Vector3.forward*_width),
                _end.localPosition + _end.TransformDirection(-Vector3.forward*_width),
        };
        _segment.SetTriangles(
            new List<int>()
            {
                0,1,2,
                3,2,1
            }
            , 0, false);
        
    }

    private void BuildBridge()
    {
        List<Vector3> bridgePoints = CalculateBridgePoints();
        Mesh meshData = new Mesh();
        meshData.name = "first";
        CombineInstance[] combines = MakeSegments(bridgePoints);
        if (combines.Length > 0)
        {
            meshData.CombineMeshes(combines, true, false);
        }
        meshData.RecalculateBounds();
        meshData.RecalculateNormals();
        meshData.RecalculateTangents();
        _meshF.sharedMesh = meshData;
        _meshC.sharedMesh = meshData;
    }

    private CombineInstance[] MakeSegments(List<Vector3> points)
    {
        CombineInstance[] combines = new CombineInstance[points.Count - 1];
        for (int i = 0; i < points.Count -1; i++)
        {
            combines[i] = MakeSegment(points[i], points[i + 1]);
        }
        return combines;
    }

    private CombineInstance MakeSegment(Vector3 startPoint, Vector3 endPoint)
    {
        CombineInstance result = new CombineInstance();
        result.mesh = new Mesh();
        result.mesh.vertices = _segment.vertices;
        result.mesh.triangles = _segment.triangles;
        result.mesh.vertices = new Vector3[] {
                startPoint + _start.TransformDirection(Vector3.forward*_width),
                startPoint + _start.TransformDirection(-Vector3.forward*_width),
               endPoint + _end.TransformDirection(Vector3.forward*_width),
                endPoint + _end.TransformDirection(-Vector3.forward*_width),
        };
        return result;
    }

    private List<Vector3> CalculateBridgePoints()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 direction = (_end.position - _start.position).normalized;
        float pointDistance = Vector3.Distance(_start.position, _end.position) / _segments;
        // Add start point
        points.Add(_start.localPosition);
        // Add points on the way
        for (int i = 1; i < _segments; i++)
        {
            points.Add(_start.localPosition + direction*pointDistance*i);
        }
        // Add end point
        points.Add(_end.localPosition);
        return points;
    }
}
