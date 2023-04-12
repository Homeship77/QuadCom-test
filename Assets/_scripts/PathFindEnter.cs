using System;
using UnityEngine;
using UserInput;
using DataProcessing;
using QuadComTest.EventSystems;
using QuadComTest.Interfaces;
using System.Collections.Generic;
using UnityEditor;
using System.Drawing;
using Unity.VisualScripting;

public class PathFindEnter : MonoBehaviour, IUserEvent
{
    [SerializeField]
    private string levelFileName;
    [SerializeField]
    private GameObject _cubePrefab;
    [SerializeField]
    private GameObject _basePlane;
    [SerializeField]
    private LineRenderer lineRenderer;

    private List<Vector3> linePath;
    private MapGenerator _mapGen;
    private Path _pathBuilder;
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _endPos = Vector3.zero;

    public event Action<float> OnUpdateEvent;

    public void ReadyToPathCalculate(Vector3 startPos, Vector3 endPos)
    {
        linePath = new List<Vector3>();
        lineRenderer.positionCount = linePath.Count;

        var startIndices = _mapGen.GetIndicesFromLocalPosition(startPos);
        var endIndices = _mapGen.GetIndicesFromLocalPosition(endPos);
        _pathBuilder.Calculate(startIndices, endIndices, out var result);

        if (result.Count > 0)
        {
            foreach (var tile in result) 
            {
                int xInd = (int)tile.x;
                int yInd = (int)tile.y; 
                var pointPos = _mapGen.GetLocalPositionFromIndices(xInd, yInd) + Vector3.up * _mapGen.HalfCubeSize.y;
                linePath.Add(pointPos);
            }
            lineRenderer.SetPositions(linePath.ToArray());
            lineRenderer.positionCount = linePath.Count;
            lineRenderer.enabled = true;
            OnUpdateEvent += DrawPath;
        }
        else
        {
            lineRenderer.enabled = false;
            OnUpdateEvent -= DrawPath;
        }
    }


    public void SelectStartPos(Vector3 startPos)
    {
        _startPos = startPos;
    }

    public void SelectEndPos(Vector3 endPos)
    {
        _endPos = endPos;
    }

    public void GetPrecisePosition(Vector3 pointPos, out Vector3 precPos) 
    {
        var indices = _mapGen.GetIndicesFromLocalPosition(pointPos);
        precPos = _mapGen.GetLocalPositionFromIndices((int)indices.x, (int)indices.y);
    }

    private void Start()
    {
        if (levelFileName.Equals(""))
            return;
        
        MouseRayCastProcessor mouseProcessor = new MouseRayCastProcessor(this);
        LoadDataFileService loadDataFileService = new LoadDataFileService();
        Vector2 levelSize = Vector2.zero;
        List<int[]> levelData = loadDataFileService.LoadDataFile(levelFileName, out levelSize);
        _mapGen = new MapGenerator(_cubePrefab, _basePlane, levelSize, levelData);
        _pathBuilder = new Path(levelData, Array.Empty<Vector2>(), 10000); //empty obtacles list

        EventManager.Subscribe(this);
    }

    private void Update()
    {
        OnUpdateEvent?.Invoke(Time.deltaTime);
    }

    private void DrawPath(float deltaTime)
    {
        lineRenderer.SetPositions(linePath.ToArray());
        lineRenderer.positionCount = linePath.Count;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        if (!_startPos.Equals(Vector3.zero))
        {
            Gizmos.DrawCube(_startPos, Vector3.one * 1.01f);
        }
        Gizmos.color = UnityEngine.Color.green;
        if (!_endPos.Equals(Vector3.zero))
        {
            Gizmos.DrawCube(_endPos, Vector3.one * 1.01f);
        }
    }

}
