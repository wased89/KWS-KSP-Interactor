using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using KSP.IO;
using GeodesicGrid;
using Weather;
using GUIUtils;
using KerbalWeatherSystems;

namespace KWSKSPButtToucher
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class HeadMaster : MonoBehaviour
    {
        PlanetSimulator pSim;
        SimulatorDisplay simDisplay;
        
        public static  Dictionary<CelestialBody, PlanetSimulator> PlanetMap;

        private float second;
        internal static int WeatherTickRate = 1;
        internal static bool hasGenerated = false;

        void Awake()
        {
            DontDestroyOnLoad(this);
            
            
        }

        void GenerateNewGrids()
        {
            PlanetMap = new Dictionary<CelestialBody, PlanetSimulator>();
            int i = 0;
            foreach(CelestialBody body in FlightGlobals.Bodies)
            {
                i++;
                if(body.atmosphere)
                {
                    Debug.Log(body.name + " has atmosphere!");
                    pSim = new PlanetSimulator(Settings.gridLevel, Settings.Layers, WeatherFunctions.getSunPosition);
                    
                    PlanetMap.Add(body, pSim);
                    
                    PlanetMap[body].bufferFlip += BufferFlip;
                    Debug.Log("Map added to: " + body.name);
                }
            }
            setInitTemps();
            hasGenerated = true;
        }
        void setInitTemps()
        {
            foreach(CelestialBody body in PlanetMap.Keys)
            {
                for (int AltLayer = 0; AltLayer < PlanetMap[body].LiveMap.Count; AltLayer++)
                {
                    foreach(Cell cell in Cell.AtLevel(Settings.gridLevel))
                    {
                        PlanetMap[body].SetInitTempOfCell(FlightGlobals.getExternalTemperature
                            (PlanetMap[body].LiveMap[AltLayer][cell].Altitude,body), AltLayer, cell);
                    }
                    
                }
            }
        }
        void FixedUpdate()
        {
            if(hasGenerated == false)
            {
                GenerateNewGrids();
            }
            second += 0.02f;
            if (second >= (WeatherTickRate * 0.02) && (WeatherTickRate * 0.02) != 0)
            {
                if (FlightGlobals.currentMainBody != null && PlanetMap.ContainsKey(FlightGlobals.currentMainBody))
                {
                    //Debug.Log("Weather Tick!");
                    //Debug.Log(body.name + " has: " + PlanetMap[body].Count + " layers.");
                    if (hasGenerated == true)
                    {
                        pSim = PlanetMap[FlightGlobals.currentMainBody];
                        pSim.UpdateNCells(Settings.CellsPerUpdate);

                    }

                    //Debug.Log(PlanetMap[body].Count);
                }
                second = 0f;
            }
            
        }

        void BufferFlip()
        {
            
            simDisplay.OnBufferChange();
        }
        
    }
}
