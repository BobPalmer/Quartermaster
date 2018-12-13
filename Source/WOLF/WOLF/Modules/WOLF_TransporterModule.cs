using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WOLF
{
    public class WOLF_TransporterModule : WOLF_ConverterModule
    {
        private static readonly string INSUFFICIENT_PAYLOAD_MESSAGE = "This vessel is too small to establish a transport route.";
        private static readonly string INVALID_CONNECTION_MESSAGE = "Destination must be in a different biome.";
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

        [KSPEvent(guiName = "Cancel route", guiActive = true, guiActiveEditor = false)]
        public void ResetRoute()
        {
            OriginBody = string.Empty;
            OriginBiome = string.Empty;
            StartingVesselMass = 0d;
            IsConnectedToOrigin = false;

            ToggleEventButtons();
        }

        [KSPEvent(guiName = "Connect to origin depot", guiActive = true, guiActiveEditor = false)]
        public void ConnectToOrigin()
        {
            // Check for issues that would prevent deployment
            var deployCheckResult = CanConnectToDepot();
            if (!string.IsNullOrEmpty(deployCheckResult))
            {
                DisplayMessage(deployCheckResult);
                return;
            }

            var vesselMass = Convert.ToInt32(vessel.totalMass / 1000d);
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

            var body = vessel.mainBody.name;
            var biome = GetVesselBiome();
            if (body == OriginBody && biome == OriginBiome)
            {
                DisplayMessage(INVALID_CONNECTION_MESSAGE);
                return;
            }

            var routeCost = CalculateRouteCost();
            var routePayload = CalculateRoutePayload();
            if (routePayload < MINIMUM_PAYLOAD)
            {
                DisplayMessage(INSUFFICIENT_PAYLOAD_MESSAGE);
                return;
            }

            // TODO - whatever we need to do to register the route with wolf

            DisplayMessage(Messenger.SUCCESSFUL_DEPLOYMENT_MESSAGE);
            ResetRoute();
        }

        private int CalculateRouteCost()
        {
            var endingMass = vessel.totalMass;
            var massDelta = StartingVesselMass - vessel.totalMass;
            if (massDelta < 0)
                massDelta = 0;

            int massDeltaTonnes = Convert.ToInt32(massDelta / 1000d);
            if (massDeltaTonnes < 0)
                massDeltaTonnes = 0;

            return massDeltaTonnes * ROUTE_COST_MULTIPLIER;
        }

        private int CalculateRoutePayload()
        {
            return Convert.ToInt32(vessel.totalMass / 1000d);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Events["ConnectToDepotEvent"].guiName = "Connect to destination depot";
            ToggleEventButtons();
        }

        private void ToggleEventButtons()
        {
            Events["CancelRoute"].active = IsConnectedToOrigin;
            Events["ConnectToDepotEvent"].active = IsConnectedToOrigin;
            Events["ConnectToOrigin"].active = !IsConnectedToOrigin;

            MonoUtilities.RefreshContextWindows(part);
        }
    }
}
