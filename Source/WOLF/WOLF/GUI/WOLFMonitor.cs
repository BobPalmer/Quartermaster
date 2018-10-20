using System;
using System.IO;
using System.Reflection;
using KSP.UI.Screens;
using UnityEngine;
using USITools;

namespace WOLF.GUI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class WOLFMonitor_Flight : WOLFMonitor
    { }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class WOLFMonitor_SpaceCenter : WOLFMonitor
    { }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class WOLFMonitor_TStation : WOLFMonitor
    { }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class WOLFMonitor_Editor : WOLFMonitor
    { }

    public class WOLFMonitor : MonoBehaviour
    {
        private ApplicationLauncherButton wolfButton;
        private IButton wolfTButton;
        private Rect _windowPosition = new Rect(300, 60, 700, 460);
        private GUIStyle _windowStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _scrollStyle;
        private GUIStyle _smButtonStyle;
        private Vector2 _scrollPos = Vector2.zero;
        private bool _hasInitStyles = false;
        private bool windowVisible;
        public static bool renderDisplay = false;
        public int curTab = 0;

        private TabResourcePool _tPool;

        private TabResourcePool PoolTab
        {
            get
            {
                if (_tPool == null)
                    _tPool = new TabResourcePool();
                return _tPool;
            }
        }

        void Awake()
        {
            if (ToolbarManager.ToolbarAvailable)
            {
                this.wolfTButton = ToolbarManager.Instance.add("UKS", "WOLF");
                wolfTButton.TexturePath = "UmbraSpaceIndustries/MKS/Assets/UI/WOLF";
                wolfTButton.ToolTip = "USI WOLF";
                wolfTButton.Enabled = true;
                wolfTButton.OnClick += (e) => { if (windowVisible) { GuiOff(); windowVisible = false; } else { GuiOn(); windowVisible = true; } };
            }
            else
            {
                var texture = new Texture2D(36, 36, TextureFormat.RGBA32, false);
                var textureFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets/UI/WOLF.png");
                print("Loading " + textureFile);
                texture.LoadImage(File.ReadAllBytes(textureFile));
                this.wolfButton = ApplicationLauncher.Instance.AddModApplication(GuiOn, GuiOff, null, null, null, null,
                    ApplicationLauncher.AppScenes.ALWAYS, texture);
            }
        }

        public void GuiOn()
        {
            renderDisplay = true;
        }

        public void Start()
        {
            if (!_hasInitStyles)
            {
                InitStyles();
            }
        }

        public void GuiOff()
        {
            renderDisplay = false;
        }

        private void OnGUI()
        {
            try
            {
                if (!renderDisplay)
                    return;

                if (Event.current.type == EventType.Repaint || Event.current.isMouse)
                {
                    //preDrawQueue
                }
                Ondraw();
            }
            catch (Exception ex)
            {
                print("ERROR in WOLFMonitor (OnGui) " + ex.Message);
            }
        }


        private void Ondraw()
        {
            _windowPosition = GUILayout.Window(16, _windowPosition, OnWindow, "Warehousing and Orbital Logistics Framework [W.O.L.F.]", _windowStyle);
        }

        private void OnWindow(int windowId)
        {
            GenerateWindow();
        }

        private void GenerateWindow()
        {
            var tabStrings = new[] { "Resource Pools"};
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            curTab = GUILayout.SelectionGrid(curTab, tabStrings, 6, _smButtonStyle);
            GUILayout.EndHorizontal();
            switch (curTab)
            {
                case 0:
                    PoolTab.Display();
                    break;
            }
            GUILayout.EndVertical();
            UnityEngine.GUI.DragWindow();
        }

        private static int GetFocusedPlanet()
        {
            if (HighLogic.LoadedSceneHasPlanetarium && MapView.MapCamera && MapView.MapCamera.target)
            {
                var cameraTarget = MapView.MapCamera.target;
                if (cameraTarget.celestialBody)
                {
                    return cameraTarget.celestialBody.flightGlobalsIndex;
                }
                else if (cameraTarget.vessel)
                {
                    return cameraTarget.vessel.mainBody.flightGlobalsIndex;
                }
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                return FlightGlobals.ActiveVessel.mainBody.flightGlobalsIndex;
            }
            return -1;
        }

        internal void OnDestroy()
        {
            if (wolfButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(wolfButton);
                wolfButton = null;
            }
            else
            {
                wolfTButton.Destroy();
                wolfTButton = null;
            }
        }

        private void InitStyles()
        {
            _windowStyle = new GUIStyle(HighLogic.Skin.window);
            _windowStyle.fixedWidth = 700;
            _windowStyle.fixedHeight = 460f;
            _labelStyle = new GUIStyle(HighLogic.Skin.label);
            _scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);
            _smButtonStyle = new GUIStyle(HighLogic.Skin.button);
            _smButtonStyle.fontSize = 10;
            _hasInitStyles = true;
        }

    }
}
