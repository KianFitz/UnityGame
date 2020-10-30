using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Net.Client
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public static Dictionary<uint, PlayerManager> players = new Dictionary<uint, PlayerManager>();

        public GameObject localPlayerPrefab;
        public GameObject playerPrefab;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        public void Start()
        {
            Client.Instance().Connect();
        }

        /// <summary>Spawns a player.</summary>
        /// <param name="_id">The player's ID.</param>
        /// <param name="_name">The player's name.</param>
        /// <param name="_position">The player's starting position.</param>
        /// <param name="_rotation">The player's starting rotation.</param>
        public void SpawnPlayer(uint _id, Vector3 _position, Quaternion _rotation)
        {
            GameObject _player;
            if (_id == Client.Instance().myId)
            {
                _player = Instantiate(localPlayerPrefab, _position, _rotation);
            }
            else
            {
                _player = Instantiate(playerPrefab, _position, _rotation);
            }

            _player.GetComponent<PlayerManager>().id = _id;
            players.Add(_id, _player.GetComponent<PlayerManager>());
        }
    }
}
