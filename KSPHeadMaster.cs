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


namespace KWSKSPButtToucher
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class KSPHeadMaster : MonoBehaviour
    {
        //PlanetSimulator pSim;
        
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
                    KSPWeatherCallbacks kspwcb = new KSPWeatherCallbacks(body);
                    PlanetSimulator pSim = new PlanetSimulator(KWSSettings.gridLevel, KWSSettings.Layers,  kspwcb.SunDirection, kspwcb.SunlightAngle);
                    
                    PlanetMap.Add(body, pSim);
                    
                    PlanetMap[body].bufferFlip += BufferFlip;
                    Debug.Log("Map added to: " + body.name);
                }
            }
            setInitVars();
            hasGenerated = true;
        }
        void setInitVars()
        {
            foreach(CelestialBody body in PlanetMap.Keys)
            {
                PlanetMap[body].SetBodyKSun((float)Weather.Heating.calculateBodyKSun(body.orbit.radius));

                for (int AltLayer = 0; AltLayer < PlanetMap[body].LiveMap.Count; AltLayer++)
                {
                    foreach(Cell cell in Cell.AtLevel(KWSSettings.gridLevel))
                    {
                        PlanetMap[body].SetInitTempOfCell(FlightGlobals.getExternalTemperature
                            (PlanetMap[body].LiveMap[AltLayer][cell].Altitude,body), AltLayer, cell);

                        PlanetMap[body].SetInitPressureOfCell((float)FlightGlobals.getStaticPressure
                            (PlanetMap[body].LiveMap[AltLayer][cell].Altitude, body), AltLayer, cell);

                        PlanetMap[body].SetInitDensityOfCell((float)FlightGlobals.getAtmDensity
                            (PlanetMap[body].LiveMap[AltLayer][cell].Pressure), AltLayer, cell);

                        
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
                        PlanetSimulator pSim = PlanetMap[FlightGlobals.currentMainBody];
                        pSim.UpdateNCells(KWSSettings.CellsPerUpdate);

                    }

                    //Debug.Log(PlanetMap[body].Count);
                }
                second = 0f;
            }
            
        }

        void BufferFlip()
        {
            
            
        }
        
    }
}
