using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace KWSKSPButtToucher
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    class Settings : MonoBehaviour
    {
        internal static int gridLevel = 5;
        internal static int Layers = 6;
        //internal static int WeatherTickRate = 1;
        internal static int CellsPerUpdate = 50;
        internal static float cellDefinitionAlt = 2500;
        private int dummyGridDef = gridLevel;

        private static Rect SettingsGUI = new Rect(100, 100, 250, 250);
        private int SettingsGUIID;
        private bool showSettings = true;

        void OnGUI()
        {
            if (showSettings)
            {
                SettingsGUI = GUI.Window(SettingsGUIID, SettingsGUI, OnWindow, "KWS Settings~");
            }
        }

        void OnWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Weather Grid Level: " + dummyGridDef);
            GUILayout.Label("Weather Tick Rate: " + (HeadMaster.WeatherTickRate * 0.02).ToString("0.00") + " seconds");
            GUILayout.Label("Altitude Increment: " + cellDefinitionAlt + " metres");
            GUILayout.Label("Cells per Tick: " + CellsPerUpdate);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Grid Lvl Up")) { dummyGridDef++; }
            if (GUILayout.Button("Grid Lvl Down")) { if (dummyGridDef > 5) { dummyGridDef--; } }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Tick Up")) { HeadMaster.WeatherTickRate += 1; }
            if (GUILayout.Button("Tick Down")) { if (HeadMaster.WeatherTickRate >= 1) { HeadMaster.WeatherTickRate -= 1; } }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cells Up")) { CellsPerUpdate += 1; }
            if (GUILayout.Button("Cells Down")) { if (CellsPerUpdate > 0) { CellsPerUpdate -= 1; } }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Generate New Grid!")) 
            { 
                gridLevel = dummyGridDef; 
                HeadMaster.hasGenerated = false; 

            }
            GUILayout.EndVertical();

        }
    }
}
