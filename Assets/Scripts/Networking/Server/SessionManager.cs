using Assets.Scripts.Networking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;

namespace Assets.Scripts.Networking.Server
{
    class SessionManager : MonoBehaviour
    {
        private Dictionary<int, Session> _currentSessions;
        private Queue<TcpClient> _connectQueue;
        private DateTime _lastServerTime, _currServerTime;

        [SerializeField] internal Vector3 _serverSpawnPosition;

        internal void SendToAll(ByteBuffer buff)
        {
            foreach (var session in _currentSessions)
                session.Value.SendDirectMessage(buff);
        }

        internal int MaxPlayers { get; set; } = 16;

        private static SessionManager _instance;
        public static SessionManager Instance() => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        private void Start()
        {
            try
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 60;

                ServerManager.Start();
                Debug.Log("Started Server..");
            }
            catch
            {
                Debug.LogError("Failed to start server");
            }
        }

        private void FixedUpdate()
        {
            _lastServerTime = _currServerTime;
            _currServerTime = DateTime.Now;

            TimeSpan diff = _currServerTime.Subtract(_lastServerTime);
            OnUpdate(diff.TotalMilliseconds);
        }

        private SessionManager()
        {
            InitialiseSessions();
        }

        private void InitialiseSessions()
        {
            _currentSessions = new Dictionary<int, Session>(MaxPlayers);
            _connectQueue = new Queue<TcpClient>();
        }

        internal int GetNewestId()
        {
            if (_currentSessions.Count == MaxPlayers)
                return 0;

            if (_currentSessions.Count == 0)
                return 1;

            return (_currentSessions.Last().Key + 1);
        }

        internal void AddSessionToQueue(TcpClient client)
        {
            _connectQueue.Enqueue(client);
        }

        internal void OnUpdate(double diff)
        {
            HandleSessionQueue();

            foreach (var session in _currentSessions)
            {
                session.Value.Update(diff);
            }
        }

        internal void KickSession(Session session)
        {
            session.Kick();
            _currentSessions[session.GetId()] = null;
        }

        private void HandleSessionQueue()
        {
            lock (_connectQueue)
            {
                while (_connectQueue.Count > 0)
                {
                    TcpClient client = _connectQueue.Dequeue();

                    int newSessionId = GetNewestId();
                    if (newSessionId == 0)
                    {
                        Debug.LogError($"Can't connect player at {client.Client.RemoteEndPoint} because there are no sessions slots full!");
                        return;
                    }

                    Session newUser = new Session(newSessionId, client);
                    _currentSessions[newSessionId] = newUser;
                    newUser.Connect();
                    newUser.SendAuth();
                }
            }
        }
    }
}
