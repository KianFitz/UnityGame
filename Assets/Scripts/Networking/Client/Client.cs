using Assets.Scripts.Networking.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;
using Assets.Scripts.Networking.Client.Handlers.Auth;
using Assets.Scripts.Networking.Client.Handlers.WorldHandler;

public class Client : MonoBehaviour
{
    private static Client _instance;
    public static Client Instance() => _instance;

    public string Ip = "127.0.0.1";
    public int Port = 27930;
    public int ClientID;

    private delegate void OpcodeHandler(ByteBuffer data);
    private static Dictionary<Opcode, OpcodeHandler> _OpcodeHandler;

    private NetworkStream _stream;
    private TcpClient _client;
    private byte[] _buffer;

    private const int dataBufferSize = 4096;

    private bool _isConnected = false;

    private void Awake()
    {
        if (_instance is null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("Duplicate instance of Client, Destroying newest");
            Destroy(this);
        }
    }

    void Start()
    {
        InitialisePackets();
        _client = new TcpClient()
        {
            SendBufferSize = dataBufferSize,
            ReceiveBufferSize = dataBufferSize
        };

        _buffer = new byte[dataBufferSize];
        ConnectToServer();
    }

    private void InitialisePackets()
    {
        _OpcodeHandler = new Dictionary<Opcode, OpcodeHandler>()
        {
            { Opcode.SMSG_AUTH, AuthHandler.HandleServerAuthMSG },
            { Opcode.CMSG_AUTH_ACK, Handle_NULL },
            { Opcode.SMSG_PLAYER_JOINED, WorldHandler.NewPlayerSpawned },
            { Opcode.Test, Test }
        };
    }

    private void Test(ByteBuffer data)
    {
        Debug.Log("Recieved Test");
    }

    private void Handle_NULL(ByteBuffer data)
    {
        throw new NotImplementedException("Received Client Side Packet on Client!");
    }

    private void ConnectToServer()
    {
        _client.BeginConnect(Ip, Port, OnConnectCallback, null);
    }

    private void OnConnectCallback(IAsyncResult ar)
    {
        _client.EndConnect(ar);

        if (!_client.Connected)
        {
            return;
        }

        _isConnected = true;
        _stream = _client.GetStream();
        _stream.BeginRead(_buffer, 0, dataBufferSize, OnReceivedData, null);
    }

    private void OnReceivedData(IAsyncResult result)
    {
        try
        {
            int dataLength = _stream.EndRead(result);
            if (dataLength <= 0)
            {
                Disconnect();
                return;
            }

            byte[] incomingData = new byte[dataLength];
            Array.Copy(_buffer, incomingData, dataLength);

            HandlePacket(incomingData);
            _stream.BeginRead(_buffer, 0, dataBufferSize, OnReceivedData, null);
        }
        catch (Exception ex)
        {
            Debug.Log($"Failed to receive data due to an exception with message {ex.Message} at {ex.StackTrace}");
        }
    }

    private void HandlePacket(byte[] incomingData)
    {
        ByteBuffer buff = new ByteBuffer(incomingData);
        int opcode = buff.ReadInt();

        if (_OpcodeHandler.TryGetValue((Opcode)opcode, out OpcodeHandler handler))
        {
            handler.Invoke(buff);
        }
        else
        {
            Debug.LogError("Recieved unknown packet, disconnecting client");
            Disconnect();
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    private void Disconnect()
    {
        if (_isConnected)
        {
            _client.Close();
            _stream.Close();
            _client = null;
            _stream = null;

            _isConnected = false;
        }
    }
}
