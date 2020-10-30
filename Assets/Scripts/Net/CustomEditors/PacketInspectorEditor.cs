using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Networking.CustomEditors
{
    [CustomEditor(typeof(PacketInspector)), CanEditMultipleObjects]
    public class PacketInspectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PacketInspector inspector = (PacketInspector)target;

            if (DrawDefaultInspector())
                if (inspector.autoUpdate)
                    inspector.FindOpcode();

            if (GUILayout.Button("Update"))
                inspector.FindOpcode();

        }
    }
}
