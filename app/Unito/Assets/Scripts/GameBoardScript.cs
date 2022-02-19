using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoardScript : MonoBehaviour
{
    public delegate void OnGameBoardRebuild(int x, int y, float spacingX, float spacingY);

    public static event OnGameBoardRebuild onGameBoardRebuild;
    
    [SerializeField] private Transform redPrefab;
    [SerializeField] private Transform greenPrefab;
    [SerializeField] private Transform bluePrefab;
    [SerializeField] private float spacingX;
    [SerializeField] private float spacingY;

    private List<List<LogicTile>> _tiles = new List<List<LogicTile>>();
    private List<Transform> tileInstances = new List<Transform>();

    private void Awake()
    {
        NetworkEventManager.onInitMessage += RebuildOnInit;
    }

    private void RebuildOnInit(InitMessage msg)
    {
        _tiles = TileHelper.ConvertMsgToLogicTiles(msg.tiles, transform.position, spacingX, spacingY);
        GenerateBoardTiles(_tiles);
        onGameBoardRebuild?.Invoke(msg.tiles.Count, msg.tiles[0].Count, spacingX, spacingY);
    }

    private void GenerateBoardTiles(IEnumerable<List<LogicTile>> tiles)
    {
        foreach (var tile in tiles.SelectMany(xList => xList))
        {
            tileInstances.Add(InstantiatePrefabAtPosition(tile.Pos, tile.TileType));
        }
    }

    private Transform InstantiatePrefabAtPosition(Vector3 pos, TileType tileType)
    {
        switch (tileType)
        {
            case TileType.RED:
                return Instantiate(redPrefab, pos, Quaternion.identity);
            case TileType.GREEN:
                return Instantiate(greenPrefab, pos, Quaternion.identity);
            case TileType.BLUE:
                return Instantiate(bluePrefab, pos, Quaternion.identity);
            default:
                return Instantiate(redPrefab, pos, Quaternion.identity);
        }
    }
}


public class LogicTile
{
    public Vector3 Pos { get; }
    public TileType TileType { get; }

    public LogicTile(Vector3 pos, TileType tileType)
    {
        Pos = pos;
        TileType = tileType;
    }
}

public class TileHelper
{
    public static List<List<LogicTile>> ConvertMsgToLogicTiles(List<List<MsgTile>> msgTiles, Vector3 startPos,
        float spacingX, float spacingY)
    {
        var convertedTiles = new List<List<LogicTile>>();
        for (var x = 0; x < msgTiles.Count; x++)
        {
            var yList = new List<LogicTile>();
            for (var y = 0; y < msgTiles[x].Count; y++)
            {
                yList.Add(new LogicTile(
                    new Vector3(x * spacingX, 0, y * spacingY) + startPos,
                    msgTiles[x][y].tileType)
                );
            }

            convertedTiles.Add(yList);
        }

        return convertedTiles;
    }
}