using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;
    //[SerializeField] private int width = 256; //x-axis of the terrain
    //[SerializeField] private int height = 256; //z-axis

    [SerializeField] private float sinWaveLength = 1;
    [SerializeField] private float sinWaveAmplitude = 1;
    
    [SerializeField] private int depth = 20; //y-axis

    [SerializeField] private float scale = 20f;

    [SerializeField] private int cellSize = 20;
    [SerializeField] private int cellCountWidth = 10;
    [SerializeField] private int cellCountHeight = 10;

    [SerializeField] private float tilingCutoff = 1f;
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 0f;

    private void Start()
    {
        //offsetX = Random.Range(0f, 9999f);
        //offsetY = Random.Range(0f, 9999f);
    }

    private void Update()
    {
        _terrain.terrainData = GenerateTerrain(_terrain.terrainData);
    }

    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        var width = cellSize * cellCountWidth;
        var height = cellSize * cellCountHeight;
        terrainData.heightmapResolution = 65;
        terrainData.alphamapResolution = 65;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights(terrainData, 65,65));
        terrainData.SetAlphamaps(0,0, GenerateSplatmapData(terrainData, cellCountWidth, cellCountHeight));
        return terrainData;
    }

    /**
     * Based on https://docs.unity3d.com/2021.3/Documentation/ScriptReference/TerrainData.SetAlphamaps.html
     */
    float[,,] GenerateSplatmapData(TerrainData terrainData, float cellCountWidth, float cellCountHeight)
    {
        
        var splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        var cellExtendWidth = terrainData.alphamapWidth / cellCountWidth;
        var cellExtendHeight = terrainData.alphamapHeight / cellCountHeight;
        
        for (var y = 0; y < terrainData.alphamapHeight; y++)
        {
            var texPosY = ((float)(y) / terrainData.alphamapHeight);
            var iY = (int)((texPosY / cellExtendWidth) * cellCountHeight);
            
            for (var x = 0; x < terrainData.alphamapWidth; x++)
            {
                splatmapData[x, y, 0] = 1;
                var texPosX = ((float)(x) / terrainData.alphamapWidth);
                var iX = (int)((texPosX / cellExtendHeight) * cellCountWidth);

                splatmapData[x, y, 1] = GetTileColor(iX, iY);
            }    
        }

        return splatmapData;
    }

    int GetTileColor(int iX, int iY)
    {
        if (iX % 2 == 0)
        {
            return iY % 2 == 0 ? 1 : 0;
        }
        return iY % 2 == 0 ? 0 : 1;
    }

    float[,] GenerateHeights(TerrainData terrainData, int width, int height)
    {
        var depths = new float[width, height];
        for(var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                depths[x, y] = CalculateHeight(width, height, x,y);
            }
        }

        return OverlayWidthSin(depths);
    }

    float[,] OverlayWidthSin(float[,] noise)
    {
        float sinState = 0;
        for(var x = 0; x < noise.GetLength(0); x++)
        {
            for (var y = 0; y < noise.GetLength(1); y++)
            {
                noise[x, y] += ( ((Mathf.Sin(sinState) / 2) + 1 ) * sinWaveAmplitude) ;
            }

            sinState += sinWaveLength;
        }

        return noise;
    }

    float CalculateHeight (int width, int height, float x, float y)
    {
        var xCoord = x / width * scale + offsetX;
        var yCoord = y / height * scale + offsetY;
        
        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}