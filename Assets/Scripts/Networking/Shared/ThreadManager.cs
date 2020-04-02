using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking.Client
{
    class ThreadManager : MonoBehaviour
    {
        readonly static Queue<Action> _queue = new Queue<Action>();

        private void Update()
        {
            UpdateQueue();
        }

        public static void AddToQueue(Action action)
        {
            lock (_queue)
            {
                if (action is null)
                    return;

                _queue.Enqueue(action);
            }
        }

        static void UpdateQueue()
        {
            lock (_queue)
            {
                if (_queue.Count != 0)
                {
                    while (_queue.Count > 0)
                    {
                        Action action = _queue.Dequeue();
                        action();
                    }
                }
            }
        }


    }
}
