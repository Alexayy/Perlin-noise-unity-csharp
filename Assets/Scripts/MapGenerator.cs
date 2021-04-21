using System;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap };

    public DrawMode drawMode;
    
    [Header("Map Size")] public int mapWidth;
    public int mapHeigth;

    [Header("Noise Control")] public float noiseScale;
    public int octaves;
    
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offSet;

    [Header("Update Control")] public bool autoUpdate;

    [Header("REGIONS")] public TerrainType[] regions;
    
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeigth, seed, noiseScale, octaves, persistance,
            lacunarity, offSet);

        Color[] colorMap = new Color[mapWidth * mapHeigth];
        for (int y = 0; y < mapHeigth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }
        
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if (drawMode == DrawMode.ColorMap)
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeigth));
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
            mapWidth = 1;
        
        if (mapHeigth < 1)
            mapHeigth = 1;
        
        if (lacunarity < 1)
            lacunarity = 1;

        if (octaves < 0)
            octaves = 0;
    }
}

[Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}