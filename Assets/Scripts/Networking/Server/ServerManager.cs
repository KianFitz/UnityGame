using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using Assets.Scripts.Networking.Server;

public class ServerManager
{
    static TcpListener _server;
    static UdpManager _udpManager;

    public static int Port = 27930;

    public static void Start()
    {
        _server = new TcpListener(IPAddress.Any, Port);
        _server.Start();
        _server.BeginAcceptTcpClient(new AsyncCallback(OnClientConnected), null);
    }

    private static void OnClientConnected(IAsyncResult ar)
    {
        TcpClient client = _server.EndAcceptTcpClient(ar);
        _server.BeginAcceptTcpClient(new AsyncCallback(OnClientConnected), null);

        SessionManager.Instance().AddSessionToQueue(client);
    }
}
