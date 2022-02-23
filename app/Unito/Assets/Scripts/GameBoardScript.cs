using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameBoardScript : MonoBehaviour
{
    private static readonly System.Random getRandom = new System.Random();
    public delegate void OnGameBoardRebuild(int x, int y, float spacingX, float spacingY);

    public static event OnGameBoardRebuild onGameBoardRebuild;

    [SerializeField] private Transform redPrefab;
    [SerializeField] private Transform greenPrefab;
    [SerializeField] private Transform bluePrefab;
    [SerializeField] private float spacingX;
    [SerializeField] private float spacingY;
    [SerializeField] private Transform playerPrefab;

    private List<List<LogicTile>> _tiles = new List<List<LogicTile>>();
    private List<Transform> tileInstances = new List<Transform>();
    private Dictionary<string, Transform> players = new Dictionary<string, Transform>();

    /// <summary>
    /// Registering methods to corresponding events
    /// There methods are called with the give parameters, when the event is fired
    /// </summary>
    private void Awake()
    {
        NetworkEventManager.onInitMessage += RebuildOnInit;
        NetworkEventManager.onMoveMessage += OnMoveMessage;
    }

    /// <summary>
    /// For the sake of this example, only two players "p1" and "p2" are spawned at random locations on the map 
    /// </summary>
    private void CreateBothPlayers()
    {
        players.Add("p1", SpawnPlayerAtRandomPos());
        players.Add("p2", SpawnPlayerAtRandomPos());
    }
    /// <summary>
    /// Spawns player (in the middle) of a random tile
    /// </summary>
    /// <returns>Instantiated object</returns>
    private Transform SpawnPlayerAtRandomPos()
    {
        var pX = getRandom.Next(_tiles.Count);
        var pY = getRandom.Next(_tiles[pX].Count);
        return Instantiate(playerPrefab, _tiles[pX][pY].Pos, Quaternion.identity);
    }

    /// <summary>
    /// Adds the waypoint the player will walk to based on the server send move to message
    /// </summary>
    /// <param name="msg">Move message parsed from server send message</param>
    private void OnMoveMessage(MoveMessage msg)
    {
        players[msg.player].GetComponent<PlayerScript>().AddWaypoint(_tiles[msg.x][msg.y].Pos);
    }

    /// <summary>
    /// Clears tiles and player object and recreates the gameboard based on the parameteres in the InitMessage object
    /// </summary>
    /// <param name="msg">InitMessage object parsed from the server send message</param>
    private void RebuildOnInit(InitMessage msg)
    {
        ClearBoardAndPlayers();
        _tiles = TileHelper.ConvertMsgToLogicTiles(msg.tiles, transform.position, spacingX, spacingY);
        GenerateBoardTiles(_tiles);
        CreateBothPlayers();
        onGameBoardRebuild?.Invoke(msg.tiles.Count, msg.tiles[0].Count, spacingX, spacingY);
    }

    /// <summary>
    /// Destroyes all LogicTiles, the visual tile objects and the players 
    /// </summary>
    private void ClearBoardAndPlayers()
    {
        _tiles = new List<List<LogicTile>>();
        tileInstances.ForEach(Destroy);
        foreach (var player in players.Values)
        {
            Destroy(player);
        }
    }
    
    /// <summary>
    /// Creates the necessary visual tile objects (prefabs) based on the server send LogicTiles
    /// Also adds the x/y coordinates to the tilescripts and adds the lower earth parts only if they are on the
    /// outer edge of the gameboard
    /// </summary>
    /// <param name="tiles">2D-List </param>
    private void GenerateBoardTiles(List<List<LogicTile>> tiles)
    {
        for (var x = 0; x < tiles.Count; x++)
        {
            for (var y = 0; y < tiles[x].Count; y++)
            {
                var tilePrefab = InstantiatePrefabAtPosition(tiles[x][y].Pos, tiles[x][y].TileType);
                tilePrefab.GetComponent<TileScript>().InitXYIndexes(x,y);
                tileInstances.Add(tilePrefab);
                if (x == 0 || y == 0 || x + 1 == tiles.Count || y + 1 == tiles[x].Count)
                {
                    tilePrefab.GetComponent<TileScript>().AddEarthPart();
                }
            }
        }
    }

    /// <summary>
    /// Instantiated based on the tile type the corresponding prefab
    /// </summary>
    /// <param name="pos">world position to spawn</param>
    /// <param name="tileType">type of tile</param>
    /// <returns>Spawned object</returns>
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