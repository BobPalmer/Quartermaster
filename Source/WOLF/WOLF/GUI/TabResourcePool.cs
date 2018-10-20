using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using KolonyTools;
using Quartermaster;
using UnityEngine;
using USITools;

namespace WOLF
{
    public class TabResourcePool
    {
        private GUIStyle _labelStyle;
        private GUIStyle _texStyle;
        private GUIStyle _scrollStyle;
        private Vector2 _scrollPos = Vector2.zero;
        private Texture2D texOrbit;
        private Texture2D texBase;
        private Texture2D texArrow;

        private double LastUpdate;
        private const double UpdateFrequency = 0.5d;
        private List<NetworkRepository.PoolData> _poolData;
        private IResourceNetworkProvider _resNet;

        public TabResourcePool()
        {
            _labelStyle = new GUIStyle(HighLogic.Skin.label);
            _texStyle = new GUIStyle(HighLogic.Skin.label);
            _texStyle.margin = new RectOffset(0, 0, 0, 0);
            _texStyle.padding = new RectOffset(0, 0, 0, 0);
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            _scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);
            LastUpdate = Planetarium.GetUniversalTime();
            _resNet = new ResourceNetwork();
            texOrbit = LoadTex("WOLF_Orbit",16);
            texBase= LoadTex("WOLF_Base",16);
            texArrow= LoadTex("WOLF_Arrow",16);
        }

        private Texture2D LoadTex(string file, int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var textureFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets/UI/" + file + ".png");
            tex.LoadImage(File.ReadAllBytes(textureFile));
            return tex;
        }

        private void RefreshPoolList()
        {
            LastUpdate = Planetarium.GetUniversalTime();
            _poolData = _resNet.Instance.Repo.GetPoolDisplayList();
        }

        public void Display()
        {
            if (LastUpdate + UpdateFrequency < Planetarium.GetUniversalTime() || _poolData == null)
            {
                RefreshPoolList();
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, _scrollStyle, GUILayout.Width(680), GUILayout.Height(380));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.Height(24));
            GUILayout.Label("<color=#FFFFFF></color>", _labelStyle, GUILayout.Width(30));
            GUILayout.Label("<color=#FFFFFF>Body</color>", _labelStyle, GUILayout.Width(120));
            GUILayout.Label("<color=#FFFFFF>Vessel</color>", _labelStyle, GUILayout.Width(150));
            GUILayout.Label("<color=#FFFFFF>Resource</color>", _labelStyle, GUILayout.Width(150));
            GUILayout.Label("<color=#FFFFFF>Input</color>", _labelStyle, GUILayout.Width(50));
            GUILayout.Label("<color=#FFFFFF>Store</color>", _labelStyle, GUILayout.Width(50));
            GUILayout.Label("<color=#FFFFFF>Output</color>", _labelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            var count = _poolData.Count;
            for (int i = 0; i < count; ++i)
            {
                var p = _poolData[i];
                var dCount = p.ResourceList.Count;
                for (int d = 0; d < dCount; ++d)
                {
                    var det = p.ResourceList[d];
                    GUILayout.BeginHorizontal(GUILayout.Height(24));
                    if(p.VesselSituation == "Orbiting")
                        GUILayout.Label(texOrbit, _texStyle, GUILayout.Width(30));
                    else
                        GUILayout.Label(texBase, _texStyle, GUILayout.Width(30));
                    GUILayout.Label(String.Format("{0}", p.BodyName), _labelStyle, GUILayout.Width(120));
                    GUILayout.Label(p.VesselName, _labelStyle, GUILayout.Width(150));
                    GUILayout.Label(det.ResourceName, _labelStyle, GUILayout.Width(150));
                    GUILayout.Label(det.Incoming.ToString(), _labelStyle, GUILayout.Width(50));
                    GUILayout.Label(det.Available.ToString(), _labelStyle, GUILayout.Width(50));
                    GUILayout.Label(det.Outgoing.ToString(), _labelStyle, GUILayout.Width(50));
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }



    }
}