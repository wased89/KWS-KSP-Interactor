using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using GeodesicGrid;
using Weather;
using KWSKSPButtToucher;
using KerbalWeatherSimulator;

namespace GUIUtils
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class InFlightGUI : MonoBehaviour
    {
        private Rect MainFlightGUI = new Rect(100,100,175,50);
        private Rect WeatherDataGUI = new Rect(100,100,350,500);


        private bool showGUI = true;
        private bool showWeatherDataGUI = false;
        
        private int WeatherDataGUIID;
        private int MainFlightGUIID;


        void Awake()
        {
            MainFlightGUIID = Guid.NewGuid().GetHashCode();
            WeatherDataGUIID = Guid.NewGuid().GetHashCode();
        }
        
        void OnGUI()
        {
            if(showGUI)
            {
                MainFlightGUI = GUILayout.Window(MainFlightGUIID, MainFlightGUI, OnWindow, "Main GUI~");
                if (showWeatherDataGUI)
                {
                    GUI.Window(WeatherDataGUIID, WeatherDataGUI, WeatherDataUI, "WeatherData~");
                }
            }
            
        }

        void OnWindow(int windowID)
        {
            if(showGUI)
            {
                showWeatherDataGUI = GUILayout.Toggle(showWeatherDataGUI, "Weather Data~");
                GUI.DragWindow();
            }
            
        }

        int i = 0;
        int AltLayer = 0;
        void WeatherDataUI(int windowID)
        {
            
            CelestialBody body = FlightGlobals.currentMainBody;
            PlanetSimulator pSim = KWSKSPButtToucher.KSPHeadMaster.PlanetMap[body];
            Cell cell = KSPWeatherFunctions.getCellAtLocation(body, FlightGlobals.ActiveVessel.GetWorldPos3D());

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Body Up")) {if( i < KWSKSPButtToucher.KSPHeadMaster.PlanetMap.Keys.Count - 1) i++; };
            if (GUILayout.Button("Body Down")) { if (i > 0) i--; };
            GUILayout.EndHorizontal();

            CelestialBody testBody = KWSKSPButtToucher.KSPHeadMaster.PlanetMap.ElementAt(i).Key;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Layer Up")) { if (AltLayer < KWSKSPButtToucher.KSPHeadMaster.PlanetMap[body].LiveMap.Count - 1) { AltLayer++; } }
            if (GUILayout.Button("Layer Down")) { if (AltLayer > 0) { AltLayer--; } }
            GUILayout.EndHorizontal();
            //Debug.Log("1");
            //GUILayout.Label("CellID: " + CellIDLabel); CellIDLabel = GUILayout.TextField(CellIDLabel, 10); CellIDInt = int.Parse(CellIDLabel);
            GUILayout.Label("Body: " + testBody.name);
            GUILayout.Label("Layer: " + AltLayer);
            GUILayout.Label("Current location: " + FlightGlobals.ActiveVessel.GetWorldPos3D().ToString());
            //GUILayout.Label("Temperature: " + WeatherFunctions.getCellTemperature(FlightGlobals.currentMainBody, WeatherFunctions.getCellAtLocation(FlightGlobals.currentMainBody, FlightGlobals.ActiveVessel.GetWorldPos3D())));
            GUILayout.Label("Temperature: " + ((KSPHeadMaster.PlanetMap[testBody].LiveMap[AltLayer][cell].Temperature - 273.15)).ToString("0.0000"));
            GUILayout.Label("Pressure: " + KSPHeadMaster.PlanetMap[testBody].LiveMap[AltLayer][cell].Pressure.ToString("0.000000000"));
            GUILayout.Label("Density: " + KSPHeadMaster.PlanetMap[testBody].LiveMap[AltLayer][cell].Density);
            //Debug.Log("2");
            GUILayout.Label("Cell Latitude: " + WeatherFunctions.getLatitude(cell));
            GUILayout.Label("Cell Longitude: " + WeatherFunctions.getLongitude(cell));
            GUILayout.Label("Cell Altitude: " + KSPHeadMaster.PlanetMap[testBody].LiveMap[AltLayer][cell].Altitude);
            GUILayout.Label("isOcean?: " + KSPWeatherFunctions.isOcean(testBody, cell));
            GUILayout.Label("Albedo: " + KSPHeadMaster.PlanetMap[testBody].LiveMap[AltLayer][cell].Albedo);
            GUILayout.Label("Daytime?: " + Heating.isSunlight(KSPHeadMaster.PlanetMap[testBody], AltLayer, cell) + " " + Heating.getSunlightAngle(KSPHeadMaster.PlanetMap[testBody], AltLayer, cell));
            GUILayout.Label("Shortwave Abs: " + KWSKSPButtToucher.KSPHeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].SWAbsorbed);
            GUILayout.Label("Shortwave Out: " + KWSKSPButtToucher.KSPHeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].SWTransmitted);
            GUILayout.Label("Longwave In: " + KWSKSPButtToucher.KSPHeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].LWIn);
            //Debug.Log("3");
            //GUILayout.Label("Cell Pos: " + Cell.KWSBODY[FlightGlobals.currentMainBody][CellIDInt].CellPosition);
            
            GUILayout.EndVertical();

            GUI.DragWindow();
        }

    }
}
