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

    /// <summary>
    /// This queue is needed to break the multithread problem caused by unity beeing single threaded
    /// The queue is only read from the mainthread (in the Update funtion) and can be filled by other threads
    /// </summary>
    private Queue<string> _messageQueue = new Queue<string>();

    WebSocket ws;

    void Start()
    {
        ws = new WebSocket("ws://localhost:1324");
        
        ws.OnMessage += (sender, e) =>
        {
            _messageQueue.Enqueue(e.Data); // Queue fixes multithreading problem
        };
        ws.Connect();
    }

    void Update()
    {
        if (ws == null)
        {
            return;
        }
        // Check if queue is empty and if not send events based on the message type
        RunMessageQueue();
    }

    private void RunMessageQueue()
    {
        if (_messageQueue == null || _messageQueue.Count <= 0) return;
        var json = _messageQueue.Dequeue();
        // first parse the base message object to get the message type
        // It is important to use the JsonConvert instead of the JsonUtility from Unity, otherwise enum are broken
        var msg = JsonConvert.DeserializeObject<Message>(json);
        Debug.Log(msg.msgType);
        // parse the message a second time with the specific message object
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

// From here are the objects that can be send via the network
// Normally you store them in seperate class files and reference them, also they are normally larger
// It is important to add the [Serializable] above to mark them as json parsable

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
