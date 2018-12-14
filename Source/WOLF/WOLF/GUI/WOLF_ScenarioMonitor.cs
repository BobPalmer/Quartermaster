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
        private IRegistryCollection _wolfRegistry;
        private readonly List<Window> _childWindows = new List<Window>();
        private WOLF_RouteMonitor _routeMonitor;

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

            _wolfButton = ApplicationLauncher.Instance.AddModApplication(GuiOn, GuiOff, null, null, null, null,
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
            _wolfScenario = FindObjectOfType<WOLF_ScenarioModule>();
            _wolfRegistry = _wolfScenario.ServiceManager.GetService<IRegistryCollection>();
            _routeMonitor = _wolfScenario.ServiceManager.GetService<WOLF_RouteMonitor>();

            if (!_hasInitStyles)
            {
                InitStyles();
            }

            // Setup tab labels
            _tabLabels = new[] { "Depots", "Harvestable Resources", "Routes" };

            // Setup child windows
            if (!_childWindows.Contains(_routeMonitor.ManageTransfersGui))
                _childWindows.Add(_routeMonitor.ManageTransfersGui);
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
            var newActiveTab = GUILayout.SelectionGrid(activeTab, _tabLabels, 3, _smButtonStyle);
            if (newActiveTab != activeTab)
            {
                // If a new tab was selected, hide the route transfer manager window
                _routeMonitor.ManageTransfersGui.SetVisible(false);
                activeTab = newActiveTab;
            }
            GUILayout.EndHorizontal();

            // Show the UI for the currently selected tab
            switch (activeTab)
            {
                case 0:
                    ShowDepots();
                    break;
                case 1:
                    ShowHarvestableResources();
                    break;
                case 2:
                    _scrollPos = _routeMonitor.DrawWindow(_scrollPos);
                    break;
            }

            GUILayout.EndVertical();

            // Make UI window draggable
            GUI.DragWindow();
        }

        private void ShowDepots()
        {
            ShowResources(r => !r.ResourceName.EndsWith(WOLF_DepotModule.HARVESTABLE_RESOURCE_SUFFIX));
        }

        private void ShowHarvestableResources()
        {
            ShowResources(r => r.ResourceName.EndsWith(WOLF_DepotModule.HARVESTABLE_RESOURCE_SUFFIX), "Abundance", "Harvested");
        }

        /// <summary>
        /// Displays the UI for WOLF
        /// </summary>
        private void ShowResources(Func<IResourceStream, bool> filter, string incomingHeaderLabel = "Incoming", string outgoingHeaderLabel = "Outgoing", string availableHeaderLabel = "Available")
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, _scrollStyle, GUILayout.Width(680), GUILayout.Height(380));
            GUILayout.BeginVertical();

            try
            {
                // Display column headers
                GUILayout.BeginHorizontal();
                GUILayout.Label("Body/Biome", _labelStyle, GUILayout.Width(160));
                GUILayout.Label("Resource", _labelStyle, GUILayout.Width(165));
                GUILayout.Label(incomingHeaderLabel, _labelStyle, GUILayout.Width(80));
                GUILayout.Label(outgoingHeaderLabel, _labelStyle, GUILayout.Width(80));
                GUILayout.Label(availableHeaderLabel, _labelStyle, GUILayout.Width(80));
                GUILayout.EndHorizontal();

                var depotsByPlanet = _wolfRegistry.GetDepots()
                    .GroupBy(d => d.Body)
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Select(d => d).OrderBy(d => d.Biome));

                foreach (var planet in depotsByPlanet)
                {
                    var planetDisplayName = planet.Key;

                    foreach (var depot in planet.Value)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(string.Format("<color=#FFFFFF>{0}:{1}</color>", planetDisplayName, depot.Biome), _labelStyle, GUILayout.Width(160));
                        GUILayout.EndHorizontal();

                        var resources = depot.GetResources()
                            .Where(filter)
                            .OrderBy(r => r.ResourceName);

                        foreach (var resource in resources)
                        {
                            var resourceName = resource.ResourceName.EndsWith(WOLF_DepotModule.HARVESTABLE_RESOURCE_SUFFIX)
                                ? resource.ResourceName.Remove(resource.ResourceName.Length - WOLF_DepotModule.HARVESTABLE_RESOURCE_SUFFIX.Length)
                                : resource.ResourceName;

                            GUILayout.BeginHorizontal();
                            GUILayout.Label(string.Empty, _labelStyle, GUILayout.Width(160));
                            GUILayout.Label(resourceName, _labelStyle, GUILayout.Width(165));
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
