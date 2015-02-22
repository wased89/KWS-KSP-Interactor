using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using GeodesicGrid;
using Weather;
using KWSKSPButtToucher;

namespace GUIUtils
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class InFlightGUI : MonoBehaviour
    {
        private Rect MainFlightGUI = new Rect(250,250,300,50);
        private Rect WeatherDataGUI = new Rect(250,250,450,500);


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
                MainFlightGUI = GUI.Window(MainFlightGUIID, MainFlightGUI, OnWindow, "Main GUI~");
                if (showWeatherDataGUI)
                {
                    GUI.Window(WeatherDataGUIID, WeatherDataGUI, WeatherDataUI, "WeatherData~");
                }
            }
            
        }

        void OnWindow(int windowID)
        {

            showWeatherDataGUI = GUILayout.Toggle(showWeatherDataGUI, "Weather Data~");

            GUI.DragWindow();
        }

        int i = 1;
        int AltLayer = 0;
        void WeatherDataUI(int windowID)
        {
            
            CelestialBody body = FlightGlobals.currentMainBody;
            PlanetSimulator pSim = HeadMaster.PlanetMap[body];
            Cell cell = WeatherFunctions.getCellAtLocation(body, FlightGlobals.ActiveVessel.GetWorldPos3D());

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Body Up")) {if( i < HeadMaster.PlanetMap.Keys.Count - 1) i++; };
            if (GUILayout.Button("Body Down")) { if (i > 0) i--; };
            GUILayout.EndHorizontal();

            CelestialBody testBody = HeadMaster.PlanetMap.ElementAt(i).Key;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Layer Up")) { if (AltLayer < HeadMaster.PlanetMap[body].LiveMap.Count - 1) { AltLayer++; } }
            if (GUILayout.Button("Layer Down")) { if (AltLayer > 0) { AltLayer--; } }
            GUILayout.EndHorizontal();
            //Debug.Log("1");
            //GUILayout.Label("CellID: " + CellIDLabel); CellIDLabel = GUILayout.TextField(CellIDLabel, 10); CellIDInt = int.Parse(CellIDLabel);
            GUILayout.Label("Body: " + testBody.name);
            GUILayout.Label("Layer: " + AltLayer);
            GUILayout.Label("Current location: " + FlightGlobals.ActiveVessel.GetWorldPos3D().ToString());
            //GUILayout.Label("Temperature: " + WeatherFunctions.getCellTemperature(FlightGlobals.currentMainBody, WeatherFunctions.getCellAtLocation(FlightGlobals.currentMainBody, FlightGlobals.ActiveVessel.GetWorldPos3D())));
            GUILayout.Label("Temperature: " + ((WeatherFunctions.getCellTemperature(testBody, AltLayer, cell) - 273.15)).ToString("0.0000"));
            GUILayout.Label("Pressure: " + HeadMaster.PlanetMap[testBody].LiveMap[AltLayer][cell].Pressure.ToString("0.000000000"));
            GUILayout.Label("Density: " + HeadMaster.PlanetMap[testBody].LiveMap[AltLayer][cell].Density);
            //Debug.Log("2");
            GUILayout.Label("Cell Latitude: " + WeatherFunctions.getCellLatitude(cell));
            GUILayout.Label("Cell Longitude: " + WeatherFunctions.getCellLongitude(cell));
            GUILayout.Label("Cell Altitude: " + WeatherFunctions.getCellAltitude(testBody, AltLayer, cell));
            GUILayout.Label("isOcean?: " + WeatherFunctions.cellContainsOcean(testBody, AltLayer, cell));
            GUILayout.Label("Albedo: " + WeatherFunctions.getCellAlbedo(testBody, AltLayer, cell));
            GUILayout.Label("Daytime?: " + WeatherFunctions.IsSunlight(testBody, AltLayer, cell) + " " + WeatherFunctions.getSunlightAngle(testBody, AltLayer, cell));
            GUILayout.Label("Shortwave Abs: " + HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].SWAbsorbed);
            GUILayout.Label("Shortwave Out: " + HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].SWTransmitted);
            GUILayout.Label("Longwave In: " + HeadMaster.PlanetMap[body].LiveMap[AltLayer][cell].LWIn);
            //Debug.Log("3");
            //GUILayout.Label("Cell Pos: " + Cell.KWSBODY[FlightGlobals.currentMainBody][CellIDInt].CellPosition);
            
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

    }
}
