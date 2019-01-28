using System;
using System.Linq;
using UnityEngine;

namespace WOLF
{
    public class WOLF_RouteMonitor
    {
        private readonly IRouteRegistry _routeRegistry;
        private GUIStyle _windowStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _scrollStyle;
        private GUIStyle _smButtonStyle;

        public WOLF_GuiManageTransfers ManageTransfersGui { get; private set; }

        public WOLF_RouteMonitor(IRegistryCollection routeRegistry)
        {
            _routeRegistry = routeRegistry;

            InitStyles();

            ManageTransfersGui = new WOLF_GuiManageTransfers(routeRegistry);
        }

        public Vector2 DrawWindow(Vector2 scrollPosition)
        {
            var newScrollPosition = GUILayout.BeginScrollView(scrollPosition, _scrollStyle, GUILayout.Width(680), GUILayout.Height(830));
            GUILayout.BeginVertical();

            try
            {
                // Display manage transfers window
                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Empty, UIHelper.labelStyle, GUILayout.Width(520));
                if (GUILayout.Button("Manage Transfers", UIHelper.buttonStyle, GUILayout.Width(120)))
                {
                    ToggleTransfersWindow();
                }
                GUILayout.EndHorizontal();

                // Display column headers
                GUILayout.BeginHorizontal();
                GUILayout.Label("Origin", _labelStyle, GUILayout.Width(160));
                GUILayout.Label("Destination", _labelStyle, GUILayout.Width(160));
                GUILayout.Label("Cargo Space", _labelStyle, GUILayout.Width(90));
                GUILayout.Label("Resource", _labelStyle, GUILayout.Width(160));
                GUILayout.Label("Quantity", _labelStyle, GUILayout.Width(70));
                GUILayout.EndHorizontal();

                var routes = _routeRegistry.GetRoutes();
                if (routes != null && routes.Count > 0)
                {
                    var orderedRoutes = routes
                        .OrderBy(r => r.OriginBody)
                            .ThenBy(r => r.OriginBiome)
                            .ThenBy(r => r.DestinationBody)
                            .ThenBy(r => r.DestinationBiome);

                    foreach (var route in orderedRoutes)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(string.Format("<color=#FFFFFF>{0}:{1}</color>", route.OriginBody, route.OriginBiome), _labelStyle, GUILayout.Width(160));
                        GUILayout.Label(string.Format("<color=#FFFFFF>{0}:{1}</color>", route.DestinationBody, route.DestinationBiome), _labelStyle, GUILayout.Width(160));
                        GUILayout.Label(string.Format("<color=#FFFFFF>{0}</color>", route.Payload), _labelStyle, GUILayout.Width(90));
                        GUILayout.EndHorizontal();

                        var resources = route.GetResources()
                            .OrderBy(r => r.Key);

                        foreach (var resource in resources)
                        {
                            var resourceName = resource.Key;

                            GUILayout.BeginHorizontal();
                            GUILayout.Label(string.Empty, _labelStyle, GUILayout.Width(160));
                            GUILayout.Label(string.Empty, _labelStyle, GUILayout.Width(160));
                            GUILayout.Label(string.Empty, _labelStyle, GUILayout.Width(90));
                            GUILayout.Label(resourceName, _labelStyle, GUILayout.Width(160));
                            GUILayout.Label(string.Format("<color=#FFD900>{0}</color>", resource.Value), _labelStyle, GUILayout.Width(70));
                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("[WOLF] ERROR in {0}: {1} Stack Trace: {2}", GetType().Name, ex.Message, ex.StackTrace));
            }
            finally
            {
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }

            return newScrollPosition;
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
        }

        private void ToggleTransfersWindow()
        {
            if (ManageTransfersGui.IsVisible())
                ManageTransfersGui.ResetAndClose();
            else
                ManageTransfersGui.SetVisible(true);
        }
    }
}
