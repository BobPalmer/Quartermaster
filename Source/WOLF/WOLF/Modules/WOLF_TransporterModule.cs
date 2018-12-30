using KSP.Localization;
using System;
using System.Text;
using UnityEngine;

namespace WOLF
{
    [KSPModule("Transporter")]
    public class WOLF_TransporterModule : WOLF_ConverterModule
    {
        private static string CANCEL_ROUTE_GUI_NAME = "#"; // "Cancel route";
        private static string CONNECT_TO_ORIGIN_GUI_NAME = "#"; // "Connect to origin depot";
        private static string CONNECT_TO_DESTINATION_GUI_NAME = "#"; // "Connect to destination depot";
        private static string INSUFFICIENT_PAYLOAD_MESSAGE = "#"; // "This vessel is too small to establish a transport route.";
        private static string INVALID_CONNECTION_MESSAGE = "#"; // "Destination must be in a different biome.";

        private static readonly int MINIMUM_PAYLOAD = 1;
        private static readonly int ROUTE_COST_MULTIPLIER = 3;

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

            var routeCost = CalculateRouteCost();  // TODO - deduct route cost from origin
            var routePayload = CalculateRoutePayload();
            if (routePayload < MINIMUM_PAYLOAD)
            {
                DisplayMessage(INSUFFICIENT_PAYLOAD_MESSAGE);
                return;
            }

            try
            {
                _registry.CreateRoute(OriginBody, OriginBiome, destinationBody, destinationBiome, routePayload);

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
                massDelta = 0;

            int massDeltaTonnes = Convert.ToInt32(massDelta);
            if (massDeltaTonnes < 0)
                massDeltaTonnes = 0;

            return massDeltaTonnes * ROUTE_COST_MULTIPLIER;
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
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_TRANSPORTER_INVALID_CONNECTION_MESSAGE", out string invalidConnectionMessage))
            {
                INVALID_CONNECTION_MESSAGE = invalidConnectionMessage;
            }

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
    }
}
