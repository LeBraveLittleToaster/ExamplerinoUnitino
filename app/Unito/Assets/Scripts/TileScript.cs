using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TileScript : MonoBehaviour
{
    private static readonly Random getRandom = new Random();

    [SerializeField] private Transform prefabEarthPart;
    [SerializeField] private Vector3 scale = new Vector3(.2f, .2f, .2f);

    [SerializeField] private Transform[] prefabsBuildings;
    [SerializeField] private Transform[] prefabsEnvironmentEntities;
    [SerializeField] private Transform[] buildingSpawns;
    [SerializeField] private Transform[] environmentEntitesSpawns;
    [SerializeField] private int buildingCount = 1;
    [SerializeField] private int environmentEntities = 2;
    [SerializeField] private Vector3 buildingScale = new Vector3(.2f, .2f, .2f);
    [SerializeField] private Vector3 entityScale = new Vector3(.2f, .2f, .2f);
    [SerializeField] private bool buildingsWithRandomRotation = true;
    [SerializeField] private bool entitesWithRandomRotation = true;

    private Transform earth;
    private List<Transform> buildings = new List<Transform>();
    private List<Transform> entities = new List<Transform>();
    public int XIndex { get; private set; }
    public int YIndex { get; private set; }

    void Start()
    {
        SpawnBuildings();
        SpawnEntities();
    }

    public void InitXYIndexes(int x, int y)
    {
        XIndex = x;
        YIndex = y;
    }

    private void SpawnBuildings()
    {
        for (var i = 0; i < buildingCount; i++)
        {
            var building = prefabsBuildings[getRandom.Next(prefabsBuildings.Length)];
            var buildingSpawn = buildingSpawns[getRandom.Next(buildingSpawns.Length)];
            buildings.Add(InstatiateBuildingOrEntity(building, buildingSpawn, buildingsWithRandomRotation));
        }
    }
    private void SpawnEntities()
    {
        for (var i = 0; i < environmentEntities; i++)
        {
            var entity = prefabsEnvironmentEntities[getRandom.Next(prefabsEnvironmentEntities.Length)];
            var entitySpawn = environmentEntitesSpawns[getRandom.Next(environmentEntitesSpawns.Length)];
            entities.Add(InstatiateBuildingOrEntity(entity, entitySpawn, entitesWithRandomRotation));
        }
    }

    private Transform InstatiateBuildingOrEntity(Transform prefab, Transform spawn, bool withRndRotation)
    {
        var rotation = withRndRotation ? transform.rotation * Quaternion.AngleAxis(getRandom.Next(360), Vector3.up) : transform.rotation;
        var instance = Instantiate(prefab, spawn.position, rotation);
        instance.localScale = entityScale;
        return instance;
    }

    public void AddEarthPart()
    {
        if (earth == null)
        {
            var rndRotationOffset = getRandom.Next(4);
            var quaternion = Quaternion.AngleAxis(90 * rndRotationOffset, Vector3.up);
            earth = Instantiate(prefabEarthPart, transform.position, transform.rotation * quaternion);
            earth.localScale = scale;
        }
    }
}