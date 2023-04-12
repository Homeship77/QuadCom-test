using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DataProcessing
{
    public class MapGenerator
    {
        private float _baseSizeX;
        private float _baseSizeY;
        private GameObject _cubePref;
        private GameObject _basePlane;
        private List<int[]> _levelData;
        Vector3 _cubeSize;
        Vector3 _halfCubeSize;
        private float _startX;
        private float _startY;
        private float _startZ;

        public int BaseSizeX => (int)_baseSizeX;
        public int BaseSizeY => (int)_baseSizeY;
        public List<int[]> LevelData => _levelData;

        public Vector3 HalfCubeSize => _halfCubeSize;

        public MapGenerator(GameObject cubePref, GameObject basePlane, Vector2 baseSize, List<int[]> levelData)
        {
            _baseSizeX = baseSize.x;
            _baseSizeY = baseSize.y;
            _levelData = levelData;
            _basePlane = basePlane;
            _cubePref = cubePref;
            _cubeSize = _cubePref.transform.localScale;
            _halfCubeSize = _cubeSize * 0.5f;
            //left top corner of base plane
            _startX = _halfCubeSize.x * _baseSizeX + _basePlane.transform.position.x; 
            _startY = _halfCubeSize.z * _baseSizeY + _basePlane.transform.position.z;
            _startZ = _basePlane.transform.position.y;
            _halfCubeSize.z = -_halfCubeSize.z;
            _halfCubeSize.x = -_halfCubeSize.x;
            SetupBasePlane();
            GenerateMap();
        }

        public Vector2 GetIndicesFromLocalPosition(Vector3 position)
        {
            var x = Mathf.Abs(position.x - _startX - _halfCubeSize.x) / _cubeSize.x;
            var y = Mathf.Abs(position.z - _startY - _halfCubeSize.z) / _cubeSize.z;
            return new Vector2((int)Math.Round(x, MidpointRounding.ToEven), (int)Math.Round(y, MidpointRounding.ToEven));
        }

        public Vector3 GetLocalPositionFromIndices(int x, int y, int height = -1)
        {
            if (height < 0) 
                height = LevelData[x][y] > 0 ? LevelData[x][y] - 1 : LevelData[x][y];

            var res = new Vector3(_startX - x * _cubeSize.x, _startZ + height * _cubeSize.y, _startY - y * _cubeSize.z);
            res = res + _halfCubeSize;
            return res;
        }

        private void SetupBasePlane()
        {
            var startScale = _basePlane.transform.localScale;
            _basePlane.transform.localScale = new Vector3(startScale.x * (int)_baseSizeX, startScale.z, startScale.y * (int)_baseSizeY);
            var renderer = _basePlane.GetComponent<MeshRenderer>();
            renderer.sharedMaterial.mainTextureScale = new Vector2(BaseSizeX, BaseSizeY);
        }

        private void GenerateMap()
        {
            for (int i =0; i < BaseSizeX; i++) 
            { 
                for(int j =0; j < BaseSizeY; j++)
                {
                    for (int k = 0; k < _levelData[i][j]; k++)
                    {
                        var position = GetLocalPositionFromIndices(i, j, k);
                        var newCube = GameObject.Instantiate(_cubePref);
                        newCube.transform.localPosition = position;
                        newCube.transform.localRotation = Quaternion.identity;
                    }
                }
            }
        }
    }
}