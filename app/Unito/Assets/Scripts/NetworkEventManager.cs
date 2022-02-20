using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

// WebSocket Sharp Lib: https://github.com/sta/websocket-sharp
// WebSocket Client example by: https://medium.com/unity-nodejs/websocket-client-server-unity-nodejs-e33604c6a006
public class NetworkEventManager : MonoBehaviour
{
    public delegate void OnMoveMessage(MoveMessage msg);

    public static event OnMoveMessage onMoveMessage;
    
    public delegate void OnInitMessage(InitMessage msg);

    public static event OnInitMessage onInitMessage;

    private Queue<string> _messageQueue = new Queue<string>();

    WebSocket ws;

    void Start()
    {
        ws = new WebSocket("ws://localhost:1324");
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            _messageQueue.Enqueue(e.Data); // Queue fixes multithreading problem
        };
    }

    void Update()
    {
        if (ws == null)
        {
            return;
        }
        RunMessageQueue();
    }

    private void RunMessageQueue()
    {
        if (_messageQueue == null || _messageQueue.Count <= 0) return;
        var json = _messageQueue.Dequeue();
        var msg = JsonConvert.DeserializeObject<Message>(json);
        Debug.Log(msg.msgType);
        TriggerEventOnMessageType(msg.msgType, json);
        
    }

    private void TriggerEventOnMessageType(MessageType msgType, string json)
    {
        switch (msgType)
        {
            case MessageType.INIT:
                onInitMessage?.Invoke(JsonConvert.DeserializeObject<InitMessage>(json));
                break;
            case MessageType.MOVE:
                onMoveMessage?.Invoke(JsonConvert.DeserializeObject<MoveMessage>(json));
                break;
            default:
                Debug.Log("Failed to parse MessageType");
                break;
        }
    }
}
[Serializable]
public enum MessageType
{
    INIT,
    MOVE
}
[Serializable]
public enum TileType
{
    RED, GREEN, BLUE
}
[Serializable]
public class Message
{
    public MessageType msgType;
}
[Serializable]
public class MoveMessage : Message
{
    public int x;
    public int y;
    public string player;
}

[Serializable]
public class InitMessage : Message
{
    public List<List<MsgTile>> tiles;
}
[Serializable]
public class MsgTile
{
    public TileType tileType;
}