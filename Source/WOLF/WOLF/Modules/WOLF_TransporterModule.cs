using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WOLF
{
    [KSPModule("Transporter")]
    public class WOLF_TransporterModule : WOLF_ConverterModule
    {
        private static string CANCEL_ROUTE_GUI_NAME = "#"; // "Cancel route";
        private static string CONNECT_TO_ORIGIN_GUI_NAME = "#"; // "Connect to origin depot";
        private static string CONNECT_TO_DESTINATION_GUI_NAME = "#"; // "Connect to destination depot";
        private static string CURRENT_BIOME_GUI_NAME = "#autoLOC_USI_WOLF_CURRENT_BIOME_GUI_NAME"; // "Current biome";
        private static string INSUFFICIENT_PAYLOAD_MESSAGE = "#autoLOC_USI_WOLF_TRANSPORTER_INSUFFICIENT_PAYLOAD_MESSAGE"; // "This vessel is too small to establish a transport route.";
        private static string INSUFFICIENT_TRANSPORT_CREDITS_MESSAGE = "#autoLOC_USI_WOLF_TRANSPORTER_INSUFFICIENT_TRANSPORT_CREDITS_MESSAGE"; // "Origin depot needs an additional ({0}) TransportCredits to support this route."; 
        private static string INVALID_CONNECTION_MESSAGE = "#autoLOC_USI_WOLF_TRANSPORTER_INVALID_CONNECTION_MESSAGE"; // "Destination must be in a different biome.";
        private static string ROUTE_COST_GUI_NAME = "#autoLOC_USI_WOLF_TRANSPORTER_ROUTE_COST_GUI_NAME";  // "Route cost";

        private static readonly int MINIMUM_PAYLOAD = 1;
        private static readonly double ROUTE_COST_MULTIPLIER = 1d;

        private double _nextBiomeUpdate = 0d;

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Current biome")]
        public string CurrentBiome = string.Empty;

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Route cost")]
        public int RouteCost = 0;

        [KSPField(isPersistant = true)]
        public bool IsConnectedToOrigin = false;

        [KSPField(isPersistant = true)]
        public double StartingVesselMass = 0d;

        [KSPField(isPersistant = true)]
        public string OriginBody;

        [KSPField(isPersistant = true)]
        public string OriginBiome;

        [KSPEvent(guiActive = true, guiActiveEditor = false)]
        public void ResetRoute()
        {
            OriginBody = string.Empty;
            OriginBiome = string.Empty;
            StartingVesselMass = 0d;
            IsConnectedToOrigin = false;

            ToggleEventButtons();
        }

        [KSPEvent(guiActive = true, guiActiveEditor = false)]
        public void ConnectToOrigin()
        {
            // Check for issues that would prevent deployment
            var deployCheckResult = CanConnectToDepot();
            if (!string.IsNullOrEmpty(deployCheckResult))
            {
                DisplayMessage(deployCheckResult);
                return;
            }

            var vesselMass = Convert.ToInt32(vessel.totalMass);
            if (vesselMass < MINIMUM_PAYLOAD)
            {
                DisplayMessage(INSUFFICIENT_PAYLOAD_MESSAGE);
                return;
            }

            OriginBody = vessel.mainBody.name;
            OriginBiome = GetVesselBiome();
            StartingVesselMass = vessel.totalMass;
            IsConnectedToOrigin = true;

            ToggleEventButtons();
        }

        // We'll piggyback on the base class ConnectToDepotEvent to
        //   handle making the connection on the destination side
        protected override void ConnectToDepot()
        {
            // Check for issues that would prevent deployment
            var deployCheckResult = CanConnectToDepot();
            if (!string.IsNullOrEmpty(deployCheckResult))
            {
                DisplayMessage(deployCheckResult);
                return;
            }

            var destinationBody = vessel.mainBody.name;
            var destinationBiome = GetVesselBiome();
            if (destinationBody == OriginBody && destinationBiome == OriginBiome)
            {
                DisplayMessage(INVALID_CONNECTION_MESSAGE);
                return;
            }

            var routePayload = CalculateRoutePayload();
            if (routePayload < MINIMUM_PAYLOAD)
            {
                DisplayMessage(INSUFFICIENT_PAYLOAD_MESSAGE);
                return;
            }

            var originDepot = _registry.GetDepot(OriginBody, OriginBiome);
            var routeCost = CalculateRouteCost();
            if (routeCost > 0)
            {
                // Make sure origin depot has enough TransportCredits to support the route
                var originTransportCredits = originDepot.GetResources()
                    .Where(r => r.ResourceName == "TransportCredits")
                    .FirstOrDefault();
                if (originTransportCredits == null)
                {
                    DisplayMessage(string.Format(INSUFFICIENT_TRANSPORT_CREDITS_MESSAGE, routeCost));
                    return;
                }
                if (originTransportCredits.Available < routeCost)
                {
                    DisplayMessage(string.Format(INSUFFICIENT_TRANSPORT_CREDITS_MESSAGE, routeCost - originTransportCredits.Available));
                    return;
                }
            }

            try
            {
                _registry.CreateRoute(OriginBody, OriginBiome, destinationBody, destinationBiome, routePayload);
                if (routeCost > 0)
                {
                    originDepot.NegotiateConsumer(new Dictionary<string, int> { { "TransportCredits", routeCost } });
                }

                if (OriginBody == destinationBody)
                    DisplayMessage(string.Format(Messenger.SUCCESSFUL_DEPLOYMENT_MESSAGE, OriginBody));
                else
                    DisplayMessage(string.Format(Messenger.SUCCESSFUL_DEPLOYMENT_MESSAGE, OriginBody + " and " + destinationBody));

                ResetRoute();
            }
            catch (Exception ex)
            {
                DisplayMessage(ex.Message);
            }
        }

        private int CalculateRouteCost()
        {
            var endingMass = vessel.totalMass;
            var massDelta = StartingVesselMass - vessel.totalMass;
            if (massDelta < 0)
                return 0;

            var routeCost = massDelta * ROUTE_COST_MULTIPLIER;
            return Math.Max(Convert.ToInt32(routeCost), 0);
        }

        private int CalculateRoutePayload()
        {
            return Convert.ToInt32(vessel.totalMass);
        }

        public override string GetInfo()
        {
            return PartInfo;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_TRANSPORTER_INSUFFICIENT_PAYLOAD_MESSAGE", out string payloadMessage))
            {
                INSUFFICIENT_PAYLOAD_MESSAGE = payloadMessage;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_TRANSPORTER_INSUFFICIENT_TRANSPORT_CREDITS_MESSAGE", out string tCredsMessage))
            {
                INSUFFICIENT_TRANSPORT_CREDITS_MESSAGE = tCredsMessage;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_TRANSPORTER_INVALID_CONNECTION_MESSAGE", out string invalidConnectionMessage))
            {
                INVALID_CONNECTION_MESSAGE = invalidConnectionMessage;
            }

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_CURRENT_BIOME_GUI_NAME", out string currentBiomeGuiName))
            {
                CURRENT_BIOME_GUI_NAME = currentBiomeGuiName;
            }
            Fields["CurrentBiome"].guiName = CURRENT_BIOME_GUI_NAME;

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_TRANSPORTER_ROUTE_COST_GUI_NAME", out string routeCostGuiName))
            {
                ROUTE_COST_GUI_NAME = routeCostGuiName;
            }
            Fields["RouteCost"].guiName = ROUTE_COST_GUI_NAME;

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_TRANSPORTER_CANCEL_ROUTE_GUI_NAME", out string cancelRouteGuiName))
            {
                CANCEL_ROUTE_GUI_NAME = cancelRouteGuiName;
            }
            Events["ResetRoute"].guiName = CANCEL_ROUTE_GUI_NAME;

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_TRANSPORTER_CONNECT_TO_ORIGIN_GUI_NAME", out string originGuiName))
            {
                CONNECT_TO_ORIGIN_GUI_NAME = originGuiName;
            }
            Events["ConnectToOrigin"].guiName = CONNECT_TO_ORIGIN_GUI_NAME;

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_TRANSPORTER_CONNECT_TO_DESTINATION_GUI_NAME", out string destinationGuiName))
            {
                CONNECT_TO_DESTINATION_GUI_NAME = destinationGuiName;
            }
            Events["ConnectToDepotEvent"].guiName = CONNECT_TO_DESTINATION_GUI_NAME;

            ToggleEventButtons();
        }

        private void ToggleEventButtons()
        {
            Events["ResetRoute"].active = IsConnectedToOrigin;
            Events["ConnectToDepotEvent"].active = IsConnectedToOrigin;
            Events["ConnectToOrigin"].active = !IsConnectedToOrigin;

            MonoUtilities.RefreshContextWindows(part);
        }

        void Update()
        {
            // Display current biome and route cost in PAW
            if (HighLogic.LoadedSceneIsFlight)
            {
                var now = Planetarium.GetUniversalTime();
                if (now >= _nextBiomeUpdate)
                {
                    _nextBiomeUpdate = now + 1d;  // wait one second between biome updates
                    CurrentBiome = GetVesselBiome();
                    RouteCost = CalculateRouteCost();
                }
            }
        }
    }
}
