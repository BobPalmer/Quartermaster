using System;
using UnityEngine;
using USITools;

namespace WOLF
{
    public class KolonyInventory
    {
        private GUIStyle _labelStyle;
        private GUIStyle _texStyle;
        private GUIStyle _scrollStyle;
        private Vector2 _scrollPos = Vector2.zero;

        private const int LOCAL_LOGISTICS_RANGE = 150;
        private List<ResourceSummary> _resourceList;
        private double LastUpdate;
        private const double UpdateFrequency = 0.5d;
        public string vesselId;

        private Texture2D _txLtGray;
        private Texture2D _txDkGray;
        private Texture2D _txLtGreen;
        private Texture2D _txDkGreen;
        private Texture2D _txLtRed;
        private Texture2D _txDkRed;

        public KolonyInventory()
        {
            _labelStyle = new GUIStyle(HighLogic.Skin.label);
            _texStyle = _labelStyle;
            _texStyle.margin = new RectOffset(0, 0, 0, 0);
            _texStyle.padding = new RectOffset(0, 0, 0, 0);
            _scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);
            _resourceList = new List<ResourceSummary>();
            LastUpdate = Planetarium.GetUniversalTime();

            _txLtGray = LoadTex(new Color(.5f, .5f, 0.5f));
            _txDkGray = LoadTex(new Color(0.2f, 0.2f, 0.2f)); ;
            _txLtGreen = LoadTex(new Color(0.25f, 0.75f, 0.25f)); ;
            _txDkGreen = LoadTex(new Color(0f, 0.25f, 0f)); ;
            _txLtRed = LoadTex(new Color(0.75f, 0.25f, 0.25f)); ;
            _txDkRed = LoadTex(new Color(0.25f, 0f, 0f)); ;
        }

        private void RefreshResourceList()
        {
            if (vesselId != FlightGlobals.ActiveVessel.id.ToString())
            {
                vesselId = FlightGlobals.ActiveVessel.id.ToString();
                _resourceList.Clear();
            }
            var delta = Planetarium.GetUniversalTime() - LastUpdate;
            LastUpdate = Planetarium.GetUniversalTime();

            if (delta > 5)
                return;

            var vList = LogisticsTools.GetNearbyVessels(LOCAL_LOGISTICS_RANGE, false, FlightGlobals.ActiveVessel, true);
            vList.Add(FlightGlobals.ActiveVessel);

            var vCount = vList.Count;
            for (int i = 0; i < vCount; ++i)
            {
                var thisVessel = vList[i];
                var pCount = thisVessel.Parts.Count;
                for (int p = 0; p < pCount; ++p)
                {
                    var thisPart = thisVessel.Parts[p];
                    var rCount = thisPart.Resources.Count;
                    for (int r = 0; r < rCount; ++r)
                    {
                        var res = thisPart.Resources[r];
                        if (res.resourceName == "ElectricCharge" || !res.isVisible)
                            continue;

                        var found = false;
                        var rlCount = _resourceList.Count();
                        for (int c = 0; c < rlCount; ++c)
                        {
                            var thisRes = _resourceList[c];
                            if (thisRes.ResourceName == res.resourceName)
                            {
                                found = true;
                                var netChange = res.maxAmount - thisRes.MaxAmount;
                                netChange += res.amount - thisRes.CurrentAmout;
                                netChange /= delta;
                                thisRes.Changes.Add(netChange);
                                thisRes.CurrentAmout = res.amount;
                                thisRes.MaxAmount = res.maxAmount;
                                break;
                            }
                        }
                        if (!found)
                        {
                            _resourceList.Add(new ResourceSummary
                            {
                                ResourceName = res.resourceName,
                                MaxAmount = res.maxAmount,
                                CurrentAmout = res.amount,
                                Changes = new List<double>()
                            });
                        }
                    }
                }
            }
        }

        public void Display()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                GUILayout.Label("Kolony Inventory only available on active vessels.", _labelStyle, GUILayout.Width(400));
                return;
            }

            if (LastUpdate + UpdateFrequency < Planetarium.GetUniversalTime())
            {
                RefreshResourceList();
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, _scrollStyle, GUILayout.Width(680), GUILayout.Height(380));
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUIButton.LayoutButton("Reset"))
            {
                _resourceList = new List<ResourceSummary>();
            }
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("<color=#FFFFFF>Resource</color>", _labelStyle, GUILayout.Width(180));
            GUILayout.Label("<color=#FFFFFF>Inventory</color>", _labelStyle, GUILayout.Width(190));
            GUILayout.Label("<color=#FFFFFF>Rate</color>", _labelStyle, GUILayout.Width(80));
            GUILayout.Label("<color=#FFFFFF>Supply</color>", _labelStyle, GUILayout.Width(150));
            GUILayout.EndHorizontal();

            var count = _resourceList.Count;
            for (int i = 0; i < count; ++i)
            {
                var r = _resourceList[i];
                var netChange = GetNetChange(r);
                var rowCol = "d6d6d6";
                if (netChange > ResourceUtilities.FLOAT_TOLERANCE * 100)
                {
                    rowCol = "b1f700";
                }
                else if (netChange < -(ResourceUtilities.FLOAT_TOLERANCE * 100))
                {
                    rowCol = "f7da00";
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("<color=#" + rowCol + ">" + r.ResourceName + "</color>", _labelStyle, GUILayout.Width(160));
                GUILayout.Label("", _labelStyle, GUILayout.Width(10));
                int Percent = (int)(r.CurrentAmout / r.MaxAmount * 100d);

                if (netChange > ResourceUtilities.FLOAT_TOLERANCE * 100)
                {
                    _texStyle.normal.background = _txLtGreen;
                    GUILayout.Label("", _texStyle, GUILayout.Width(Percent));
                    _texStyle.normal.background = _txDkGreen;
                    GUILayout.Label("", _texStyle, GUILayout.Width(100 - Percent));
                }
                else if (netChange < -(ResourceUtilities.FLOAT_TOLERANCE * 100))
                {
                    _texStyle.normal.background = _txLtRed;
                    GUILayout.Label("", _texStyle, GUILayout.Width(Percent));
                    _texStyle.normal.background = _txDkRed;
                    GUILayout.Label("", _texStyle, GUILayout.Width(100 - Percent));
                }
                else
                {
                    _texStyle.normal.background = _txLtGray;
                    GUILayout.Label("", _texStyle, GUILayout.Width(Percent));
                    _texStyle.normal.background = _txDkGray;
                    GUILayout.Label("", _texStyle, GUILayout.Width(100 - Percent));
                }
                _texStyle.normal.background = null;

                GUILayout.Label("", _labelStyle, GUILayout.Width(5));
                GUILayout.Label(String.Format("<color=#{0}>[{1:0}%]</color>", rowCol, r.CurrentAmout / r.MaxAmount * 100d), _labelStyle, GUILayout.Width(50));
                GUILayout.Label("", _labelStyle, GUILayout.Width(10));


                var sign = "";
                if (netChange < 0)
                {
                    sign = "-";
                }
                else if (netChange > 0)
                {
                    sign = "+";
                }

                GUILayout.Label(String.Format("<color=#{0}>{1}</color>", rowCol, sign), _labelStyle, GUILayout.Width(10));
                GUILayout.Label(String.Format("<color=#{0}>{1}</color>", rowCol, FormatChange(Math.Abs(netChange))), _labelStyle, GUILayout.Width(75));
                GUILayout.Label("", _labelStyle, GUILayout.Width(10));
                if (Math.Abs(netChange) < ResourceUtilities.FLOAT_TOLERANCE * 100)
                    GUILayout.Label("<color=#" + rowCol + ">---</color>", _labelStyle, GUILayout.Width(150));
                else
                    GUILayout.Label("<color=#" + rowCol + ">" + DurationDisplay(Math.Abs(r.CurrentAmout / netChange)) + "</color>", _labelStyle, GUILayout.Width(150));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        public string FormatChange(double perSec)
        {
            double secsPerMinute = 60d;
            double secsPerHour = secsPerMinute * 60d;
            double secsPerDay = SecondsPerDay();

            if (perSec > SecondsPerDay())
            {
                return String.Format("{0:0.0000}/day", perSec / secsPerDay);
            }
            else if (perSec > secsPerHour)
            {
                return String.Format("{0:0.0000}/hr", perSec / secsPerHour);
            }
            else if (perSec > secsPerMinute)
            {
                return String.Format("{0:0.0000}/min", perSec / secsPerMinute);
            }
            else
            {
                return String.Format("{0:0.0000}/sec", perSec);
            }
        }


        private double GetNetChange(ResourceSummary res)
        {
            double avg = 0d;
            double tot = 0d;
            double count = res.Changes.Count;
            if (count < 2)
                return 0;

            for (int i = 0; i < count; ++i)
            {
                tot += res.Changes[i];
            }
            avg = tot / count;
            if (count > 5)
                res.Changes.RemoveAt(0);
            return avg;
        }

        private Texture2D LoadTex(Color c)
        {
            Texture2D result = new Texture2D(1, 1);
            result.SetPixels(new[] { c });
            result.Apply();
            return result;
        }

        private Texture MakeTex(double maxAmount, double curAmount, double change)
        {
            var width = 100;
            var height = 15;

            Color bg = new Color(0.2f, 0.2f, 0.2f);
            Color fg = new Color(.5f, .5f, 0.5f);

            if (change < -(ResourceUtilities.FLOAT_TOLERANCE * 100))
            {
                bg = new Color(0.25f, 0f, 0f);
                fg = new Color(0.75f, 0.25f, 0.25f);
            }
            else if (change > ResourceUtilities.FLOAT_TOLERANCE * 100)
            {
                bg = new Color(0f, 0.25f, 0f);
                fg = new Color(0.25f, 0.75f, 0.25f);
            }

            var pix = new Color[width * height];
            var percent = curAmount / maxAmount;
            for (int i = 0; i < pix.Length; i++)
            {
                var col = i % width;
                if (col >= (width * percent))
                {
                    pix[i] = bg;
                }
                else
                {
                    pix[i] = fg;
                }
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public double SecondsPerDay()
        {
            return GameSettings.KERBIN_TIME ? 21600d : 86400d;
        }

        public double SecondsPerYear()
        {
            return GameSettings.KERBIN_TIME ? SecondsPerDay() * 426d : SecondsPerDay() * 365d;
        }

        public string DurationDisplay(double s)
        {
            const double secsPerMinute = 60d;
            const double secsPerHour = secsPerMinute * 60d;
            double secsPerDay = SecondsPerDay();
            double secsPerYear = SecondsPerYear();

            double y = Math.Floor(s / secsPerYear);
            s = s - (y * secsPerYear);
            double d = Math.Floor(s / secsPerDay);
            s = s - (d * secsPerDay);
            double h = Math.Floor(s / secsPerHour);
            s = s - (h * secsPerHour);
            double m = Math.Floor(s / secsPerMinute);
            s = s - (m * secsPerMinute);

            return string.Format("{0:0}y:{1:0}d:{2:00}h:{3:00}m:{4:00}s", y, d, h, m, s);
        }

        private class ResourceSummary
        {
            public string ResourceName { get; set; }
            public double CurrentAmout { get; set; }
            public double MaxAmount { get; set; }
            public List<double> Changes { get; set; }
        }

    }
}