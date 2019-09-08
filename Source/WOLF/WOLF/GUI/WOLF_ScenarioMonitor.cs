﻿using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WOLF
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class WOLF_ScenarioMonitor_Editor : WOLF_ScenarioMonitor
    {
        public WOLF_ScenarioMonitor_Editor()
        {
            _isEditor = true;
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class WOLF_ScenarioMonitor_Flight : WOLF_ScenarioMonitor { }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class WOLF_ScenarioMonitor_SpaceCenter : WOLF_ScenarioMonitor { }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class WOLF_ScenarioMonitor_TStation : WOLF_ScenarioMonitor { }

    public class WOLF_ScenarioMonitor : MonoBehaviour
    {
        private readonly List<Window> _childWindows = new List<Window>();
        private readonly Dictionary<IDepot, bool> _depotDisplayStatus
            = new Dictionary<IDepot, bool>();
        private bool _displayAllToggle = false;
        private bool _hasInitStyles = false;
        protected bool _isEditor = false;
        private GUIStyle _labelStyle;
        private WOLF_PlanningMonitor _planningMonitor;
        private WOLF_RouteMonitor _routeMonitor;
        private Vector2 _scrollPos = Vector2.zero;
        private GUIStyle _scrollStyle;
        private GUIStyle _smButtonStyle;
        private string[] _tabLabels;
        private Rect _windowPosition = new Rect(300, 60, 700, 460);
        private GUIStyle _windowStyle;
        private ApplicationLauncherButton _wolfButton;
        private IRegistryCollection _wolfRegistry;
        private WOLF_ScenarioModule _wolfScenario;

        public static bool showGui = false;
        public int activeTab = 0;

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
            showGui = true;
        }

        public void GuiOff()
        {
            showGui = false;
        }

        /// <summary>
        /// Implementation of <see cref="MonoBehaviour"/>.Start
        /// </summary>
        void Start()
        {
            _wolfScenario = FindObjectOfType<WOLF_ScenarioModule>();
            _wolfRegistry = _wolfScenario.ServiceManager.GetService<IRegistryCollection>();
            _routeMonitor = _wolfScenario.ServiceManager.GetService<WOLF_RouteMonitor>();
            _planningMonitor = _wolfScenario.ServiceManager.GetService<WOLF_PlanningMonitor>();

            if (!_hasInitStyles)
            {
                InitStyles();
            }

            // Setup tab labels
            _tabLabels = new[] { "Depots", "Harvestable Resources", "Routes" };
            if (_isEditor)
                _tabLabels = _tabLabels.Concat(new string[] { "Planner" }).ToArray();

            // Setup child windows
            if (!_childWindows.Contains(_routeMonitor.ManageTransfersGui))
                _childWindows.Add(_routeMonitor.ManageTransfersGui);

            // Check for missing hoppers
            var hoppers = _wolfRegistry.GetHoppers();
            if (hoppers.Count > 0)
            {
                var hopperIds = GetHopperIds();
                foreach (var hopper in hoppers)
                {
                    if (!hopperIds.Contains(hopper.Id))
                    {
                        var resourcesToRelease = new Dictionary<string, int>();
                        foreach (var input in hopper.Recipe.InputIngredients)
                        {
                            resourcesToRelease.Add(input.Key, input.Value * -1);
                        }

                        var result = hopper.Depot.NegotiateConsumer(resourcesToRelease);
                        if (result is FailedNegotiationResult)
                        {
                            Debug.LogError("[WOLF] Could not release hopper resources back to depot.");
                        }

                        _wolfRegistry.RemoveHopper(hopper.Id);
                    }
                }
            }
        }

        private List<string> GetHopperIds()
        {
            var vessels = FlightGlobals.Vessels;
            List<string> hopperIds = new List<string>();
            foreach (var vessel in vessels)
            {
                if (vessel.loaded)
                {
                    var modules = vessel.FindPartModulesImplementing<WOLF_HopperModule>();
                    var ids = modules.Select(m => m.HopperId);
                    hopperIds.AddRange(ids);
                }
                else
                {
                    foreach (var part in vessel.protoVessel.protoPartSnapshots)
                    {
                        foreach (var module in part.modules)
                        {
                            if (module.moduleName == "WOLF_HopperModule")
                            {
                                var id = module.moduleValues.GetValue("HopperId") ?? string.Empty;
                                if (!string.IsNullOrEmpty(id))
                                {
                                    hopperIds.Add(id);
                                }
                            }
                        }
                    }
                }
            }

            return hopperIds;
        }

        private void InitStyles()
        {
            _windowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                fixedWidth = 700f,
                fixedHeight = 900f
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
                if (!showGui)
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
        /// Displays the outer WOLF UI
        /// </summary>
        private void OnWindow(int windowId)
        {
            GUILayout.BeginVertical();

            // Show UI navigation tabs
            GUILayout.BeginHorizontal();
            var newActiveTab = GUILayout.SelectionGrid(activeTab, _tabLabels, _tabLabels.Length, _smButtonStyle);
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
                case 3:
                    _scrollPos = _planningMonitor.DrawWindow(_scrollPos);
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
        /// Displays the inner WOLF UI depot and harvestable windows
        /// </summary>
        private void ShowResources(Func<IResourceStream, bool> resourceFilter, string incomingHeaderLabel = "Incoming", string outgoingHeaderLabel = "Outgoing", string availableHeaderLabel = "Available")
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, _scrollStyle, GUILayout.Width(680), GUILayout.Height(830));
            GUILayout.BeginVertical();

            try
            {
                // Display column headers
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(_displayAllToggle ? "-" : "+", UIHelper.buttonStyle, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    _displayAllToggle = !_displayAllToggle;
                    var depotKeys = _depotDisplayStatus.Keys.ToArray();
                    for (int i = 0; i < depotKeys.Length; i++)
                    {
                        var key = depotKeys[i];
                        _depotDisplayStatus[key] = _displayAllToggle;
                    }
                }
                GUILayout.Label("Body/Biome", _labelStyle, GUILayout.Width(150));
                GUILayout.Label("Resource", _labelStyle, GUILayout.Width(155));
                GUILayout.Label(incomingHeaderLabel, _labelStyle, GUILayout.Width(80));
                GUILayout.Label(outgoingHeaderLabel, _labelStyle, GUILayout.Width(80));
                GUILayout.Label(availableHeaderLabel, _labelStyle, GUILayout.Width(80));
                GUILayout.EndHorizontal();

                var depots = _wolfRegistry.GetDepots();
                if (depots != null && depots.Any())
                {
                    var depotsByPlanet = depots
                        .GroupBy(d => d.Body)
                        .OrderBy(g => g.Key)
                        .ToDictionary(g => g.Key, g => g.Select(d => d).OrderBy(d => d.Biome));

                    foreach (var planet in depotsByPlanet)
                    {
                        var planetDisplayName = planet.Key;

                        foreach (var depot in planet.Value)
                        {
                            if (!_depotDisplayStatus.ContainsKey(depot))
                            {
                                _depotDisplayStatus.Add(depot, false);
                            }

                            var resources = depot.GetResources()
                                .Where(resourceFilter)
                                .OrderBy(r => r.ResourceName);

                            if (depot.IsEstablished || resources.Any())
                            {
                                var visible = _depotDisplayStatus[depot];
                                GUILayout.BeginHorizontal();
                                if (GUILayout.Button(visible ? "-" : "+", UIHelper.buttonStyle, GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    _depotDisplayStatus[depot] = !_depotDisplayStatus[depot];
                                    visible = _depotDisplayStatus[depot];
                                    _displayAllToggle = visible;
                                }
                                GUILayout.Label(string.Format("<color=#FFFFFF>{0}:{1}</color>", planetDisplayName, depot.Biome), _labelStyle, GUILayout.Width(160));
                                GUILayout.EndHorizontal();

                                if (visible)
                                {
                                    foreach (var resource in resources)
                                    {
                                        var resourceName = resource.ResourceName.EndsWith(WOLF_DepotModule.HARVESTABLE_RESOURCE_SUFFIX)
                                            ? resource.ResourceName.Remove(resource.ResourceName.Length - WOLF_DepotModule.HARVESTABLE_RESOURCE_SUFFIX.Length)
                                            : resource.ResourceName;

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label(string.Empty, _labelStyle, GUILayout.Width(170));
                                        GUILayout.Label(resourceName, _labelStyle, GUILayout.Width(155));
                                        GUILayout.Label(string.Format("<color=#FFD900>{0}</color>", resource.Incoming), _labelStyle, GUILayout.Width(80));
                                        GUILayout.Label(string.Format("<color=#FFD900>{0}</color>", resource.Outgoing), _labelStyle, GUILayout.Width(80));
                                        GUILayout.Label(string.Format("<color=#FFD900>{0}</color>", resource.Available), _labelStyle, GUILayout.Width(80));
                                        GUILayout.EndHorizontal();
                                    }
                                }
                            }
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
