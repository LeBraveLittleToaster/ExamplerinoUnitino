using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 256; //x-axis of the terrain
    public int height = 256; //z-axis

    public float sinWaveLength = 1;
    public float sinWaveAmplitude = 1;
    
    public int depth = 20; //y-axis

    public float scale = 20f;

    public float offsetX = 0f;
    public float offsetY = 0f;

    private void Start()
    {
        //offsetX = Random.Range(0f, 9999f);
        //offsetY = Random.Range(0f, 9999f);
    }

    private void Update()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
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