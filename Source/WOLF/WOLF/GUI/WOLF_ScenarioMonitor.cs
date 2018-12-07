using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WOLF
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class WOLF_ScenarioMonitor_Editor : WOLF_ScenarioMonitor { }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class WOLF_ScenarioMonitor_Flight : WOLF_ScenarioMonitor { }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class WOLF_ScenarioMonitor_SpaceCenter : WOLF_ScenarioMonitor { }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class WOLF_ScenarioMonitor_TStation : WOLF_ScenarioMonitor { }

    public class WOLF_ScenarioMonitor : MonoBehaviour
    {
        private ApplicationLauncherButton _wolfButton;
        private Rect _windowPosition = new Rect(300, 60, 700, 460);
        private GUIStyle _windowStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _scrollStyle;
        private GUIStyle _smButtonStyle;
        private Vector2 _scrollPos = Vector2.zero;
        private bool _hasInitStyles = false;

        public static bool renderDisplay = false;
        public int activeTab = 0;

        private string[] _tabLabels;
        private WOLF_ScenarioModule _wolfScenario;
        private IDepotRegistry _depotRegistry;
        private List<Window> _childWindows = new List<Window>();
        private Dictionary<string, string> _planetDisplayNames;

        /// <summary>
        /// Implementation of <see cref="MonoBehaviour"/>.Awake
        /// </summary>
        void Awake()
        {
            var texture = new Texture2D(36, 36, TextureFormat.RGBA32, false);
            var textureFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets/UI/WOLF.png");

            if (GameSettings.VERBOSE_DEBUG_LOG)
                Debug.Log("[WOLF] WOLF_ScenarioMonitor.Awake: Loading " + textureFile);

            texture.LoadImage(File.ReadAllBytes(textureFile));

            this._wolfButton = ApplicationLauncher.Instance.AddModApplication(GuiOn, GuiOff, null, null, null, null,
                ApplicationLauncher.AppScenes.ALWAYS, texture);
        }

        public void GuiOn()
        {
            renderDisplay = true;
        }

        public void GuiOff()
        {
            renderDisplay = false;
        }

        /// <summary>
        /// Implementation of <see cref="MonoBehaviour"/>.Start
        /// </summary>
        void Start()
        {
            _wolfScenario = HighLogic.FindObjectOfType<WOLF_ScenarioModule>();
            _depotRegistry = _wolfScenario.ServiceManager.GetService<IDepotRegistry>();

            if (!_hasInitStyles)
            {
                InitStyles();
            }

            // Setup tab labels
            _tabLabels = new[] { "WOLF Depots" };

            // Cache planet display names
            _planetDisplayNames = FlightGlobals.Bodies
                .Select(b => new { b.name, DisplayName = b.bodyName })
                .ToDictionary(b => b.name, b => b.DisplayName);
        }

        private void InitStyles()
        {
            _windowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                fixedWidth = 700,
                fixedHeight = 460f
            };
            _labelStyle = new GUIStyle(HighLogic.Skin.label);
            _scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);
            _smButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                fontSize = 10
            };
            _hasInitStyles = true;
        }

        /// <summary>
        /// Implementation of <see cref="MonoBehaviour"/>.OnGUI
        /// </summary>
        void OnGUI()
        {
            try
            {
                if (!renderDisplay)
                    return;

                // Draw main window
                _windowPosition = GUILayout.Window(12, _windowPosition, OnWindow, "WOLF Dashboard", _windowStyle);

                // Draw child windows
                foreach (var window in _childWindows)
                {
                    if (window.IsVisible())
                        window.DrawWindow();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[WOLF] ERROR in WOLF_ScenarioMonitor.OnGUI: " + ex.Message);
            }
        }

        /// <summary>
        /// Displays the main WOLF UI
        /// </summary>
        private void OnWindow(int windowId)
        {
            GUILayout.BeginVertical();

            // Show UI navigation tabs
            GUILayout.BeginHorizontal();
            activeTab = GUILayout.SelectionGrid(activeTab, _tabLabels, 6, _smButtonStyle);
            GUILayout.EndHorizontal();

            // Show the UI for the currently selected tab
            switch (activeTab)
            {
                case 0:
                    ShowDepots();
                    break;
            }

            GUILayout.EndVertical();

            // Make UI window draggable
            GUI.DragWindow();
        }

        /// <summary>
        /// Displays the UI for WOLF Depots
        /// </summary>
        private void ShowDepots()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, _scrollStyle, GUILayout.Width(600), GUILayout.Height(380));
            GUILayout.BeginVertical();

            try
            {
                // Display column headers
                GUILayout.BeginHorizontal();
                GUILayout.Label(" Body/Biome", _labelStyle, GUILayout.Width(160));
                GUILayout.Label("Resource", _labelStyle, GUILayout.Width(165));
                GUILayout.Label("Incoming", _labelStyle, GUILayout.Width(80));
                GUILayout.Label("Outgoing", _labelStyle, GUILayout.Width(80));
                GUILayout.Label("Available", _labelStyle, GUILayout.Width(80));
                GUILayout.EndHorizontal();

                var depotsByPlanet = _depotRegistry.GetDepots()
                    .GroupBy(d => d.Body)
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Select(d => d).OrderBy(d => d.Biome));

                foreach (var planet in depotsByPlanet)
                {
                    var planetDisplayName = _planetDisplayNames[planet.Key];

                    foreach (var depot in planet.Value)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(string.Format("<color=#FFFFFF>{0}:{1}</color>", planetDisplayName, depot.Biome), _labelStyle, GUILayout.Width(160));
                        GUILayout.EndHorizontal();

                        foreach (var resource in depot.GetResources())
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(string.Empty, _labelStyle, GUILayout.Width(160));
                            GUILayout.Label(resource.ResourceName, _labelStyle, GUILayout.Width(165));
                            GUILayout.Label(string.Format("<color=#FFD900>{0}</color>", resource.Incoming), _labelStyle, GUILayout.Width(80));
                            GUILayout.Label(string.Format("<color=#FFD900>{0}</color>", resource.Outgoing), _labelStyle, GUILayout.Width(80));
                            GUILayout.Label(string.Format("<color=#FFD900>{0}</color>", resource.Available), _labelStyle, GUILayout.Width(80));
                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[WOLF] ERROR in WOLF_ScenarioMonitor.ShowDepots: " + ex.StackTrace);
            }
            finally
            {
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// Implementation of <see cref="MonoBehaviour"/>.OnDestroy
        /// </summary>
        void OnDestroy()
        {
            if (_wolfButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(_wolfButton);
                _wolfButton = null;
            }
        }
    }
}
