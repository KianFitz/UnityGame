using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Net.Server
{
    class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _instance;
        public static NetworkManager Instance() => _instance;

        public GameObject playerPrefab;
        public Vector3 spawnPosition;

        private void Awake()
        {
            if (_instance is null)
            {
                _instance = this;
            }
            else if (_instance != null)
            {
                Debug.Log("Instance already exists, destroying obejct!");
                Destroy(this);
            }
        }

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;

            Server.Start(50, 27930);
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }

        public Player InstantiatePlayer()
        {
            return Instantiate(playerPrefab, spawnPosition, Quaternion.identity).GetComponent<Player>();
        }
    }
}
