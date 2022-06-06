using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;
    [SerializeField] private int width = 256; //x-axis of the terrain
    [SerializeField] private int height = 256; //z-axis

    [SerializeField] private float sinWaveLength = 1;
    [SerializeField] private float sinWaveAmplitude = 1;
    
    [SerializeField] private int depth = 20; //y-axis

    [SerializeField] private float scale = 20f;

    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 0f;

    private void Start()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
    }

    private void Update()
    {
        _terrain.terrainData = GenerateTerrain(_terrain.terrainData);
    }

    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return overlayWidthSin(heights);
    }

    float[,] overlayWidthSin(float[,] noise)
    {
        float sinState = 0;
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noise[x, y] += ( ((Mathf.Sin(sinState) / 2) + 1 ) * sinWaveAmplitude) ;
            }

            sinState += sinWaveLength;
        }

        return noise;
    }

    float CalculateHeight (int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}